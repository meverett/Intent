using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.IO;
using System.Windows.Forms;

using IronJS;
using IronJS.Hosting;
using IronJS.Native;

namespace Intent
{
    /// <summary>
    /// The core intent messaging runtime.
    /// </summary>
    public static class IntentRuntime
    {
        #region Fields

        #region Console

        // Internal version of the console buffer allocated on the large object heap
        static StringBuilder consoleBuffer = new StringBuilder(96000);

        #endregion Console

        #region Adapters

        // Current list of loaded and configured message adapters.
        static List<MessageAdapter> adapters = new List<MessageAdapter>();
        static Dictionary<int, MessageAdapter> adaptersById = new Dictionary<int, MessageAdapter>();

        #endregion Adapters;

        #region Script

        // Global script context for message adapters
        private static CSharp.Context script;
        
        // Used to call script update loops
        static System.Threading.Timer updateTimer;

        // List of adapter script udpate functions/callbacks
        static Dictionary<MessageAdapter, FunctionObject> updateFunctions = new Dictionary<MessageAdapter, FunctionObject>();

        #endregion Script

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the runtime's JavaScript context.
        /// </summary>
        public static CSharp.Context Script { get { return script; } }

        /// <summary>
        /// Gets the last text update that was written to the console.
        /// </summary>
        public static string LastConsoleText { get; private set; }

        /// <summary>
        /// Gets the current contents of the console buffer.
        /// </summary>
        public static string Console { get { return consoleBuffer.ToString(); } }

