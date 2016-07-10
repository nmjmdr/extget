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
        private string outputDir;

        public FileGetter(IHandler handler,string outputDir) {
            this.handler = handler;
            this.outputDir = outputDir;
        }

        public async Task<Result> GetAsync(Request request) {

            // Need a way to cancel the download!!!!
            FileStream fs = null;
            string filepath = Path.Combine(outputDir,request.Uri.DeriveFileName());
            try {
                Response response = await handler.GetAsync(request);

                if (!response.Result.IsSuccess) {
                    return response.Result;
                }
                
                if(File.Exists(filepath)) {
                    return Result.Failure(request.Uri.AbsoluteUri, ErrorCode.FailedToGet, string.Format("Target file {0} already exists, not dowloanding",filepath));
                }

                fs = File.Open(filepath, FileMode.Create);
                await response.OutStream.CopyToAsync(fs);
                await fs.FlushAsync();
                fs.Close();
                response.OutStream.Close();

            } catch(Exception exp) {
                if(fs != null) {
                    // delete the file
                    fs.Close();
                    File.Delete(filepath);
                }
                return Result.Failure(request.Uri.AbsoluteUri,ErrorCode.FailedToGet,string.Format("An exception occured during the download: {0}",exp.Message));
            } 
            return Result.Ok(request.Uri.AbsoluteUri);
        }

        
    }
}
