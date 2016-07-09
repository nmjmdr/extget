using Extget.Common;
using Extget.Workbench;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget {
    class Program {

        private const int DefaultConcurrency = 20;
        private const string PluginsDirConfigKey = "plugins-dir";

        static void Main(string[] args) {
            verifyConfiguration();
            string err;
            IDictionary<OptionType, Option> options;
            options = parseCommandLineArguments(args, out err);


            if (options.ContainsKey(OptionType.Help)) {
                printHelp();
                return;
            }

            if (!options.ContainsKey(OptionType.InputFile) || !options.ContainsKey(OptionType.OutputDir)) {
                Console.WriteLine("Invalid arguments passed. -h prints help");
                return;
            }

            if (!options.ContainsKey(OptionType.DegreeOfConcurrency)) {
                Console.WriteLine(string.Format("Warning! - concurrency value not specified, assuming default of {0} (Extget will download {0} files concurrently)", DefaultConcurrency));
            }


            IEnumerable<ValidationResult> errors;
            StartupParams p = buildStartupParams(options, out errors);

            if (p == null) {
                printValidationErrors(errors);
                return;
            }

            runWorkBench(options, p);
        }

        private static void runWorkBench(IDictionary<OptionType, Option> options, StartupParams p) {
            string error;
            List<Uri> uris = parseInputFile(options[OptionType.InputFile], out error);

            if(uris == null) {
                Console.WriteLine(error);
                Environment.Exit(-1);
            }

            try {
                Bench bench = new Bench(p);
                bench.Run();

                EnqueueRequests(uris, bench);
            } catch (ValidationsException exp) {
                printValidationErrors(exp.ValidationResults);
            } catch (Exception exp) {
                Console.WriteLine("An error occured, cannot recover, exiting");
                Console.WriteLine("Error: " + exp.Message);
            }
        }

        private static IDictionary<OptionType,Option> parseCommandLineArguments(string[] args, out string err) {
            HashSet<string> noValueOptions = new HashSet<string>();
            noValueOptions.Add("-h");

            IEnumerable<Arg> arguments = new CommandLineParser(noValueOptions).Parse(args);
            IDictionary<OptionType,Option> options = ArgInterpretor.Interpret(arguments, out err);

            if(options == null) {
                Console.WriteLine(err);
                Environment.Exit(-1);
            }

            return options;
        }

        private static void verifyConfiguration() {
            bool validConfiguration = checkConfiguration();

            if (!validConfiguration) {
                Console.WriteLine("Invalid configuration settings, please check if plugins-dir value is set to a valid directory");
                Environment.Exit(-1);
            }
        }

        private static void EnqueueRequests(List<Uri> uris, Bench bench) {
            foreach(Uri uri in uris) {
                bench.Enqueue(new Request(uri));
            }
        }

        private static List<Uri> parseInputFile(Option inputFileOption, out string error) {
            error = "";
            if(!File.Exists(inputFileOption.Value)) {
                error = string.Format("Input file: {0} does not exist", inputFileOption.Value);
                return null;
            }
            List<Uri> urls = new List<Uri>();
            StreamReader reader = new StreamReader(inputFileOption.Value);
            while (!reader.EndOfStream) {
                string line = reader.ReadLine();
                line.Trim();
                try {
                    Uri uri = new Uri(line);
                    urls.Add(uri);
                } catch (UriFormatException) {
                    Console.WriteLine("Malformed uri - {0}, ingorning it", line);
                    continue;
                }
            }

            if(urls.Count == 0) {
                error = string.Format("No valid Uris found in input file: {0}", inputFileOption.Value);
                return null;
            }

            return urls;
        }

        private static void printValidationErrors(IEnumerable<ValidationResult> errors) {
            foreach(ValidationResult r in errors) {
                Console.WriteLine(r.ErrorMessage);
            }
        }

        private static bool checkConfiguration() {
            string pluginsFolder = ConfigurationManager.AppSettings[PluginsDirConfigKey];
            if(string.IsNullOrWhiteSpace(pluginsFolder) || !Directory.Exists(pluginsFolder)) {
                return false;
            }
            return true;
        }

        private static StartupParams buildStartupParams(IDictionary<OptionType, Option> options,out IEnumerable<ValidationResult> errors) {

            errors = null;
            int conncurrency = DefaultConcurrency;

            if(options.ContainsKey(OptionType.DegreeOfConcurrency)) {
                int result;
                if(int.TryParse(options[OptionType.DegreeOfConcurrency].Value,out result)) {
                    conncurrency = result; 
                }
            }

            StartupParams p = new StartupParams{ DegreeOfConcurrency = conncurrency,
                                                   EvtHandler = downloadEvtHandler,
                                                   OutputPath = options[OptionType.OutputDir].Value,
                                                   PluginsPath = ConfigurationManager.AppSettings[PluginsDirConfigKey]
                                               };

            errors = p.Validate(new ValidationContext(p));
            if(errors.Count() > 0) {
                return null;
            }

            return p;
        }

        private static void downloadEvtHandler(SchedulerEvent evt) {
            if(evt.Type == EventType.Failed) {
                displayFailure(evt);
            } else if(evt.Type == EventType.Completed) {
                displayCompleted(evt);
            } else if(evt.Type == EventType.Started) {
                displayStarted(evt);
            }
        }

        private static void displayStarted(SchedulerEvent evt) {
            Console.WriteLine("Downloading - {0}", evt.Result.Uri);
        }

        private static void displayCompleted(SchedulerEvent evt) {
            Console.WriteLine("Downloaded - {0}", evt.Result.Uri);
        }

        private static void displayFailure(SchedulerEvent evt) {           
            Console.WriteLine("Failed to download - {0}", evt.Result.Uri);
            Console.WriteLine("Error: {0}", evt.Result.Message);           
        }

        private static void printHelp() {
            Console.WriteLine("Help:");
        }
    }
}
