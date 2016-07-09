using System;
using System.Threading.Tasks;
using Extget.Worker;
using System.Net.Http;
using System.Net;
using System.IO;
using Extget.Common;

namespace TestBed {
    public class HttpHandler : IHandler {
        public string Scheme {
            get {
                return "http";
            }
        }

        public async Task<Response> GetAsync(Request request) {

            HttpClient client = new HttpClient();

            HttpResponseMessage httpResponse = await client.GetAsync(request.Uri, HttpCompletionOption.ResponseHeadersRead);

            if(!httpResponse.IsSuccessStatusCode) {
                Response response = buildErrorResponse(request.Uri,httpResponse.StatusCode, httpResponse.ReasonPhrase);
            }

            Stream stream = await httpResponse.Content.ReadAsStreamAsync();
            return Response.Ok(request.Uri.AbsoluteUri,stream);
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