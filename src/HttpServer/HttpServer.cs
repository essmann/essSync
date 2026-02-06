using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Text.Json;

public class HttpServer
{
    public int Port;
    private HttpListener _listener;
    public static WebSocket Client = null;
    private static readonly object _clientLock = new object();

    //For queuing messages to send from other parts of the system
    private static readonly ConcurrentQueue<string> _messageQueue = new();
    private static readonly CancellationTokenSource _wsCts = new();
    private static Task _wsSenderTask;

    // Add HTTP endpoint handlers
    private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, Task<string>>> _getEndpoints;
    private Dictionary<string, Func<HttpListenerRequest, HttpListenerResponse, string, Task<string>>> _postEndpoints;

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

    public void AddPostEndpoint(string path, Func<HttpListenerRequest, HttpListenerResponse, string, Task<string>> handler)
    {
        _postEndpoints[path] = handler;
    }

    public async Task Start()
    {
        _listener = new HttpListener();

        _listener.Prefixes.Add($"http://localhost:{Port}/");
        _listener.Start();
        _ = startMessageQueueProcessor(); //Starts tracking the queue for messages in a thread

        LogInfo($"Server started on http://localhost:{Port}/");

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
                        //If there is already an open websocket connection, close it.
                        if (Client != null && Client.State == WebSocketState.Open)
                        {
                            Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "New connection", CancellationToken.None);
                        }
                        Client = wsContext.WebSocket;
                    }

                    LogSuccess("WebSocket client connected");
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
                LogError($"Error: {ex.Message}");
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

            // Log HTTP request with color
            LogHttpRequest(request.HttpMethod, path);

            // Read and log POST body if present
            string requestBody = null;
            if (request.HttpMethod == "POST" && request.HasEntityBody)
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                    LogPostBody(requestBody);
                }
            }

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
                    LogWarning($"404 - Endpoint not found: {path}");
                }
            }
            else if (request.HttpMethod == "POST")
            {
                // First try exact match
                if (_postEndpoints.TryGetValue(path, out var postHandler))
                {
                    responseText = await postHandler(request, response, requestBody);
                }
                // Then try wildcard match
                else if (_postEndpoints.TryGetValue("/*", out var wildcardHandler))
                {
                    responseText = await wildcardHandler(request, response, requestBody);
                }
                else
                {
                    response.StatusCode = 404;
                    responseText = "404 - Not Found";
                    LogWarning($"404 - Endpoint not found: {path}");
                }
            }

            // Log response status
            LogHttpResponse(response.StatusCode);

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
            LogError($"Error handling HTTP request: {ex.Message}");
            LogError($"Stack trace: {ex.StackTrace}");
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
                    LogWebSocket($"◀ {message}");
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
            LogError($"WebSocket error: {ex.Message}");
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
            LogWarning("Client disconnected");
        }
    }

    // Helper method to send messages
    private static async Task SendMessage(string message)
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

                LogWebSocket($"▶ {message}");
            }
            catch (Exception ex)
            {
                LogError($"Error sending: {ex.Message}");
            }
        }
        else
        {
            LogWarning("No client connected");
        }
    }

    public void Stop()
    {
        _listener?.Stop();
        _listener?.Close();
    }

    private async Task startMessageQueueProcessor()
    {
        //this token tells the task to cancel.
        while (!_wsCts.Token.IsCancellationRequested)
        {

            if (!_messageQueue.IsEmpty)
            {

                try
                {
                    Console.WriteLine("Attempting to send message via queue.");
                    Console.WriteLine("Queue size: " + _messageQueue.Count);

                    string message;
                    _messageQueue.TryDequeue(out message);
                    await SendMessage(message);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            else
            {
                await Task.Delay(100);
            }

        }

    }
    public async Task QueueMessage(string message)
    {
        _messageQueue.Enqueue(message);
        await Task.CompletedTask;
    }
    // ============================================
    // COLORFUL LOGGING METHODS
    // ============================================

    private static void LogHttpRequest(string method, string path)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"[{DateTime.Now:HH:mm:ss}] ");

        // Color code by HTTP method
        switch (method)
        {
            case "GET":
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case "POST":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case "PUT":
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case "DELETE":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }

        Console.Write($"{method,-6}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($" {path}");
        Console.ResetColor();
    }

    private static void LogPostBody(string body)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("       │ ");
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("Body: ");
        Console.ResetColor();

        // Try to pretty-print JSON
        try
        {
            var jsonDoc = JsonDocument.Parse(body);
            var prettyJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            var lines = prettyJson.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(lines[i]);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("       │ ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(lines[i]);
                }
            }
            Console.ResetColor();
        }
        catch
        {
            // Not JSON, just print as-is
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(body);
            Console.ResetColor();
        }
    }

    private static void LogHttpResponse(int statusCode)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("       └─> ");

        if (statusCode >= 200 && statusCode < 300)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (statusCode >= 400 && statusCode < 500)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (statusCode >= 500)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.WriteLine($"{statusCode}");
        Console.ResetColor();
    }

    private static void LogSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("✓ ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    private static void LogInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("ℹ ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    private static void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("⚠ ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    private static void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("✗ ");
        Console.ResetColor();
        Console.WriteLine(message);
    }

    private static void LogWebSocket(string message)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Write("[WS] ");
        Console.ResetColor();
        Console.WriteLine(message);
    }
}