using Extget.Common;
using Extget.Worker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpHandlerPlugin {
    public class HttpHandler : IHandler {
        public string Scheme {
            get {
                return "http";
            }
        }

        public async Task<Response> GetAsync(Request request) {

            HttpClient client = new HttpClient();

            try {
                HttpResponseMessage httpResponse = await client.GetAsync(request.Uri, HttpCompletionOption.ResponseHeadersRead);

                if (!httpResponse.IsSuccessStatusCode) {
                    Response response = buildErrorResponse(request.Uri,httpResponse.StatusCode, httpResponse.ReasonPhrase);
                    return response;
                }

                Stream stream = await httpResponse.Content.ReadAsStreamAsync();
                return Response.Ok(request.Uri.AbsoluteUri,stream);

            } catch(Exception exp) {
                return Response.Failure(request.Uri.AbsoluteUri,ErrorCode.FailedToGet, string.Format("An exception occured during download: {0}", exp.Message));
            }
        }

        private Response buildErrorResponse(Uri uri,HttpStatusCode statusCode, string reasonPhrase) {
            ErrorCode code = ErrorCode.FailedToGet;
            switch (statusCode) {
                case HttpStatusCode.NotFound:
                    code = ErrorCode.FileNotFound;
                    break;
            }

            return Response.Failure(uri.AbsoluteUri,code, reasonPhrase);
        }
    }
}
