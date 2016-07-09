using Extget.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extget.HandlerRepo
{
    public class HandlerRepository
    {
        // static ensures single instance, no need for a lock
        private static HandlerRepository instance = new HandlerRepository();

        private Dictionary<string, IHandler> repo;
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        public static HandlerRepository Instance {
            get {
                return instance;
            }
        }

        private HandlerRepository() {
            repo = new Dictionary<string, IHandler>();
        }

        public void SetHandler(string protocol,IHandler handler) {
            try {
                rwLock.EnterWriteLock();
                if (!repo.ContainsKey(protocol)) {
                    repo.Add(protocol, handler);
                    return;
                }
                repo[protocol] = handler;
            } finally {
                rwLock.ExitWriteLock();
            }
        } 
        
        public IHandler Get(string protocol) {
            try {
                rwLock.EnterReadLock();
                if (!repo.ContainsKey(protocol)) {
                    return null;
                }
                return repo[protocol];
            } finally {
                rwLock.ExitReadLock();
            }
        }   

    }
}
