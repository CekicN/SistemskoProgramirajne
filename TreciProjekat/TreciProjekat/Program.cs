using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreciProjekat
{
    internal class Program
    {
        static HttpListener listener = new HttpListener();
        static async Task Main()
        {
            listener.Prefixes.Add($"http://localhost:5050/");
            listener.Start();
            var stream = new StatisticStream();
            var observer = new StatisticObserver();
            stream.Subscribe(observer);

            while(true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                Task.Run(() => stream.GetStatistic(context));
            }

        }
    }
}
//nU_VieVJBVc/4hhykkFMmHE
