using Extget.Common;
using Extget.HandlerRepo;
using Extget.Worker;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extget.Workbench {
    public class Bench {

        private Scheduler scheduler;
        private StartupParams p;
        private CountdownEvent countdown;
       
        public Bench(StartupParams p) {
            var errors = p.Validate(new ValidationContext(p));
            if(errors.Count() > 0 ) {
                throw new ValidationsException(errors);
            }
            this.p = p;           
        }

        public void Run() {
            setupProtocolHandlers(p.PluginsPath);
            scheduler = new Scheduler(this.p.DegreeOfConcurrency, interceptor, this.p.OutputPath);
            scheduler.Start();
        }

        public void interceptor(SchedulerEvent evt) {
            if(evt.Type == EventType.Completed || evt.Type == EventType.Failed) {
                countdown.Signal();
            }
            this.p.EvtHandler(evt);
        }

        public void Enqueue(List<Request> requests) {           
            foreach (Request r in requests) {
                scheduler.Enqueue(r);
            }
            countdown = new CountdownEvent(requests.Count);
            scheduler.EndOfEnqueing();
        }

        public void WaitUntilAllDone() {
            countdown.Wait();
        }
        
        public void Stop() {
            scheduler.Stop();
        }

        private void setupProtocolHandlers(string pluginsPath) {
            // read the directory for dlls 
            foreach(string file in Directory.EnumerateFiles(pluginsPath, "*.dll", SearchOption.TopDirectoryOnly)) {
                IHandler handler = ProtocolHandlerFactory.GetHandler(file);
                if(handler == null) {
                    // could not load handler from this assembly, ignore
                    continue;
                }
                foreach (string scheme in handler.Schemes) {
                    HandlerRepository.Instance.SetHandler(scheme, handler);
                }
            }
        }
    }
}
