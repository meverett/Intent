using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Intent.Json;

using IronJS;
using IronJS.Hosting;

namespace Intent
{
    /// <summary>
    /// Base class for subclasses that implement a message adapting service.
    /// </summary>
    public abstract class MessageAdapter
    {
        #region Fields

        // Used to provide adapters with a unique ID
        static object idLock = new object();
        static int adapterId = 0;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the adapter's unique instance ID.
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Gets whether or not the adapter is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the name of the message adapter.
        /// </summary>
        public string Name { get { return IntentRuntime.GetName(this); } }

        /// <summary>
        /// When overriden in derived classes, returns the default settings
        /// dictionary as a parsed JavaScript object.
        /// </summary>
        public virtual CommonObject DefaultSettings
        {
            get
            {
                return IntentRuntime.Script.Execute(FormatSettingsScript(DefaultSettingsScript)) as CommonObject;
            }
        }

        /// <summary>
        /// When overriden in derived classes, returns the default settings
        /// JavaScript.
        /// </summary>
        public virtual string DefaultSettingsScript
        {
            get
            {
                return "{}";
            }
        }

        /// <summary>
        /// Gets the current settings object.
        /// </summary>
        public CommonObject CurrentSettings { get; private set; }

        /// <summary>
        /// Gets the current settings object script.
        /// </summary>
        public string CurrentSettingScript { get; private set; }
        
        /// <summary>
        /// Gets or sets whether or not the adapter currently has any errors or is
        /// in an error state.
        /// </summary>
        public bool HasErrors { get; set; }

        /// <summary>
        /// Any current exception associated with the adapter's settings script.
        /// </summary>
        public IronJS.Error.CompileError SettingsException { get; private set; }

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when a message has been received by the message adapter.
        /// </summary>
        public event EventHandler MessageReceived;

        /// <summary>
        /// Occurs when a message has been sent by the message adapter.
        /// </summary>
        public event EventHandler MessageSent;

        #endregion Events

        #region Constructors

        public MessageAdapter()
        {
            // Assign a new unique adapter ID
            lock (idLock)
            {
                Id = ++adapterId;
            }
        }

        #endregion Constructors

        #region Methods

        #region Operation

        public void Start()
        {
            if (IsRunning) return;
            IntentRuntime.WriteLine("Starting: " + Name);
            OnStart();
            IsRunning = true;
        }

        protected virtual void OnStart() { }

        public void Stop()
        {
            if (!IsRunning) return;
            IntentRuntime.WriteLine("Stopping: " + Name);
            OnStop();
            IsRunning = false;
        }

        protected virtual void OnStop() { }

        #endregion Operation

        #region Messaging

        /// <summary>
        /// Triggers a <see cref="MessageReceived"/> event.
        /// </summary>
        protected void TriggerMessageReceived()
        {
            if (MessageReceived != null) MessageReceived(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers a <see cref="MessageSent"/> event.
        /// </summary>
        protected void TriggerMessageSent()
        {
            if (MessageSent != null) MessageSent(this, EventArgs.Empty);
        }

        #endregion Messaging

        #region Settings/Configuration

        /// <summary>
        /// Applies adapter settings given the parsed JavaScript object that contains
        /// the settings data.
        /// </summary>
        /// <param name="settings">The JavaScript settings object.</param>
        public void ApplySettings(CommonObject settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            // Pass the parsed settings data down for custom consumption
            OnApplySettings(settings);

            // Assign the current settings to the adapter
            CurrentSettings = settings;
        }

        /// <summary>
        /// Applies adapter settings given a JavaScript string that defines the
        /// adapter settings object.
        /// </summary>
        /// <param name="settingsScript">The settings JavaScript to parse and apply.</param>
        public void ApplySettings(string settingsScript)
        {
            if (string.IsNullOrEmpty(settingsScript)) throw new ArgumentNullException("script");

            // Assign the current settings script to the adapter
            CurrentSettingScript = settingsScript;

            // Parse the settings javascript
            try
            {
                var settings = (CommonObject)IntentRuntime.Script.Execute(FormatSettingsScript(settingsScript));

                // Pass the parsed settings data down for custom consumption
                ApplySettings(settings);
                SettingsException = null;
                HasErrors = false;
            }
            catch (Error.CompileError ce)
            {
                HasErrors = true;
                SettingsException = ce;
            }
        }

        /// <summary>
        /// When overriden in derived classes, applies adapter settings given the
        /// parsed JavaScript object that contains the settings data.
        /// </summary>
        /// <param name="settings">The JavaScript settings object.</param>
        protected virtual void OnApplySettings(CommonObject settings) { }

        // Given the input settings script, ensures that it is ready for script execution
        string FormatSettingsScript(string settings)
        {
            // Make sure that a variable declaration prepends the JavaScript or the parser will throw an error
            if (!settings.StartsWith("var") && (settings.StartsWith("{") || settings.StartsWith("[")))
                settings = "var settings = " + settings;

            return settings;
        }

        #endregion Settings/Configuration

        #region Script

        // sendMessage(id, message); Sends an outgoing message from script on behalf of the message adapter
        internal static void _SendMessage(BoxedValue boxedId, CommonObject message)
        {
            // Don't send messages if we're not running
            if (!IntentRuntime.IsRunning) return;

            MessageAdapter adapter;

            #region Validate

            // Get the adapter in question
            var id = TypeConverter.ToInt32(boxedId);
            adapter = IntentRuntime.GetAdapterById(id);

            if (adapter == null)
            {
                IntentRuntime.WriteLine("sendMessage(id, message): message adapter ID '{0}' not found.", id);
                return;
            }

            if (message == null)
            {
                IntentRuntime.WriteLine("{0}:{1} -> sendMessage(id, message): message parameter cannot be NULL", adapter.Name, adapter.Id);
                return;
            }

            #endregion Validate

            // Pass on to subclass to send
            adapter.OnSendMessage(message);
        }

        /// <summary>
        /// When overriden in a derived class sends a message on behalf of a script invocation.
        /// </summary>
        /// <param name="message">The script message to send.</param>
        protected virtual void OnSendMessage(CommonObject message) { }

        #endregion Script

        #endregion Methods
    }
}
