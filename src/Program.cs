using essSync.src.Database;
using essSync.src.HttpServer;
using essSync.src.Utils;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;



DeviceId.Initialize();
string deviceId = DeviceId.getDeviceId();
Console.WriteLine("Device ID: " + deviceId);

Client client = new Client();

Thread.Sleep(1000);

// Creating/Accessing the database
using var db = new SharedContext();
db.Database.EnsureCreated();

DbApi dbApi = new DbApi(db);


var newFolder = new SharedFolder
{
    FolderName = "My Documents",
    LocalPath = @"C:\Users\YourName\Documents\MyFolder",
    FolderGuid = Guid.NewGuid().ToString(),
    IsPaused = false,
    CreatedAt = DateTime.UtcNow,
    LastSyncedAt = DateTime.UtcNow

};




dbApi.AddSharedFolder(newFolder);

string[] paths = { "C:\\Users\\kenwi\\Downloads\\track", "C:\\Users\\kenwi\\Downloads\\track2" };
FolderWatcher.SetFolderPaths(paths);

//Start the HTTP server and initialize endpoints.
HttpServer server = new HttpServer();
var endpoints = new Endpoints(server, dbApi);
endpoints.init();

_ = server.Start();

Console.WriteLine("Server started. Press Enter to exit...");
Console.ReadLine();




