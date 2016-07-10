using Extget.Common;
using Extget.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHandlerPlugin
{
    public class FileHandler : IHandler {
        public string[] Schemes {
            get {
                return new string[] { "file" };
            }
        }

        public async Task<Response> GetAsync(Request request) {
            
            try {
                FileStream fs = File.OpenRead(request.Uri.AbsolutePath);

                Response response =  await Task.Run(() => {
                    return Response.Ok(request.Uri.AbsoluteUri, fs);
                });
                return response;

            } catch (FileNotFoundException) {
                return Response.Failure(request.Uri.AbsoluteUri, ErrorCode.FileNotFound, string.Format("The file {0} could not located", request.Uri.AbsolutePath));
            } catch (Exception exp) {
                return Response.Failure(request.Uri.AbsoluteUri, ErrorCode.FailedToGet, string.Format("An exception occured during download: {0}", exp.Message));
            }
           
        }
    }
}
