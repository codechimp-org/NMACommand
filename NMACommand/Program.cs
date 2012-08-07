using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMALib;

namespace NMACommand
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                // Command line parsing
                Arguments CommandLine = new Arguments(args, false);


                // If no arguments then display help
                if (args.Length == 0 | CommandLine["?"] != null | CommandLine["help"] != null)
                {
                    Console.WriteLine("Notify My Android Command Line Utility.\n");
                    
                    Console.WriteLine("NMACommand [/A:application /K:key] [/P:priority] /E:event /D:description\n");
                    
                    Console.WriteLine("/A:application       If specified /K must also be included.");
                    Console.WriteLine("                     Specifies the application name to send.");
                    Console.WriteLine("                     If omitted the value in the config file will be used.");
                    Console.WriteLine("/K:apikey            If specified /A must also be included.");
                    Console.WriteLine("                     Specifies the API Key to send the notification.");
                    Console.WriteLine("                     If omitted the value in the config file will be used.");
                    Console.WriteLine("/P:priority          Specifies the priority.");
                    Console.WriteLine("                     Emergency, High, Moderate, Normal, VeryLow.");
                    Console.WriteLine("                     If omitted Normal will be used.");
                    Console.WriteLine("/E:\"event\"           [Mandatory] Specifies the event name to send.");
                    Console.WriteLine("/D:\"description\"     [Mandatory] Specifies the description to send.");

                    Environment.Exit(0);
                }

                if (CommandLine["D"] == null | CommandLine["E"] == null)
                {
                    Console.WriteLine("Invalid command line arguments.\n/D and /E must be specified.\nUse /? to display help.");
                    Environment.Exit(1);
                }

                if (CommandLine["D"].Trim().Length == 0 | CommandLine["E"].Trim().Length == 0)
                {
                    Console.WriteLine("Invalid command line arguments.\n/D and /E must be specified and include a value.\nUse /? to display help.");
                    Environment.Exit(1);
                }

                // Create a client/notification.
                NMAClient Client = new NMAClient();

                if (CommandLine["A"] != null & CommandLine["K"] != null)
                {
                    NMAClientConfiguration clientConfig = new NMAClientConfiguration();
                    clientConfig.ApplicationName = CommandLine["A"];
                    clientConfig.ApiKeychain = CommandLine["K"];
                    Client = new NMAClient(clientConfig);
                }
                else if ((CommandLine["A"] != null & CommandLine["K"] == null) | (CommandLine["A"] == null & CommandLine["K"] != null))
                {
                    Console.WriteLine("Invalid command line arguments.\nIf using either /A or /K both switches must be specified.\nUse /? to display help.");
                    Environment.Exit(1);
                }

                // Create the notification command
                NMANotification notification =
                    new NMANotification
                    {
                        Description = CommandLine["D"],
                        Event = CommandLine["E"],
                        Priority = NMANotificationPriority.Normal
                    };

                // If a priority is set override the default above
                if (CommandLine["P"] != null)
                {
                    switch (CommandLine["P"].ToUpper())
                    {
                        case "EMERGENCY":
                        case "E":
                            notification.Priority = NMANotificationPriority.Emergency;
                            break;
                        case "HIGH":
                        case "H":
                            notification.Priority = NMANotificationPriority.High;
                            break;
                        case "MODERATE":
                        case "M":
                            notification.Priority = NMANotificationPriority.Moderate;
                            break;
                        case "NORMAL":
                        case "N":
                            notification.Priority = NMANotificationPriority.Normal;
                            break;
                        case "VERYLOW":
                        case "L":
                            notification.Priority = NMANotificationPriority.VeryLow;
                            break;
                        default:
                            notification.Priority = NMANotificationPriority.Normal;
                            break;
                    }
                }


                // Post the notification.
                Client.PostNotification(notification);
                Console.WriteLine ("Notification Sent");
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("An error occured {0}.", e.Message));
                Environment.Exit(2);
            }

            // Clean exit return code
            Environment.Exit(0);
        }
    }
}
