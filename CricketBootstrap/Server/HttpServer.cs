using System.Net;
using System.Text;

namespace CricketBootstrap.Server;

public class HttpServer {
    private readonly HttpListener _httpListener;
    private readonly Dictionary<string, DynamicDocument> _documents;

    public HttpServer(Dictionary<string, DynamicDocument> documents, string address = "localhost", short port = 8080) {
        _documents = documents;
        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add($"http://{address}:{port}/");
        _httpListener.Start();

        var listenTask = HandleIncomingRequests();
        listenTask.GetAwaiter().GetResult();
    }

    private async Task HandleIncomingRequests() {
        for (;;) {
            var listenerContext = await _httpListener.GetContextAsync();
            var request = listenerContext.Request;
            var response = listenerContext.Response;
            if (request.Url != null) {
                await Console.Out.WriteLineAsync(
                    $"HttpServer: Client {request.UserHostAddress} requested {request.Url.AbsolutePath}");
                response.ContentType = "text/html";
                byte[] data;
                if (_documents.ContainsKey(request.Url.AbsolutePath)) {
                    lock (_documents) {
                        data = Encoding.UTF8.GetBytes(_documents[request.Url.AbsolutePath].Parse(request.QueryString));
                    }
                }
                else {
                    data = Encoding.UTF8.GetBytes("404");
                }
                response.ContentLength64 = data.LongLength;
                response.ContentEncoding = Encoding.UTF8;
                await response.OutputStream.WriteAsync(data);
            }
            response.Close();
        }
    }
}