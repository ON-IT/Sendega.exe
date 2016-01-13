using System;
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
            var destination = "";
            var pricegroup = 0;
            var contentTypeID = 1;
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

            var options = new OptionSet
            {
                {"u|username=", "Sendega username", s => username = s},
                {"p|password=", "Sendega password", s => password = s},
                {"s|sender=", "Sender", s => sender = s},
                {"d|destination=", "Destination", s => destination = s},
                {"m|message=", "Message", s => content = s},
                {"h|help", "Help", v => showHelp = v != null}
            };

            try
            {
                options.Parse(args);
                if (showHelp)
                {
                    WriteUsage();
                    return;
                }
                if (string.IsNullOrEmpty(username))
                {
                    Log.Fatal("Username is missing. Specify with -u.");
                    Environment.Exit(1);
                }

                if (string.IsNullOrEmpty(password))
                {
                    Log.Fatal("Password is missing. Specify with -p.");
                    Environment.Exit(1);
                }

                if (string.IsNullOrEmpty(content))
                {
                    Log.Fatal("Message is missing. Specify with -m.");
                    Environment.Exit(1);
                }

                if (string.IsNullOrEmpty(destination))
                {
                    Log.Fatal("Destination is missing. Specify with -d.");
                    Environment.Exit(1);
                }

                if (string.IsNullOrEmpty(sender))
                {
                    Log.Fatal("Sender is missing. Specify with -s.");
                    Environment.Exit(1);
                }

                var client = new ContentSoapClient(new BasicHttpsBinding(),
                    new EndpointAddress("https://smsc.sendega.com/Content.asmx?wsdl"));
                var response = client.Send(username, password, sender, destination, pricegroup, contentTypeID,
                    contentHeader, content, dlrUrl, ageLimit, extID, sendDate, refID, priority, gwID, pid, dcs);
                if (response.Success)
                {
                    Log.Information("SMS sent to {destination:l} from {sender:l}. Message: {content}", destination,
                        sender, content);
                    Environment.Exit(0);
                }
                else
                {
                    Log.Fatal("ERROR SENDING SMS TO {destination:l}. {ErrorMessage:l}.", destination,
                        response.ErrorMessage);
                    Environment.Exit(1);
                }
            }
            catch (OptionException e)
            {
                Log.Error(e, e.Message);
                Log.Information("Try `--help' for more information.");
                Environment.Exit(1);
            }
        }

        private static void WriteUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage example: ");
            Console.WriteLine("Sendega.exe -u SID -p password -s sender -d destination -m message");
            Console.WriteLine();
            Console.WriteLine("Go to http://www.sendega.com to create an account.");
            Console.WriteLine("Created by ON IT AS <post@on-it.no> 2016.");
            Console.WriteLine();
        }
    }
}