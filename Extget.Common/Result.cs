using System;
using System.IO;

namespace Extget.Common {
    public class Result {
        public string Uri { get; set; }
        public bool Success { get; set; }
        public ErrorCode Code { get; set; }
        public string Message { get; set; }
        
        public static Result Failure(string uri,ErrorCode code, string message) {
            return new Result { Uri = uri, Success = false, Code = code, Message = message };
        }

        public static Result Ok(string uri) {
            return new Result { Uri = uri, Success = true };
        }
    }
}