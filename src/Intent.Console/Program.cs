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
    class Program
    {
        static void Main(string[] args)
        {
            // Load the list of available message adapters
            var availableAdapters = IntentMessaging.GetAdapterInfo();

            try
            {
                // Select the adapter mode(s)
                #region Select adapter mode *

                // Ask for the adapter mode
                Console.WriteLine("Add adapters by number and then enter to start: {0}", availableAdapters.Length);

                // For now only support 1-9 as direct key press menu commands
                int length = availableAdapters.Length < 9 ? availableAdapters.Length : 9;
                for (int i = 0; i < length; i++) Console.WriteLine("{0}. {1}", i + 1, availableAdapters[i].Name);

                // Read user selection
                while (true)
                {
                    // Read the keyboard selection
                    var ki = Console.ReadKey();
                    Console.Write(" "); // space aftr typed selection
                    int selection = 0;
                    
                    // Selection: Add new adapter by number
                    if (int.TryParse(ki.KeyChar.ToString(), out selection))
                    {
                        #region Add new adapter from selection

                        // Actual list is 0-based
                        selection--;

                        if (selection >= 0 && selection < availableAdapters.Length)
                        {
                            // Create the new adapter
                            var adapter = IntentMessaging.AddAdapter(availableAdapters[selection].Type);
                            continue;
                        }

                        #endregion Add new adapter from selection
                    }
                    // Selection: Remove last adapter
                    else if (ki.KeyChar == '-')
                    {
                        #region Remove last adapter

                        var adapters = IntentMessaging.ActiveAdapters;

                        if (adapters.Count == 0)
                        {
                            Console.WriteLine(" No adapters to remove!");
                            continue;
                        }

                        // Remove the last adapter in the list
                        var lastAdapter = adapters.Last();
                        IntentMessaging.RemoveAdapter(lastAdapter);
                        continue;

                        #endregion Remove last adapter
                    }

                    // Select another or continue?
                    #region Process continue...

                    // Exit or unavailable reached...
                    if (ki.Key == ConsoleKey.Enter)
                    {
                        if (IntentMessaging.ActiveAdapters.Count == 0)
                        {
                            Console.WriteLine(" Please add add at least one adapter to continue!");
                            continue;
                        }

                        break;
                    }
                    else if (ki.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Aborting...");
                        Environment.Exit(0);
                    }

                    Console.WriteLine(" Unavailable!");

                    #endregion Process continue...
                }

                Console.WriteLine();

                #endregion Select adapter mode *

                // Start adapting...
                #region Start selected adapters =>

                Console.WriteLine("Entering playback mode; starting adapters...");
                Console.WriteLine();
                IntentMessaging.Start();               
                
                #endregion Start selected adapters =>
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }

            Console.WriteLine("=== press enter to quit ===");
            Console.ReadLine();

            // Stop adapters
            IntentMessaging.Stop();
        }
    }
}
