using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget {
    public class ArgInterpretor {

        private const string InputFileOptionString = "-i";
        private const string OutputDirOptionString = "-o";
        private const string HelpOptionString = "-h";
        private const string ConcurrencyString = "-c";

        private const int numberOfRequiredArgs = 2;

        public static IDictionary<OptionType,Option> Interpret(IEnumerable<Arg> args, out string errMessage) {
            errMessage = "";
            if(args == null || args.Count() == 0) {
                errMessage = "Insufficient or invalid parameters supplied to extget. -h prints help";
                return null;
            }

            Dictionary<OptionType, Option> optionsMap = new Dictionary<OptionType, Option>();

            foreach(Arg a in args) {
                if(a.Option.ToLower() == InputFileOptionString) {
                    optionsMap.Add(OptionType.InputFile, new Option { Type = OptionType.InputFile, Value = a.Value });
                } else if(a.Option.ToLower() == OutputDirOptionString) {
                    optionsMap.Add(OptionType.OutputDir, new Option { Type = OptionType.InputFile, Value = a.Value });
                } else if(a.Option.ToLower() == HelpOptionString) {
                    optionsMap.Add(OptionType.Help, new Option { Type = OptionType.Help });
                } else if (a.Option.ToLower() == ConcurrencyString) {
                    optionsMap.Add(OptionType.DegreeOfConcurrency, new Option { Type = OptionType.DegreeOfConcurrency, Value = a.Value });
                } else {
                    errMessage = string.Format("Unknown argument {0} passed to extget. -h prints help",a.Option);
                    return null;
                }
            }
            return optionsMap;
        }
    }
}
