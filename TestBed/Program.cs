using Extget.Common;
using Extget.Workbench;
using Extget.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBed {
    public class Program {
        static void Main(string[] args) {

            //IHandler httpHandler = new HttpHandler();

            //FileGetter getter = new FileGetter(httpHandler);

            //Task<Result> t = getter.GetAsync(new Request { Uri = new Uri(@"http://localhost/File/t.txt") });

            //Console.WriteLine("Downloading...");

            //t.ContinueWith((Task<Result> r) => {

            //   if(r.IsCompleted) {
            //        Console.WriteLine(r.Result.Success);
            //    }
            //});

            //Console.WriteLine("Here");


            //t.Wait();

            Scheduler scheduler = new Scheduler(3,resultCallback);

            scheduler.Start();

            for (int i = 0; i < 1; i++) {
                scheduler.Enqueue(new Request(new Uri(@"http://localhost/File/t1.txt")));              
            }

            Console.ReadLine();
        }

        private static void resultCallback(Result result) {
            Console.WriteLine("{0} {1} {2}", result.Uri, result.Success, result.Message);
        }
    }
}
