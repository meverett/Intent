using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Intent.Dmx;
using Intent.Midi;
using Intent.Osc;

namespace Intent
{
    /// <summary>
    /// Basic console application to run a command line Intent instance.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            #region Load Scripts

            try
            {
                IntentRuntime.LoadAllScripts("Scripts");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.ReadLine();
                return;
            }

            #endregion Load Scripts

            #region Print Controls

            Console.WriteLine();
            Console.WriteLine("CONTROLS =>");
            Console.WriteLine("space:   toggle start/stop");
            Console.WriteLine("r:       reload scripts");
            Console.WriteLine("enter:   exit");
            Console.WriteLine();

            #endregion Print Controls

            #region Get User Input

            ConsoleKey key = ConsoleKey.NoName;

            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Spacebar:
                        try
                        {
                            if (IntentRuntime.IsRunning) IntentRuntime.Stop();
                            else IntentRuntime.Start();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            IntentRuntime.Stop();
                        }
                        break;

                    case ConsoleKey.R:
                        try
                        {
                            var isRunning = IntentRuntime.IsRunning;
                            IntentRuntime.ClearAdapters();
                            IntentRuntime.LoadAllScripts("Scripts");
                            if (isRunning) IntentRuntime.Start();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            IntentRuntime.Stop();
                        }
                        break;
                }
            }

            #endregion Get User Input

            // Stop adapters
            IntentRuntime.Stop();
        }
    }
}
