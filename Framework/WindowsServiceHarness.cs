using System;
using System.ServiceProcess;
using Bergler.Common.ServiceProvider.Attributes;
using Bergler.Common.Tools;

namespace Bergler.Common.ServiceProvider.Framework
{
    public sealed partial class WindowsServiceHarness : ServiceBase
    {

        public IWindowsService ServiceImplementation { get; private set; }
        public WindowsServiceHarness()
        {
            InitializeComponent();
        }

        public WindowsServiceHarness(IWindowsService serviceImplementation)
            : this()
        {
            if (serviceImplementation == null)
            {
                throw new ArgumentNullException(nameof(serviceImplementation), "IWindowsService cannot be null in call to GenericWindowsService");
            }
            ServiceImplementation = serviceImplementation;
            ConfigureServiceFromAttributes(serviceImplementation);
        }

        protected override void OnStart(string[] args)
        {
            ServiceImplementation.Start(args);
        }

        protected override void OnStop()
        {
            ServiceImplementation.Stop();
        }

        protected override void OnPause()
        {
            ServiceImplementation.Pause();
        }

        protected override void OnContinue()
        {
            ServiceImplementation.Continue();
        }

        protected override void OnCustomCommand(int command)
        {
            ServiceImplementation.CustomCommand(command);
        }

        protected override void OnShutdown()
        {
            ServiceImplementation.Shutdown();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                ServiceImplementation.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ConfigureServiceFromAttributes(IWindowsService serviceImplementation)
        {
            var attribute = serviceImplementation.GetType().GetAttribute<WindowsServiceConfigurationAttribute>();
            if (attribute != null)
            {
                EventLog.Source = string.IsNullOrEmpty(attribute.EventLogSource)
                    ? "WindowsServiceHarness"
                    : attribute.EventLogSource;
                CanStop = attribute.CanStop;
                CanPauseAndContinue = attribute.CanPauseAndContinue;
                CanShutdown = attribute.CanShutdown;
                CanHandlePowerEvent = false;
                CanHandleSessionChangeEvent = false;
                AutoLog = true;
            }
            else
            {
                throw new InvalidOperationException(
                    $"IWindowsService implementer {serviceImplementation.GetType().FullName} must have a WindowsServiceAttribute.");
            }
        }
    }
}