using System;
using System.IO;
using Extget.Common;

namespace Extget.Worker {
    public class Result {
        public bool Success { get; set; }
        public ErrorCode Code { get; set; }
        public string Message { get; set; }
        
        public static Result Failure(ErrorCode code, string message) {
            return new Result { Success = false, Code = code, Message = message };
        }

        public static Result Ok() {
            return new Result { Success = true };
        }
    }
}