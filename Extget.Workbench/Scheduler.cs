using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Extget.Common;
using Extget.Worker;
using Extget.HandlerRepo;

namespace Extget.Workbench
{
    public delegate void OnDownloadEvent(SchedulerEvent evt);

    public class Scheduler
    {
        private int degreeOfConcurrency = 3;       
        private BlockingCollection<Request> blockingQueue;
        private SemaphoreSlim semaphore;
        private CancellationTokenSource cancelTokenSource;               
        public event OnDownloadEvent evt;
        private string outputDir;
       

        public Scheduler(int degreeOfConcurrency,OnDownloadEvent eventHandler,string outputDir) {
            cancelTokenSource = new CancellationTokenSource();
            blockingQueue = new BlockingCollection<Request>(new ConcurrentQueue<Request>());
            this.degreeOfConcurrency = degreeOfConcurrency;
            semaphore = new SemaphoreSlim(this.degreeOfConcurrency);
            this.outputDir = outputDir;
                   
            evt += eventHandler;
        }

        public void EndOfEnqueing() {
            blockingQueue.CompleteAdding();
        }

        public void Enqueue(Request request) {
            blockingQueue.Add(request);
        }

     

        public void Start() {

            Task.Run(() => {
                foreach (Request request in blockingQueue.GetConsumingEnumerable(cancelTokenSource.Token)) {

                    semaphore.Wait(cancelTokenSource.Token);
                    // raise started event
                    evt?.Invoke(new SchedulerEvent { Uri = request.Uri, Type = EventType.Started });

                    IHandler handler = HandlerRepository.Instance.Get(request.Uri.Scheme);
                    if (handler == null) {
                        Result r = Result.Failure(request.Uri.AbsoluteUri, ErrorCode.HandlerNotFound, string.Format("Could not find handler for scheme {0} in plugins", request.Uri.Scheme));
                        evt?.Invoke(new SchedulerEvent { Uri = request.Uri, Result = r, Type = EventType.Failed });
                        return;
                    }

                    FileGetter fileGetter = new FileGetter(handler, this.outputDir);

                    fileGetter.GetAsync(request).ContinueWith((t) => {
                        semaphore.Release();
                        if (t.Result.IsSuccess) {
                            evt?.Invoke(new SchedulerEvent { Uri = request.Uri, Result = t.Result, Type = EventType.Completed });
                        } else {
                            evt?.Invoke(new SchedulerEvent { Uri = request.Uri, Result = t.Result, Type = EventType.Failed });
                        }
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
