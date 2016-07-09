using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Common {
    public class Response {
        public Result Result { get; set; }
        public Stream OutStream { get; set; }

        public static Response Failure(string uri,ErrorCode code, string message) {
            return new Response { Result = Result.Failure(uri,code, message) };
        }

        public static Response Ok(string uri,Stream stream) {
            return new Response { Result = Result.Ok(uri), OutStream = stream };
        }
    }
}
