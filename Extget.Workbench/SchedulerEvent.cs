using Extget.Common;
using System;

namespace Extget.Workbench {
    public class SchedulerEvent {
        public Uri Uri { get; set; }
        public EventType Type { get; set; }
        public Result Result { get; set; }
    }
}