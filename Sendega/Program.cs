using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using NDesk.Options;
using Sendega.com.sendega.smsc;
using Serilog;

namespace Sendega
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(@"log\sendega-{Date}.log")
                .MinimumLevel.Debug()
                .CreateLogger();

            var username = "";
            var password = "";
            var sender = "";
            var pricegroup = 0;
            var contentTypeId = 1;
            var contentHeader = "";
            var content = "";
            var dlrUrl = "";
            var ageLimit = 0;
            var extID = "";
            var sendDate = "";
            var refID = "";
            var priority = 0;
            var gwID = 0;
            var pid = 0;
            var dcs = 0;

            var showHelp = false;

            var destinations = new List<string>();

            var options = new OptionSet
            {
                {"u|username=", "Sendega username", s => username = s},
                {"p|password=", "Sendega password", s => password = s},
                {"s|sender=", "Sender", s => sender = s},
                {"d|destination=", "Destination", s => destinations.AddRange(s.Split(' ', ',', ';')) },
                {"m|message=", "Message", s => content = s},
                {"pricegroup=", "Price group", (int s) => pricegroup = s},
                {"contenttype=", "Content type ID", (int i) => contentTypeId = i},
                {"contentheader=", "Content header", s => contentHeader = s},
                {"deliveryurl=", "Delivery URL", s => dlrUrl = s},
                {"agelimit", "Age limit", (int i) => ageLimit = i},
                {"senddate", "Send date", s => sendDate = s},
                {"priority", "Priority", (int i) => priority = i},
                {"extid", s => extID = s},
                {"refid", s => refID = s},
                {"gwid", (int i) => gwID = i},
                {"pid", (int i) => pid = i},
                {"dcs", (int i) => dcs = i},
                {"h|help", "Help", v => showHelp = v != null}
            };

            try
            {
                options.Parse(args);
                if (showHelp)
                {
                    WriteUsage(options);
                    return;
                }
                if (string.IsNullOrEmpty(username))
                {
                    Log.Fatal("Username is missing. Specify with -u.");
                    WriteUsage(options);
                    Environment.Exit(1);
                }

                if (string.IsNullOrEmpty(password))
                {
                    Log.Fatal("Password is missing. Specify with -p.");
                    WriteUsage(options);
                    Environment.Exit(1);
                }

                if (string.IsNullOrEmpty(content))
                {
                    Log.Fatal("Message is missing. Specify with -m.");
                    WriteUsage(options);
                    Environment.Exit(1);
                }

                if (destinations.Count == 0)
                {
                    Log.Fatal("Destination is missing. Specify with -d.");
                    WriteUsage(options);
                    Environment.Exit(1);
                }

                if (string.IsNullOrEmpty(sender))
                {
                    Log.Fatal("Sender is missing. Specify with -s.");
                    WriteUsage(options);
                    Environment.Exit(1);
                }

                var client = new ContentSoapClient(new BasicHttpsBinding(),
                    new EndpointAddress("https://smsc.sendega.com/Content.asmx?wsdl"));
                var exitCode = 0;
                foreach (var destination in destinations)
                {
                    var response = client.Send(username, password, sender, destination, pricegroup, contentTypeId,
                    contentHeader, content, dlrUrl, ageLimit, extID, sendDate, refID, priority, gwID, pid, dcs);
                    if (response.Success)
                    {
                        Log.Information("SMS sent to {destination:l} from {sender:l}. Message: {content}", destination,
                            sender, content);
                    }
                    else
                    {
                        Log.Fatal("ERROR SENDING SMS TO {destination:l}. {ErrorMessage:l}.", destination, response.ErrorMessage);
                        exitCode = 1;
                    }
                }
                Environment.Exit(exitCode);
                
            }
            catch (OptionException e)
            {
                Log.Error(e, e.Message);
                Log.Information("Try `--help' for more information.");
                Environment.Exit(1);
            }
        }

        private static void WriteUsage(OptionSet options)
        {
            var filename = GetExecutableName();
            Console.WriteLine();
            Console.WriteLine($"Usage: {filename} -u SID -p password -s sender -d destination -m message");
            Console.WriteLine();
            Console.WriteLine("In order to send the same SMS to multiple destinations you can either:");
            Console.WriteLine("1) Specify the destinations separated by , or ;");
            Console.WriteLine("2) Specify multiple destinations by appending -d DESTINATION multiple times");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("Go to http://www.sendega.com to create an account.");
            Console.WriteLine("Created by ON IT AS <post@on-it.no> 2016.");
            Console.WriteLine();
        }

        private static string GetExecutableName()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            return Path.GetFileName(codeBase);
        }
    }
}