        /// <summary>
        /// Gets whether or not the messaging system is currently running.
        /// </summary>
        public static bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the list of currently active message adapters.
        /// </summary>
        public static ICollection<MessageAdapter> ActiveAdapters
        {
            get
            {
                lock (adapters)
                {
                    return adapters.ToList();
                }
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when new information is available in the console output.
        /// </summary>
        public static event EventHandler ConsoleUpdated;

        /// <summary>
        /// Occurs when a <see cref="MessageAdapter"/> has been added or
        /// removed to the list of active adapters.
        /// </summary>
        public static event EventHandler AdaptersUpdated;

        /// <summary>
        /// Occurs when the messaging system has been started.
        /// </summary>
        public static event EventHandler Started;

        /// <summary>
        /// Occurs when the messaging system has been stopped.
        /// </summary>
        public static event EventHandler Stopped;

        #endregion Events

        #region Constructors

        static IntentRuntime()
        {
            #region Initialize Script Runtime

            script = new CSharp.Context();

            // Add global script functions
            script.SetGlobal("print", Utils.CreateFunction<Action<BoxedValue>>(script.Environment, 1, _Print));
            script.SetGlobal("addAdapter", Utils.CreateFunction<Func<string, CommonObject, BoxedValue>>(script.Environment, 2, _AddAdapter));
            script.SetGlobal("sendMessage", Utils.CreateFunction<Action<BoxedValue, CommonObject>>(script.Environment, 2, MessageAdapter._SendMessage));
            script.SetGlobal("getMousePos", Utils.CreateFunction<Func<CommonObject>>(script.Environment, 0, _GetMousePosition));

            #endregion Initialize Script Runtime
        }

        #endregion Constructors

        #region Methods

        #region Operation

        /// <summary>
        /// Starts the messaging system.
        /// </summary>
        public static void Start()
        {
            if (IsRunning) return;

            lock(adapters)
            {
                WriteLine("Intent Messaging => Starting");

                // Clear the current list of update functions
                updateFunctions.Clear();

                foreach (var adapter in adapters)
                {
                    // Check to see if the adapter implements an update function/callback
                    if (adapter.CurrentSettings != null)
                    {
                        var members = adapter.CurrentSettings.Members;

                        // If the update function exists, add it to the list
                        if (members.ContainsKey("update") && members["update"] is FunctionObject)
                            updateFunctions.Add(adapter, (FunctionObject)members["update"]);
                    }

                    // Start the adapter
                    adapter.Start();
                }

                // Start the update functions
                if (updateFunctions.Count > 0 || script.GetGlobal("update").IsFunction)
                {
                    updateTimer = new System.Threading.Timer((state) =>
                    {
                        lock (updateFunctions)
                        {
                            // Call update functions
                            foreach (KeyValuePair<MessageAdapter, FunctionObject> pair in updateFunctions)
                            {
                                pair.Value.Call(pair.Key.CurrentSettings);
                            }

                            // Call global update timer
                            var globalUpdate = script.GetGlobal("update");
                            if (globalUpdate.IsFunction) globalUpdate.Func.Call(null);
                        }
                        
                    }, null, 0, 22);
                }

                IsRunning = true;
                WriteLine("Intent Messaging => Started");
            }

            // Notify
            if (Started != null) Started(null, EventArgs.Empty);
        }

        /// <summary>
        /// Stops the messaging system.
        /// </summary>
        public static void Stop()
        {
            if (!IsRunning) return;

            lock (adapters)
            {
                WriteLine("Intent Messaging => Stopping");

                // Stop update loop thread if started
                if (updateTimer != null) updateTimer.Dispose();

                foreach (var adapter in adapters) { adapter.Stop(); adapter.HasErrors = false; }
                IsRunning = false;
                WriteLine("Intent Messaging => Stopped");
            }

            // Notify
            if (Stopped != null) Stopped(null, EventArgs.Empty);
        }

        #endregion Operation

        #region Adapters

        #region Information

        /// <summary>
        /// Gets the information for all available <see cref="MessageAdapter">message adapters</see>.
        /// </summary>
        /// <returns>An array containing the list of available message adapters.</returns>
        public static MessageAdapterInfo[] GetAdapterInfo()
        {
            return (from t in GetAdapterTypes() select new MessageAdapterInfo(GetName(t), t)).ToArray();
        }

        /// <summary>
        /// Gets the list of names of the available message adapters.
        /// </summary>
        /// <returns>A list of the available message adapters by name.</returns>
        public static string[] GetAdapterNames()
        {
            List<string> names = new List<string>();
            foreach (Type t in GetAdapterTypes()) names.Add(GetName(t));
            return names.ToArray();
        }

        public static Type[] GetAdapterTypes()
        {
            // Go through available types
            var adapterTypes = from t in Assembly.GetAssembly(typeof(MessageAdapter)).GetTypes()
                               where t.GetCustomAttributes(typeof(MessageAdapterAttribute)).Count() > 0
                               select t;

            return adapterTypes.ToArray();
        }

        /// <summary>
        /// Gets the name of a <see cref="MessageAdapter">Message Adapter</see>.
        /// </summary>
        /// <param name="adapter">The adapter who's name will be retrieved.</param>
        /// <returns>The adapter's name if it was found, otherwise NULL.</returns>
        public static string GetName(MessageAdapter adapter)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            return GetName(adapter.GetType());
        }

        /// <summary>
        /// Gets the name of a <see cref="MessageAdapter">Message Adapter</see>.
        /// </summary>
        /// <param name="type">The adapter who's name will be retrieved.</param>
        /// <returns>The adapter's name if it was found, otherwise NULL.</returns>
        public static string GetName(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var attribute = type.GetCustomAttributes(typeof(MessageAdapterAttribute), false).
                            FirstOrDefault()
                            as MessageAdapterAttribute;

            // No adapter attribute, no name...
            if (attribute == null) return null;

            return attribute.Name;
        }

        /// <summary>
        /// Gets a message adapter instance given its instance ID.
        /// </summary>
        /// <param name="id">The instance ID of the adapter to get.</param>
        /// <returns>The adapter if it is found, otherwise NULL.</returns>
        public static MessageAdapter GetAdapterById(int id)
        {
            return adaptersById.ContainsKey(id) ? adaptersById[id] : null;
        }

        #endregion Information

        #region Adding/Removing/Clearing

        /// <summary>
        /// Adds a specific message adapter to the messaging engine.
        /// </summary>
        /// <param name="type">The message adapter type to add.</param>
        /// <param name="settings">Optional JavaScript settings data to use to configure the adapter.</param>
        /// <returns>The newly added message adapter instance.</returns>
        public static MessageAdapter AddAdapter(Type type, CommonObject settings)
        {
            lock (adapters)
            {
                if (type == null) throw new ArgumentNullException("type");

                // Create
                var adapter = (MessageAdapter)Activator.CreateInstance(type);

                // Configure
                if (settings == null) adapter.ApplySettings(adapter.DefaultSettingsScript);
                else adapter.ApplySettings(settings);

                // Add
                adapters.Add(adapter);
                if (!adaptersById.ContainsKey(adapter.Id)) adaptersById.Add(adapter.Id, adapter);
                else adaptersById[adapter.Id] = adapter;

                IntentRuntime.WriteLine("Added => {0}", adapter.Name);

                // Start the adapter if we're already running
                if (IsRunning) adapter.Start();

                // Notify
                if (AdaptersUpdated != null) AdaptersUpdated(null, EventArgs.Empty);

                // Return
                return adapter;
            }
        }

        /// <summary>
        /// Adds a specific message adapter to the messaging engine.
        /// </summary>
        /// <param name="type">The message adapter type to add.</param>
        /// <param name="settingsScript">Optional JavaScript settings script to use to configure the adapter.</param>
        /// <returns>The newly added message adapter instance.</returns>
        public static MessageAdapter AddAdapter(Type type, string settingsScript)
        {
            lock (adapters)
            {
                if (type == null) throw new ArgumentNullException("type");

                // Create
                var adapter = (MessageAdapter)Activator.CreateInstance(type);

                // Configure
                if (string.IsNullOrEmpty(settingsScript)) settingsScript = adapter.DefaultSettingsScript;
                adapter.ApplySettings(settingsScript);

                // Add
                adapters.Add(adapter);
                if (!adaptersById.ContainsKey(adapter.Id)) adaptersById.Add(adapter.Id, adapter);
                else adaptersById[adapter.Id] = adapter;

                IntentRuntime.WriteLine("Added => {0}", adapter.Name);

                // Start the adapter if we're already running
                if (IsRunning) adapter.Start();

                // Notify
                if (AdaptersUpdated != null) AdaptersUpdated(null, EventArgs.Empty);

                // Return
                return adapter;
            }
        }

        /// <summary>
        /// Adds a specific message adapter to the messaging engine.
        /// </summary>
        /// <param name="type">The message adapter type to add.</param>
        /// <returns>The newly added message adapter instance.</returns>
        public static MessageAdapter AddAdapter(Type type)
        {
            return AddAdapter(type, "");
        }

        /// <summary>
        /// Adds a specific message adapter to the messaging engine.
        /// </summary>
        /// <param name="name">The name of the message adapter to add.</param>
        /// <param name="settings">Optional javascript settings data to use to configure the adapter.</param>
        /// <returns>The newly added message adapter instance.</returns>
        public static MessageAdapter AddAdapter(string name, CommonObject settings)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            
            // Find the adapter type by the supplied name
            var nameLower = name.ToLower();
            var type = GetAdapterTypes().FirstOrDefault(t => GetName(t).ToLower() == nameLower);

            // Make sure it was found
            if (type == null)throw new ArgumentException("No message adapter was found by the name: " + name);

            // Add it
            return AddAdapter(type, settings);
        }

