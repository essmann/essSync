using essSync.src.Database;
using essSync.src.HttpServer;
using essSync.src.Utils;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;


DeviceId.Initialize();
string deviceId = DeviceId.getDeviceId();
Console.WriteLine("Device ID: " + deviceId);

Client client = new Client();

Thread.Sleep(1000);

// Creating/Accessing the database
using var db = new SharedContext();
FolderWatcher watcher = new FolderWatcher();
db.Database.EnsureCreated();
DbApi dbApi = new DbApi(db);
SharedFolderApi sharedFolderApi = new SharedFolderApi(dbApi, watcher);

watcher.SetDbApi(dbApi, sharedFolderApi);

//Gets folders from DB and initializes watchers.
sharedFolderApi.Init();
sharedFolderApi.AddSharedFolder("C:\\Users\\kenwi\\Downloads\\track");

//Start the HTTP server and initialize endpoints.
HttpServer server = new HttpServer();
WebSocketClientWrapper webSocketClientWrapper = new(server);
var endpoints = new Endpoints(server, dbApi, sharedFolderApi);
endpoints.init();

_ = server.Start();

Console.WriteLine("Server started. Press Enter to exit...");


sharedFolderApi.FolderDeleted += async (folder) =>
{
    var message = new FolderDeleteMessage(folder);
    await webSocketClientWrapper
    .EnqueueMessage
    (JsonSerializer.Serialize<FolderDeleteMessage>(message));
    Console.WriteLine(folder);

    Console.WriteLine(message);
};



while (true)
{
    string key = Console.ReadLine();

    if (key == "A")
    {
        watcher.PrintPaths();
    }

}

Console.ReadLine();




