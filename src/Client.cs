using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client
{

    private TcpListener tcpListener;

    private static string ipAddress = "127.0.0.1";
    private static int port = 52345;
    private IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    public List<IPEndPoint> clientIps;


    private int heartbeatIntervalMs = 10000;

    public Client()
    {

        //1) Fetch devices from database, initialize clientIps.


        //Start listening
        // Thread listenerThread = new Thread(startListening);
        // listenerThread.Start();

        Task.Run(startListeningAsync);
        // while (true)
        // {

        // }

    }
    public async Task sendMessage(string message = "Hello")
    {
        using Socket client = new(
    ipEndPoint.AddressFamily,
    SocketType.Stream,
    ProtocolType.Tcp);

        await client.ConnectAsync(ipEndPoint);

        // Send message.
        var messageBytes = Encoding.UTF8.GetBytes(message);
        _ = await client.SendAsync(messageBytes, SocketFlags.None);
        Console.WriteLine($"Socket client sent message: \"{message}\"");

        // Receive ack.
        var buffer = new byte[1_024];
        var received = await client.ReceiveAsync(buffer, SocketFlags.None);
        var response = Encoding.UTF8.GetString(buffer, 0, received);
        if (response == "<|ACK|>")
        {
            Console.WriteLine(
                $"Socket client received acknowledgment: \"{response}\"");
        }


        client.Shutdown(SocketShutdown.Both);
    }

    public async void broadcastMessage()
    {

    }


    public async Task startListeningAsync()
    {
        // Set the TcpListener on port 13000.
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        // TcpListener server = new TcpListener(port);
        var server = new TcpListener(localAddr, port);

        // Start listening for client requests.
        server.Start();

        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Client connected");

            _ = HandleClientAsync(client); // fire-and-forget per client
        }

    }

    public async Task HandleClientAsync(TcpClient client)
    {
        byte[] buffer = new byte[1024];


        try
        {
            using NetworkStream stream = client.GetStream();
            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {msg}");
            }

            Console.WriteLine("Client disconnected");




        }
        catch (Exception e)
        {

        }
        finally
        {
            Console.WriteLine("Closing TCP connection");
            client.Close();
        }

    }
    public void startListening()
    {

        // Set the TcpListener on port 13000.
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        // TcpListener server = new TcpListener(port);
        var server = new TcpListener(localAddr, port);

        // Start listening for client requests.
        server.Start();

        // Buffer for reading data
        Byte[] bytes = new Byte[256];
        String data = null;

        // Enter the listening loop.
        while (true)
        {
            Console.Write("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            using TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");

            data = null;

            try
            {
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);


                    // Process the data sent by the client.
                    data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }

    public void onReceiveMessage()
    {

    }
    public void onClientConnect()
    {

    }

}

//How often should I try to connect to Clients?