        /// <summary>
        /// Adds a specific message adapter to the messaging engine.
        /// </summary>
        /// <param name="name">The name of the message adapter to add.</param>
        /// <param name="settingsScript">Optional javascript settings script to use to configure the adapter.</param>
        /// <returns>The newly added message adapter instance.</returns>
        public static MessageAdapter AddAdapter(string name, string settingsScript)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            // Find the adapter type by the supplied name
            var nameLower = name.ToLower();
            var type = GetAdapterTypes().FirstOrDefault(t => GetName(t).ToLower() == nameLower);

            // Make sure it was found
            if (type == null) throw new ArgumentException("No message adapter was found by the name: " + name);

            // Add it
            return AddAdapter(type, settingsScript);
        }

        /// <summary>
        /// Adds a specific message adapter to the messaging engine.
        /// </summary>
        /// <param name="name">The name of the message adapter to add.</param>
        /// <returns>The newly added message adapter instance.</returns>
        public static MessageAdapter AddAdapter(string name)
        {
            return AddAdapter(name, "");
        }

        /// <summary>
        /// Removes a message adapter from the list of currently active adapters.
        /// </summary>
        /// <param name="adapter">The message adapter to remove.</param>
        public static void RemoveAdapter(MessageAdapter adapter)
        {
            lock (adapters)
            {
                if (IsRunning) adapter.Stop();
                adapters.Remove(adapter);
                adaptersById.Remove(adapter.Id);
                IntentRuntime.WriteLine("Removed => {0}", adapter.Name);
            }

            // Notify
            if (AdaptersUpdated != null) AdaptersUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Removes a message adapter from the list of currently active adapters
        /// given its position in the list by index.
        /// </summary>
        /// <param name="index">The index of the messaging adapter to remove.</param>
        public static void RemoveAdapter(int index)
        {
            if (index < 0 || adapters.Count == 0 || index >= adapters.Count)
                throw new IndexOutOfRangeException("index");

            lock (adapters)
            {
                // Remove the adapter; stop it if we're currently running
                var adapter = adapters[index];
                if (IsRunning) adapter.Stop();
                adapters.RemoveAt(index);
                adaptersById.Remove(adapter.Id);
                IntentRuntime.WriteLine("Removed => {0}", adapter.Name);
            }

            // Notify
            if (AdaptersUpdated != null) AdaptersUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Clears the list of currently active message adapters.
        /// </summary>
        public static void ClearAdapters()
        {
            lock (adapters)
            {
                foreach (var adapter in adapters) adapter.Stop();
                adapters.Clear();
                adaptersById.Clear();
            }
        }

        #endregion Adding/Removing/Clearing

        #endregion Adapters

        #region Script

        #region Files

        /// <summary>
        /// Loads an Intent script file.
        /// </summary>
        /// <param name="filePath">The path to the script file to load.</param>
        public static void LoadScript(string filePath)
        {
            #region Validate

            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found: " + filePath);

            #endregion Validate

            // Read the file in
            var scriptText = File.ReadAllText(filePath);

            // Attempt to load and parse it
            try
            {
                script.Execute(scriptText);
            }
            catch (IronJS.Error.CompileError ce)
            {
                IntentRuntime.WriteLine("Error loading script: {0}\n{1}", filePath, ce);
            }
        }

        /// <summary>
        /// Loads all Intent script files within a given directory.
        /// </summary>
        /// <param name="dirPath">The path to the directory to load all scripts from.</param>
        public static void LoadAllScripts(string dirPath)
        {
            #region Validate

            if (string.IsNullOrEmpty(dirPath))
                throw new ArgumentNullException("dirPath");

            if (!Directory.Exists(dirPath))
                throw new DirectoryNotFoundException("Directory not found: " + dirPath);

            #endregion Validate

            // Get all script files in the directory and sort in preferential order
            var scriptFiles = Directory.GetFiles(dirPath, "*.js")
                                .OrderByDescending(f => Path.GetFileName(f).ToLower().StartsWith("setup"))
                                .OrderByDescending(f => Path.GetFileName(f).ToLower().StartsWith("lib."));

            // Load all JavaScript files found in the directory
            foreach (string filePath in scriptFiles) 
            {
                IntentRuntime.WriteLine("Loading {0}...", filePath); 
                LoadScript(filePath);
            }
        }

        #endregion Files

        #region Script Utility

        // Given the input script - make sure it will compile and return an object
        static string FormatScript(string scriptText)
        {
            // Make sure that a variable declaration prepends the JavaScript or the parser will throw an error
            if (!scriptText.StartsWith("var") && (scriptText.StartsWith("{") || scriptText.StartsWith("[")))
                scriptText = "var data = " + scriptText;

            return scriptText;
        }

        #endregion ScriptUtility

        #region Global Functions

        // script: print() 
        static void _Print(BoxedValue value)
        {
            IntentRuntime.WriteLine(value.ClrBoxed);
        }

        // script: addAdapter(name, settings)
        static BoxedValue _AddAdapter(string name, CommonObject settings)
        {
            #region Validate

            if (string.IsNullOrEmpty(name))
            {
                //    throw new ArgumentNullException("name cannot be NULL or empty.");
                IntentRuntime.WriteLine("addAdapter(name, settings): 'name' cannot be NULL or empty");
                return TypeConverter.ToBoxedValue(false);
            }
            

            if (settings == null)
            {
                //    throw new ArgumentNullException("settings cannot be NULL.");
                IntentRuntime.WriteLine("addAdapter(name, settings): 'settings' cannot be NULL or empty");
                return TypeConverter.ToBoxedValue(false);
            }

            // Get the adapter info for the given adapter name
            var info = GetAdapterInfo().SingleOrDefault(a => a.Name.ToLower() == name.Trim().ToLower());

            // Make sure an adapter by the provided name was found
            if (info == null)
            {
                //throw new ArgumentException("Message Adapter '{0}' not found.", name);
                IntentRuntime.WriteLine("addAdapter(name, settings): Message Adapter '{0}' not found.", name);
                return TypeConverter.ToBoxedValue(false);
            }

            #endregion Validate

            #region Add Adapter

            // Add the new adapter and associate it with the supplied settings object
            MessageAdapter adapter = null;

            try
            {
                adapter = AddAdapter(name, settings);
            }
            catch (Exception ex)
            {
                IntentRuntime.WriteLine("addAdapter(): An error occured while adding the message adapter.\n{0}", ex);
                return TypeConverter.ToBoxedValue(false);
            }

            #endregion Add Adapter

            #region Bind Identifcation

            // Add the adapter name to the script settings object
            adapter.CurrentSettings.Put("adapter", adapter.Name);

            // Add the adapter's unique instance ID to the script settings object
            adapter.CurrentSettings.Put("id", adapter.Id);

            #endregion Bind Identifcation

            return TypeConverter.ToBoxedValue(true);
        }

        // script: getMousePos()
        static CommonObject _GetMousePosition()
        {
            // Get mouse position
            var position = Cursor.Position;

            // Create the outgoing object
            var pos = new CommonObject(script.Environment, null);

            // Set the mouse values
            pos.Put("x", position.X);
            pos.Put("y", position.Y);

            // Return it
            return pos;
        }

        #endregion Global Functions

        #endregion Script

        #region Debugging

        /// <summary>
        /// Clears the console buffer.
        /// </summary>
        public static void ClearConsole()
        {
            consoleBuffer.Clear();
        }

        /// <summary>
        /// Writes the given value to the console.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public static void Write(object value)
        {
            LastConsoleText = string.Format("{0}", value);
            consoleBuffer.Append(LastConsoleText);
            System.Console.Write(LastConsoleText);
            if (ConsoleUpdated != null) ConsoleUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Writes the given formatted string to the console.
        /// </summary>
        /// <param name="value">The formatted string to write.</param>
        /// <param name="args">The objects to inject into the formatted string.</param>
        public static void Write(string value, params object[] args)
        {
            LastConsoleText = string.Format(value, args);
            consoleBuffer.Append(LastConsoleText);
            System.Console.Write(LastConsoleText);
            if (ConsoleUpdated != null) ConsoleUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Writes a new line to the console.
        /// </summary>
        public static void WriteLine()
        {
            LastConsoleText = System.Environment.NewLine;
            consoleBuffer.Append(LastConsoleText);
            System.Console.Write(LastConsoleText);
            if (ConsoleUpdated != null) ConsoleUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Writes the given value as a new line in the console.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public static void WriteLine(object value)
        {
            LastConsoleText = string.Format("{0}", value) + System.Environment.NewLine;
            consoleBuffer.Append(LastConsoleText);
            System.Console.Write(LastConsoleText);
            if (ConsoleUpdated != null) ConsoleUpdated(null, EventArgs.Empty);
        }

        /// <summary>
        /// Writes the given formatted string as a new line into the console.
        /// </summary>
        /// <param name="value">The formatted string to write.</param>
        /// <param name="args">The objects to inject into the formatted string.</param>
        public static void WriteLine(string value, params object[] args)
        {
            LastConsoleText = string.Format(value, args) + System.Environment.NewLine;
            consoleBuffer.Append(LastConsoleText);
            System.Console.Write(LastConsoleText);
            if (ConsoleUpdated != null) ConsoleUpdated(null, EventArgs.Empty);
        }

        #endregion Debugging

        #endregion Methods
    }
}
