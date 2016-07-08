using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Worker {
    public class FileGetter {

        private IHandler handler;

        public FileGetter(IHandler handler) {
            this.handler = handler;
        }

        public async Task<Result> GetAsync(Uri uri) {

            // Need a way to cancel the download!!!!
            Response response =  await handler.GetAsync(uri);

            if(!response.Result.Success) {
                return response.Result;
            }

            string filename = uri.DeriveFileName();

            FileStream fs = File.Open(filename, FileMode.Create);           
            await response.OutStream.CopyToAsync(fs);

            return Result.Ok();
        }
    }
}
