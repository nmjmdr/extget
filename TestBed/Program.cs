using Extget.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBed {
    public class Program {
        static void Main(string[] args) {

            IHandler httpHandler = new HttpHandler();

            FileGetter getter = new FileGetter(httpHandler);

            Task<Result> t = getter.GetAsync(new Uri(@"http://localhost/File/t.txt"));

            Console.WriteLine("Downloading...");

            t.ContinueWith((Task<Result> r) => {

               if(r.IsCompleted) {
                    Console.WriteLine(r.Result.Success);
                }
            });

            Console.WriteLine("Here");
                        

            t.Wait();
        }
    }
}
