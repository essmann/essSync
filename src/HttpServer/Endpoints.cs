using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using essSync.src.Database;
public class Endpoints
{


    private HttpServer _server;
    private DbApi _dbApi;
    private SharedFolderApi _folderApi;
    public Endpoints(HttpServer _server, DbApi dbApi, SharedFolderApi folderApi)
    {
        this._server = _server;
        this._dbApi = dbApi;
        this._folderApi = folderApi;
    }


    public void init()
    {

        _server.AddGetEndpoint("/test", async (request, res) =>
        {
            res.ContentType = "application/json";
            var folders = _dbApi.GetAllSharedFoldersWithoutContents();
            List<FolderMessage> folderMessages = new();
            int i = 0;
            while (i < folders.Count)
            {
                folderMessages.Add(new FolderMessage(folders[i]));
                i++;
            }
            return JsonSerializer.Serialize(folderMessages);
        });
        // Add HTTP endpoints
        _server.AddGetEndpoint("/status", async (request, res) =>
        {
            return $"_server is running. WebSocket connected: {HttpServer.Client != null}";
        });

        _server.AddGetEndpoint("/health", async (request, res) =>
        {
            return "OK";
        });

        _server.AddGetEndpoint("/me", async (request, res) =>
       {
           string pcName = System.Environment.MachineName;
           var dto = new DeviceInfoDTO();
           dto.DeviceGuid = DeviceId.getDeviceId();
           dto.HostName = pcName;

           return JsonSerializer.Serialize(dto);
       });

        // _server.AddGetEndpoint("/folders", async (request, res) =>
        // {

        //     //1)  Retrieve device ID in a secure manner

        //     var folders = dbApi.GetAllSharedFolders();
        //     var json = JsonArray.FromArray(folders.Select(f => JsonObject.Parse(System.Text.Json.JsonSerializer.Serialize(f))));
        //     return json.ToJsonString();
        // })
        _server.AddGetEndpoint("/getFolders", async (req, res) =>
        {

            res.ContentType = "application/json";
            var folders = _dbApi.GetAllSharedFoldersWithoutContents();
            List<FolderMessage> folderMessages = new();
            int i = 0;
            while (i < folders.Count)
            {
                folderMessages.Add(new FolderMessage(folders[i]));
                i++;
            }
            return JsonSerializer.Serialize(folderMessages);
        });
        _server.AddPostEndpoint("/addFolder", async (request, res, body) =>
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                res.ContentType = "application/json";
                // string body = await reader.ReadToEndAsync();
                var folderData = System.Text.Json.JsonSerializer.Deserialize<SharedFolder>(body);

                if (folderData != null)
                {
                    _folderApi.AddSharedFolder(folderData.LocalPath);
                    FolderMessage folderMessage = new FolderMessage(folderData);
                    return JsonSerializer.Serialize<FolderMessage>(folderMessage);
                }
                else
                {
                    res.StatusCode = 400;
                    return "Invalid folder data.";
                }
            }
        });

        _server.AddPostEndpoint("/addDevice", async (request, res, body) =>
  {
      try
      {
          // Deserialize the incoming JSON to DeviceDTO
          var deviceDTO = System.Text.Json.JsonSerializer.Deserialize<DeviceDTO>(body);

          if (deviceDTO == null)
          {
              res.StatusCode = 400;
              return "Invalid device data.";
          }

          // Transform DTO to Entity
          Device deviceEntity = new Device
          {
              DeviceId = deviceDTO.DeviceId,
              DeviceGuid = deviceDTO.DeviceGuid,
              DeviceName = deviceDTO.DeviceName,
              IsThisDevice = deviceDTO.IsThisDevice,
              LastSeenAt = deviceDTO.LastSeenAt,
              IsConnected = deviceDTO.IsConnected,
              DeviceIps = deviceDTO.DeviceIps?.Select(ip => new DeviceIp
              {
                  DeviceGuid = deviceDTO.DeviceGuid,
                  Ip = ip
              }).ToList() ?? new List<DeviceIp>()
          };

          // Save to DB
          _dbApi.AddDevice(deviceEntity);

          return "Device added successfully.";
      }
      catch (Exception ex)
      {
          res.StatusCode = 500;
          return $"Error: {ex.Message}";
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

