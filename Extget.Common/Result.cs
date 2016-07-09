using System;
using System.IO;

namespace Extget.Common {
    public class Result {
        public string Uri { get; set; }
        public bool IsSuccess { get; set; }
        public ErrorCode Code { get; set; }
        public string Message { get; set; }
        
        public static Result Failure(string uri,ErrorCode code, string message) {
            return new Result { Uri = uri, IsSuccess = false, Code = code, Message = message };
        }

        public static Result Ok(string uri) {
            return new Result { Uri = uri, IsSuccess = true };
        }
    }
}