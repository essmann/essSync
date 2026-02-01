using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using essSync.src.Database;
public class Endpoints
{


    private HttpServer _server;
    private DbApi _dbApi;
    public Endpoints(HttpServer _server, DbApi dbApi)
    {
        this._server = _server;
        this._dbApi = dbApi;
    }


    public void init()
    {

        // Add HTTP endpoints
        _server.AddGetEndpoint("/status", async (request, res) =>
        {
            return $"_server is running. WebSocket connected: {HttpServer.Client != null}";
        });

        _server.AddGetEndpoint("/health", async (request, res) =>
        {
            return "OK";
        });

        // _server.AddGetEndpoint("/folders", async (request, res) =>
        // {

        //     //1)  Retrieve device ID in a secure manner

        //     var folders = dbApi.GetAllSharedFolders();
        //     var json = JsonArray.FromArray(folders.Select(f => JsonObject.Parse(System.Text.Json.JsonSerializer.Serialize(f))));
        //     return json.ToJsonString();
        // })
        _server.AddPostEndpoint("/addFolder", async (request, res, body) =>
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                // string body = await reader.ReadToEndAsync();
                var folderData = System.Text.Json.JsonSerializer.Deserialize<SharedFolder>(body);

                if (folderData != null)
                {
                    _dbApi.AddSharedFolder(folderData);
                    return "Folder added successfully.";
                }
                else
                {
                    res.StatusCode = 400;
                    return "Invalid folder data.";
                }
            }
        });

        _server.AddGetEndpoint("/*", async (request, res) =>
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
    }

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

}

