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

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets whether or not the adapter is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the name of the message adapter.
        /// </summary>
        public string Name { get { return IntentMessaging.GetName(this); } }

        /// <summary>
        /// When overriden in derived classes, returns the default settings
        /// dictionary as a parsed JavaScript object.
        /// </summary>
        public virtual CommonObject DefaultSettings
        {
            get
            {
                return IntentMessaging.Script.Execute(FormatSettingsScript(DefaultSettingsScript)) as CommonObject;
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

        #endregion Constructors

        #region Methods

        #region Operation

        public void Start()
        {
            if (IsRunning) return;
            IntentMessaging.WriteLine("Starting: " + Name);
            OnStart();
            IsRunning = true;
        }

        protected virtual void OnStart() { }

        public void Stop()
        {
            if (!IsRunning) return;
            IntentMessaging.WriteLine("Stopping: " + Name);
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
                var settings = (CommonObject)IntentMessaging.Script.Execute(FormatSettingsScript(settingsScript));

                // Pass the parsed settings data down for custom consumption
                ApplySettings(settings);
                SettingsException = null;
                HasErrors = false;
            }
            catch (IronJS.Error.CompileError ce)
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

        #endregion Methods
    }
}
