using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Worker {
    public static class UriExtensions {

        public static string DeriveFileName(this Uri uri) {
            StringBuilder filepath = new StringBuilder();

            filepath.AppendFormat("{0}_", uri.Scheme);
            
            foreach(string segment in uri.Segments) {
                string safe = getSafeSegment(segment);
                if(safe != null) {
                    filepath.Append(safe);
                }
                filepath.Append("_");
            }
            return filepath.ToString();
        }

        private static string getSafeSegment(string segment) {

            HashSet<char> invalidCharsSet = new HashSet<char>(Path.GetInvalidFileNameChars());
            StringBuilder safe = new StringBuilder(segment);       
            foreach (char c in segment) { 
                if(invalidCharsSet.Contains(c)) {
                    safe.Replace(c, '_');
                }
            }
            return safe.ToString();
        }
    }
 }
