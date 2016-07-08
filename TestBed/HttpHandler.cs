using System;
using System.Threading.Tasks;
using Extget.Worker;
using System.Net.Http;
using System.Net;
using System.IO;

namespace TestBed {
    public class HttpHandler : IHandler {
        public async Task<Response> GetAsync(Uri uri) {

            HttpClient client = new HttpClient();

            HttpResponseMessage httpResponse = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);

            if(!httpResponse.IsSuccessStatusCode) {
                Response response = buildErrorResponse(httpResponse.StatusCode, httpResponse.ReasonPhrase);
            }

            Stream stream = await httpResponse.Content.ReadAsStreamAsync();
            return Response.Ok(stream);
        }

        private Response buildErrorResponse(HttpStatusCode statusCode, string reasonPhrase) {
            ErrorCode code = ErrorCode.FailedToGet;
            switch (statusCode) {
                case HttpStatusCode.NotFound:
                    code = ErrorCode.FileNotFound;
                    break;                
            }

            return Response.Failure(code, reasonPhrase);
        }
    }
}