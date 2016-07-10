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

                Option optionToAdd = getOption(a);
                if(optionToAdd == null) {
                    errMessage = string.Format("Unknown argument {0} passed to extget. -h prints help", a.OptionText);
                    return null;
                }

                if (!optionsMap.ContainsKey(optionToAdd.Type)) {
                    optionsMap.Add(optionToAdd.Type, optionToAdd);
                } else {
                    errMessage = string.Format("Options repeated, check the arguments. -h prints help", a.OptionText);
                    return null;
                }
            }
            return optionsMap;
        }

        private static Option getOption(Arg a) {

            if (a.OptionText.ToLower() == InputFileOptionString) {                
                    return new Option { Type = OptionType.InputFile, Value = a.Value };             
            } else if (a.OptionText.ToLower() == OutputDirOptionString) {
                return new Option { Type = OptionType.OutputDir, Value = a.Value };
            } else if (a.OptionText.ToLower() == HelpOptionString) {
                return new Option { Type = OptionType.Help };
            } else if (a.OptionText.ToLower() == ConcurrencyString) {
                return new Option { Type = OptionType.DegreeOfConcurrency, Value = a.Value };
            } else {                
                return null;
            }
        }
    }
}
