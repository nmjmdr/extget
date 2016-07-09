using Extget.Common;
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

        public async Task<Result> GetAsync(Request request) {

            // Need a way to cancel the download!!!!
            FileStream fs = null;
            string filename = request.Uri.DeriveFileName();
            try {
                Response response = await handler.GetAsync(request);

                if (!response.Result.Success) {
                    return response.Result;
                }
                fs = File.Open(filename, FileMode.Create);
                await response.OutStream.CopyToAsync(fs);
                await fs.FlushAsync();
                fs.Close();

            } catch(Exception exp) {
                if(fs != null) {
                    // delete the file
                    fs.Close();
                    File.Delete(filename);
                }
                return Result.Failure(request.Uri.AbsoluteUri,ErrorCode.FailedToGet,string.Format("An exception occured during the download: {0}",exp.Message));
            } 
            return Result.Ok(request.Uri.AbsoluteUri);
        }
    }
}
