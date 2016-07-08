using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extget.Workbench
{
    public class Scheduler
    {
        private const int DegreeOfConcurrency = 4;
        private SemaphoreSlim semaphores;
        private BlockingCollection<Request> blockingQueue;

        public Scheduler() {
        }

        public void Enqueue(Uri uri) {
        }

        public void Start() {
        }

        public void Stop() {
        }
    }
}
