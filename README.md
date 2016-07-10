# Extget

Extget is C# based library to download files from multiple sources. Extget is extensible and support for different protcol (URI schemes) can be added dynamically [by just placing the plugin dll to plugins folder].

Extget currently supports **HTTP, HTTPS, and FILE** schemes. 
.
Extget uses asynchronous programming (async, await) to download save files concurrently. 

### Usage
```sh
Extget -i <input file> -o <output directory> [-c <number of files to download concurrently>]

- i and -o options are mandatory
- c is optional. If this option is not specified, then a deafult value of 20 is used. 
- The input file should contain the source URIs - each on a seperate line

- Number of files to downloaed concurrently determines the concurrecy level. It should be greater than 0. 
   Each concurrent download consumes approximately 6 MB of memory. So provide a concurrency level    depending upon the amount of memory your system has.

Bu default Extget supports Http, Https and File URIs. Support for more protocols can be added by adding plugins to the plugins folder. 
Example:
Extget -i input_file.txt -o output_dir -c 25
```
### Version
0.0.1

### Design
```
[Command Line Tool] --- [Workbench] --- [Scheduler] --- [Filegetter] --- [Handler]
```
1. Command Line tools interprets the arguments, parses the configuration and invokes the Workbench
2. Workbench finds and loads the plugins, and enqueues the files to be downloaded onto the scheduler
3. The scheduler controls the concurrency, gets the appropriate handler for each URI, and downloads the files by invoking Filegetter
4. Filegetter invokes the handler to read the stream asynchronously and then asychrnously copies the steam to a filestream

### Developing Plugins

A plugin has to implement Extget.Worker.IHandler interface. IHandler interface has the following definition:
```C#
namespace Extget.Worker
{
    public interface IHandler {
        string[] Schemes { get; }
        Task<Response> GetAsync(Request uri);
    }
}
```

Note that the Get method should support asynchronous read. It should not ideally consume more than 6-7 MB of memory. An example is provided below:
```C#
public class HttpHandler : IHandler {
        public string[] Schemes {
            get {
                return new string[] { "http", "https" };
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
```
### Build

Clone the respository to a directory:
```sh
$ git clone https://github.com/nmjmdr/extget
```
To compile and build, Extget requires: [.Net Framework and MSBuild]. 
```sh
$ cd extget
$ build.bat
```
This creates a folder called "Output" which holds all output of the build.

### Tests

A few tests have been included. These can be found in "System_Tests" folder. They can be run by running the following batch files:
- huge_large_small.bat
- large_and_small_test.bat
- small_files_test.bat
- throttled_concurrency_test.bat
- with_access_denied_test.bat

### Things yet to be done

* Ability to cancel any download
* Progress reporting 
* Support for FTP, SFTP
* 