using essSync.src.Database;
using essSync.src.HttpServer;
using essSync.src.Utils;
using System.Net;
using System.Reflection;
using System.Text;

// Creating/Accessing the database
using var db = new SharedContext();
db.Database.EnsureCreated();

string[] paths = { "C:\\Users\\kenwi\\Downloads\\track", "C:\\Users\\kenwi\\Downloads\\track2" };
FolderWatcher.SetFolderPaths(paths);

HttpServer server = new HttpServer();

// Add HTTP endpoints
server.AddGetEndpoint("/status", async (request, res) =>
{
    return $"Server is running. WebSocket connected: {HttpServer.Client != null}";
});

server.AddGetEndpoint("/health", async (request, res) =>
{
    return "OK";
});
//server.AddGetEndpoint("/", async (request, res) =>
//{

//    Assembly assembly = Assembly.GetExecutingAssembly();
//    foreach( var thing in assembly.GetManifestResourceNames())
//    {
//        Console.WriteLine(thing);

//        Stream stream = assembly.GetManifestResourceStream(thing);

//        Console.WriteLine(thing);

//        if (thing.EndsWith(".html"))
//        {
//            if (stream!= null)
//            {
//                res.ContentType = "text/html";

//                using (stream)
//                using (StreamReader reader = new StreamReader(stream))
//                {
//                    var html = await reader.ReadToEndAsync();
//                    return html;

//                    //var htmlBuffer = Encoding.UTF8.GetBytes(html);
//                    //await res.OutputStream.WriteAsync(htmlBuffer, 0, htmlBuffer.Length);


//                }
//            }
//        }
//    }
//    return "OK";
//});

server.AddGetEndpoint("/*", async (request, res) => 
{
    const string assemblyPrefix = "essSync.src.frontend.dist";
    // Get the requested path from the URL
    string path = request.Url.AbsolutePath; // e.g., "/style.css"

    Console.WriteLine($"Browser requested: {path}");

    // Remove leading slash and default to index.html
    string fileName = path.TrimStart('/');
    if (string.IsNullOrEmpty(fileName))
    {
        fileName = "index.html";
    }

    // Map to embedded resource name
    string resourceName = $"{assemblyPrefix}.{fileName.Replace("/", ".")}";

    Console.WriteLine(resourceName);
    Assembly assembly = Assembly.GetExecutingAssembly();
    Stream stream = assembly.GetManifestResourceStream(resourceName);

    if (stream == null)
    {
        res.StatusCode = 404;
        return "Not Found";
    }

    res.ContentType = GetContentType(fileName);

    using (stream)
    using (StreamReader reader = new StreamReader(stream))
    {
        return await reader.ReadToEndAsync();
    }
});

_ = server.Start();

Console.WriteLine("Server started. Press Enter to exit...");
Console.ReadLine();


static string GetContentType(string path)
{
    string extension = Path.GetExtension(path).ToLowerInvariant();
    return extension switch
    {
        ".html" => "text/html",
        ".css" => "text/css",
        ".js" => "application/javascript",
        ".json" => "application/json",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".svg" => "image/svg+xml",
        _ => "text/plain"
    };
}