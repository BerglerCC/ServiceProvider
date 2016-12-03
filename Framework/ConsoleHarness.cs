using System;
using System.Runtime.InteropServices;

namespace Bergler.Common.ServiceProvider.Framework
{
    public static class ConsoleHarness
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();


        public static void Run(string[] args, IWindowsService service)
        {
            var serviceName = service.GetType().Name;
            service.WriteToConsole += OnWriteToConsole;
            var isRunning = true;
            AllocConsole();

            service.Start(args);

            while (isRunning)
            {
                WriteToConsole(ConsoleColor.Yellow, "Enter either [Q]uit, [P]ause, [R]esume : ");
                isRunning = HandleConsoleInput(service, Console.ReadLine());
            }

            service.Stop();
            service.Shutdown();
        }

        private static void OnWriteToConsole(string message)
        {
            WriteToConsole(ConsoleColor.DarkGreen, message);
        }

        private static bool HandleConsoleInput(IWindowsService service, string line)
        {
            var canContinue = true;
            if (line != null)
            {
                switch (line.ToUpper())
                {
                    case "Q":
                        canContinue = false;
                        break;
                    case "P":
                        service.Pause();
                        break;
                    case "R":
                        service.Continue();
                        break;
                    default:
                        WriteToConsole(ConsoleColor.Red, "Did not understand that input, try again.");
                        break;
                }
            }

            return canContinue;
        }

        public static void WriteToConsole(ConsoleColor foregroundColor, string format, params object[] formatArguments)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = foregroundColor;

            Console.WriteLine(format, formatArguments);
            Console.Out.Flush();

            Console.ForegroundColor = originalColor;
        }
    }
}