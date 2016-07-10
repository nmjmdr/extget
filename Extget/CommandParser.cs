using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget {
    public class CommandLineParser {

        private delegate state state(string input);

        private state startstate;
        private state optionstate;
        private state valuestate;        
        private state final_state;
        private List<Arg> args = new List<Arg>();
        private HashSet<string> noValueOptions;

        public CommandLineParser(HashSet<string> noValueOptions) {
            this.noValueOptions = noValueOptions;
            setupStates();
        }
       
        private void setupStates() {

            final_state = (s) => {
                return null;
            };

            startstate = (s) => {
                if(isOption(s)) {
                    args.Add(new Arg { OptionText = s });
                    if (noValueOptions.Contains(s)) {
                        args[args.Count - 1].IsNoValueOption = true;
                        return valuestate;
                    } else {
                        return optionstate;
                    }
                } else {
                    return null;
                }
            };

            optionstate = (s) => {
                if (isOption(s)) {
                    return null;
                } else if(string.IsNullOrWhiteSpace(s)) {
                    return null;
                } else {
                    if(args.Count  == 0) {
                        return null;
                    }

                    args[args.Count - 1].Value = s;
                    return valuestate;
                }
            };

            valuestate = (s) => {
                if (isOption(s)) {
                    args.Add(new Arg { OptionText = s });
                    if (noValueOptions.Contains(s)) {
                        args[args.Count - 1].IsNoValueOption = true;
                        return valuestate;
                    } else {
                        return optionstate;
                    }
                } else if (s == "EOF") {
                    return final_state;
                } else {
                    return null;
                }
            };
        }

        private bool isOption(string s) {
           if(string.IsNullOrWhiteSpace(s) || s[0] != '-') {
                return false;
            }
            return true;
        }

        public IEnumerable<Arg> Parse(string[] arguments) {

            state st = startstate;
            foreach(string arg in arguments) {
                st = st(arg);
                if(st == null) {
                    return null;
                }
            }
            st = st("EOF");

            if(st != final_state) {
                return null;
            }

            return args;
        }
    }
}
