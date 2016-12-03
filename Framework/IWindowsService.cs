using System;
using Bergler.Common.ServiceProvider.Attributes;

namespace Bergler.Common.ServiceProvider.Framework
{
    public interface IWindowsService : IDisposable
    {
        event Delegates.WriteMessage WriteToConsole;
        // This method is called when the service gets a request to start.
        void Start(string[] args);

        // This method is called when the service gets a request to stop.
        void Stop();

        // This method is called when a service gets a request to pause, 
        // but not stop completely.
        void Pause();

        // This method is called when a service gets a request to resume 
        void Continue();

        // This method is called when the machine the service is running on gets a request to shutdown
        void Shutdown();

        // This method is called when the machine the service is running on gets a request for a custom command
        void CustomCommand(int command);
    }
}