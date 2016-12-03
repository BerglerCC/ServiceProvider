using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Bergler.Common.ServiceProvider.Attributes;
using Bergler.Common.Tools;

namespace Bergler.Common.ServiceProvider.Framework
{
    [RunInstaller(true)]
    public partial class WindowsServiceInstaller : Installer
    {
        public WindowsServiceInstaller() : this(typeof (ServiceImplementation))
        {
            InitializeComponent();
        }

        public WindowsServiceInstaller(Type windowsServiceType)
        {
            if (!windowsServiceType.GetInterfaces().Contains(typeof (IWindowsService)))
                throw new ArgumentException("Type to install must implement IWindowsService.",
                    nameof(windowsServiceType));

            var attribute = windowsServiceType.GetAttribute<WindowsServiceConfigurationAttribute>();

            if (attribute == null)
                throw new ArgumentException("Type to install must be marked with a WindowsServiceAttribute.",
                    nameof(windowsServiceType));

            Configuration = attribute;
        }

        public WindowsServiceConfigurationAttribute Configuration { get; set; }

        public static void RuntimeInstall<T>() where T : IWindowsService
        {
            InstallContext<T>(new Hashtable());
        }

        public static void RuntimeUnInstall<T>(params Installer[] otherInstallers) where T : IWindowsService
        {
            InstallContext<T>();
        }

        private static void InstallContext<T>(dynamic args = null) where T : IWindowsService
        {
            var path = "/assemblypath=" + Assembly.GetEntryAssembly().Location;
            using (var ti = new TransactedInstaller())
            {
                ti.Installers.Add(new WindowsServiceInstaller(typeof (T)));
                ti.Context = new InstallContext(null, new[] {path});

                if (args == null)
                {
                    ti.Uninstall(null);
                }
                else
                {
                    ti.Install(args);
                }
            }
        }

        public override void Install(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Installing service {0}.", Configuration.Name);

            ConfigureInstallers();
            base.Install(savedState);
        }

        public override void Uninstall(IDictionary savedState)
        {
            ConsoleHarness.WriteToConsole(ConsoleColor.White, "Un-Installing service {0}.", Configuration.Name);

            ConfigureInstallers();
            base.Uninstall(savedState);
        }

        private void ConfigureInstallers()
        {
            Installers.Add(ConfigureProcessInstaller());
            Installers.Add(ConfigureServiceInstaller());
        }

        private ServiceProcessInstaller ConfigureProcessInstaller()
        {
            var result = new ServiceProcessInstaller();

            if (string.IsNullOrEmpty(Configuration.UserName))
            {
                result.Account = ServiceAccount.LocalService;
                result.Username = null;
                result.Password = null;
            }
            else
            {
                result.Account = ServiceAccount.User;
                result.Username = Configuration.UserName;
                result.Password = Configuration.Password;
            }
            return result;
        }

        private ServiceInstaller ConfigureServiceInstaller()
        {
            return new ServiceInstaller
            {
                ServiceName = Configuration.Name,
                DisplayName = Configuration.DisplayName,
                Description = Configuration.Description,
                StartType = Configuration.StartMode
            };
        }
    }
}