using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Common {
    public class Request {
        public Uri Uri { get; set; }

        public Request(Uri uri) {
            Uri = uri;
        }
    }
}
