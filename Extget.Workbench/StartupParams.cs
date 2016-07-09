using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Workbench {
    public class StartupParams : IValidatableObject {
          
        [Required]   
        public string PluginsPath { get; set; }
        [Required]       
        public int DegreeOfConcurrency { get; set; }    
        [Required]
        public string OutputPath { get; set; }   
        public OnDownloadEvent EvtHandler { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            
            if(!Directory.Exists(PluginsPath)) {                
                yield return new ValidationResult(string.Format("Plugins-dir  {0} is invalid, should point to a valid directory",PluginsPath));
            }

            if (!Directory.Exists(OutputPath)) {
                yield return new ValidationResult("output-path is invalid, should point to a valid directory");
            }

            if (DegreeOfConcurrency <= 0) {
                yield return new ValidationResult("Degree of concurrency should be greater than 0");
            }
        }
    }
}
