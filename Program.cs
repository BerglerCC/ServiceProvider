using System;
using System.Linq;
using System.ServiceProcess;
using Bergler.Common.ServiceProvider.Framework;

namespace Bergler.Common.ServiceProvider
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Contains("-i", StringComparer.InvariantCultureIgnoreCase))
            {
                WindowsServiceInstaller.RuntimeInstall<ServiceImplementation>();
            }
            else if (args.Contains("-u", StringComparer.InvariantCultureIgnoreCase))
            {
                WindowsServiceInstaller.RuntimeUnInstall<ServiceImplementation>();
            }
            else
            {
                using (var implementation = new ServiceImplementation())
                {
                    if (Environment.UserInteractive)
                    {
                        ConsoleHarness.Run(args, implementation);
                    }
                    else
                    {
                        ServiceBase.Run(new WindowsServiceHarness(implementation));
                    }
                }
            }
        }
    }
}