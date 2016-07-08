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

        public static Response Failure(ErrorCode code, string message) {
            return new Response { Result = Result.Failure(code, message) };
        }

        public static Response Ok(Stream stream) {
            return new Response { Result = Result.Ok(), OutStream = stream };
        }
    }
}
