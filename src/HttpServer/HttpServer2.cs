using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace essSync.src.HttpServer
{
    public class HttpServer2
    {
        public int Port;

        private HttpListener _listener;

        private Dictionary<string, Func<HttpListenerRequest, HttpStatusCode>> GetMethods;
        public HttpServer2(int Port = 8080)
        {
            this.Port = Port;
            this.GetMethods = new();
        }

       
        public async Task Start()
        {
            _listener = new HttpListener();

            _listener.Prefixes.Add($"http://localhost:{Port}/");

            _listener.Start();



            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
                Console.WriteLine(context);

                
            }
        }


       
    }
}
