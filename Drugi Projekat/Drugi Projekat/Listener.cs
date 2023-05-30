using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Drugi_Projekat
{
    internal class Listener
    {
        static HttpListener listener;
        public Listener()
        {
            listener = new HttpListener();
        }

        public void Start(string host)
        {
            listener.Prefixes.Add(host);
            listener.Start();
            Console.WriteLine("Server je startovan");
        }
        public void Close()
        {
            listener.Close();
        }
        private async Task<HttpListenerContext> context()
        {   
            return await listener.GetContextAsync();
        }

        public HttpListenerRequest request
        {
            get { return this.context().Result.Request; }
        }
    }
}
