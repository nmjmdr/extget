using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Extget.Common;
using Extget.Worker;

namespace Extget.Workbench
{
    public class Scheduler
    {
        private int degreeOfConcurrency = 3;       
        private BlockingCollection<Request> blockingQueue;
        private SemaphoreSlim semaphore;
        private CancellationTokenSource cancelTokenSource;
        private FileGetter fileGetter;

        public delegate void OnResult(Result result);
        public event OnResult onDownloadedEvent;

        public Scheduler(int degreeOfConcurrency,OnResult resultCallback) {
            cancelTokenSource = new CancellationTokenSource();
            blockingQueue = new BlockingCollection<Request>(new ConcurrentQueue<Request>());
            this.degreeOfConcurrency = degreeOfConcurrency;
            semaphore = new SemaphoreSlim(this.degreeOfConcurrency);
            fileGetter = new FileGetter(new TestHttpHandler());
            onDownloadedEvent += resultCallback;

        }

        public void Enqueue(Request request) {
            blockingQueue.Add(request);
        }

        public void Start() {

            Task.Run(() => {                
                foreach (Request request in blockingQueue.GetConsumingEnumerable(cancelTokenSource.Token)) {

                    semaphore.Wait(cancelTokenSource.Token);

                    fileGetter.GetAsync(request).ContinueWith((t) => {                        
                        semaphore.Release();
                        onDownloadedEvent?.Invoke(t.Result);
                    });
                }
            });            
        }

        public void Stop() {
            blockingQueue.CompleteAdding();
            cancelTokenSource.Cancel();
        }
    }
}
