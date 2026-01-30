using System.Net;
using System.Net.WebSockets;
using System.Text;

public class HttpServer
{
    public int Port;
    private HttpListener _listener;
    public static WebSocket Client = null;
    private static readonly object _clientLock = new object();

    // Add HTTP endpoint handlers
    private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task<string>>> _getEndpoints;
    private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task<string>>> _postEndpoints;



    public HttpServer(int Port = 8080)
    {
        this.Port = Port;
        _getEndpoints = new();
        _postEndpoints = new();
    }

    public void AddGetEndpoint(string path, Func<HttpListenerRequest, HttpListenerResponse, Task<string>> handler)
    {
        _getEndpoints[path] = handler;
    }

    public void AddPostEndpoint(string path, Func<HttpListenerRequest, HttpListenerResponse, Task<string>> handler)
    {
        _postEndpoints[path] = handler;
    }

    public async Task Start()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://localhost:{Port}/");
        _listener.Start();

        Console.WriteLine($"Server started on http://localhost:{Port}/");

        while (_listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    // Handle WebSocket
                    var wsContext = await context.AcceptWebSocketAsync(null);

                    lock (_clientLock)
                    {
                        if (Client != null && Client.State == WebSocketState.Open)
                        {
                            Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "New connection", CancellationToken.None);
                        }
                        Client = wsContext.WebSocket;
                    }

                    Console.WriteLine("WebSocket client connected");
                    _ = Task.Run(() => HandleWebSocketConnection(wsContext.WebSocket));
                }
                
                else
                {
                    // Handle HTTP request on separate thread
                    _ = Task.Run(() => HandleHttpRequest(context));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private async Task HandleHttpRequest(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            string path = request.Url.AbsolutePath;
            Console.WriteLine($"HTTP {request.HttpMethod} {path}");

            string responseText = null;

            if (request.HttpMethod == "GET")
            {
                // First try exact match
                if (_getEndpoints.TryGetValue(path, out var getHandler))
                {
                    responseText = await getHandler(request, response);
                }
                // Then try wildcard match
                else if (_getEndpoints.TryGetValue("/*", out var wildcardHandler))
                {
                    responseText = await wildcardHandler(request, response);
                }
                else
                {
                    response.StatusCode = 404;
                    responseText = "404 - Not Found";
                }
            }
            else if (request.HttpMethod == "POST")
            {
                // First try exact match
                if (_postEndpoints.TryGetValue(path, out var postHandler))
                {
                    responseText = await postHandler(request, response);
                }
                // Then try wildcard match
                else if (_postEndpoints.TryGetValue("/*", out var wildcardHandler))
                {
                    responseText = await wildcardHandler(request, response);
                }
                else
                {
                    response.StatusCode = 404;
                    responseText = "404 - Not Found";
                }
            }

            // Only write if handler returned text (didn't write manually)
            if (responseText != null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                response.ContentLength64 = buffer.Length;
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            }

            // Always close the response
            response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling HTTP request: {ex.Message}");
        }
    }

    private async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received: {message}");
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closing",
                        CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
        }
        finally
        {
            lock (_clientLock)
            {
                if (Client == webSocket)
                {
                    Client = null;
                }
            }
            webSocket?.Dispose();
            Console.WriteLine("Client disconnected");
        }
    }

    // Helper method to send messages
    public static async Task SendMessage(string message)
    {
        if (Client != null && Client.State == WebSocketState.Open)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(message);
                await Client.SendAsync(
                    new ArraySegment<byte>(data),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                Console.WriteLine($"Sent: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("No client connected");
        }
    }

    public void Stop()
    {
        _listener?.Stop();
        _listener?.Close();
    }
}