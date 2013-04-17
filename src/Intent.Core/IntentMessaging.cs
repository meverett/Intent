using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using IronJS;
using IronJS.Hosting;

namespace Intent
{
    /// <summary>
    /// The core intent messaging runtime.
    /// </summary>
    public static class IntentMessaging
    {
        #region Fields

        #region Console

        // Internal version of the console buffer allocated on the large object heap
        static StringBuilder consoleBuffer = new StringBuilder(96000);

        #endregion Console

        #region Adapters

        // Current list of loaded and configured message adapters.
        static List<MessageAdapter> adapters = new List<MessageAdapter>();

        #endregion Adapters;

        #endregion Fields

        #region Properties

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

        #region Methods

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

        #endregion Information

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
                foreach (var adapter in adapters) adapter.Start();
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
                foreach (var adapter in adapters) adapter.Stop();
                IsRunning = false;
                WriteLine("Intent Messaging => Stopped");
            }
            // Notify
            if (Stopped != null) Stopped(null, EventArgs.Empty);

        }

        #endregion Operation

        #region Adapters

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

                IntentMessaging.WriteLine("Added => {0}", adapter.Name);

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

                IntentMessaging.WriteLine("Added => {0}", adapter.Name);

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
                IntentMessaging.WriteLine("Removed => {0}", adapter.Name);
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
                IntentMessaging.WriteLine("Removed => {0}", adapter.Name);
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
            }
        }

        #endregion Adding/Removing/Clearing

        #endregion Adapters

        #region Script

        public static void Script()
        {
            var script = new CSharp.Context();
            var result = script.Execute("var t = { 'test': 'hey', 'number': 432, 'do': function(i) { return i + 37; } }");

            if (result is CommonObject)
            {
                var obj = (CommonObject)result;
                var doFunc = (FunctionObject)obj.Members["do"];
                var arg = BoxedValue.Box(2);
                var val = doFunc.Call(obj, arg);
                System.Console.WriteLine(val.ClrBoxed);
            }
        }

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
