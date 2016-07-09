using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Workbench {
    public class ValidationsException : ApplicationException {
        private IEnumerable<ValidationResult> validationResults;

        public IEnumerable<ValidationResult>  ValidationResults {
            get {
                return validationResults;
            }
        }

        public ValidationsException(IEnumerable<ValidationResult> validationResults) {
            this.validationResults = validationResults;
        }
    }
}
