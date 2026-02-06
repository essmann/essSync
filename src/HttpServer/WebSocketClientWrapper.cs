using System.Net.Security;

public class WebSocketClientWrapper
{
    private HttpServer _server;

    public WebSocketClientWrapper(HttpServer server)
    {
        this._server = server;
    }


    public void onMessageReceive(string message) { }
    public async Task EnqueueMessage(string message, int delayMs = 0)
    {
        await Task.Delay(delayMs);
        await _server.QueueMessage(message);
    }

}