using System.ServiceProcess;
using Bergler.Common.ServiceProvider.Attributes;
using Bergler.Common.ServiceProvider.Framework;

namespace Bergler.Common.ServiceProvider
{
    [WindowsServiceConfiguration("$safeprojectname$", DisplayName = "$projectname$", Description = "The description of the $projectname$ service.", EventLogSource = "$projectname$", StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
        public void Dispose()
        {
        }

        public event Delegates.WriteMessage WriteToConsole;

        public void Start(string[] args)
        {
            WriteToConsole?.Invoke("Service has started");
        }

        public void Stop()
        {
            WriteToConsole?.Invoke("Service has stopped");
        }

        public void Pause()
        {
            WriteToConsole?.Invoke("Service has paused");
        }

        public void Continue()
        {
            WriteToConsole?.Invoke("Service has continued");
        }

        public void Shutdown()
        {
            WriteToConsole?.Invoke("Service has been shutdown");
        }

        public void CustomCommand(int command)
        {
            WriteToConsole?.Invoke("Service has gotten a custom command");
        }
    }
}