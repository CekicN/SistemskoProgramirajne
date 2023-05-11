using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Text;
using System.Threading;

namespace Brojac
{
    class Brojac
    {
        static List<string> words = new List<string>();
        static Tuple<List<string>, DateTime> tuple = new Tuple<List<string>, DateTime>(words, DateTime.Now);//words-CacheAccessTime
        static int count;
        static Stopwatch stopwatch = new Stopwatch();
        static HttpListener listener = new HttpListener();
        static MemoryCache cache = new MemoryCache("MyCache");
        static void Main(string[] args)
        {
            listener.Prefixes.Add($"http://localhost:5050/");
            listener.Start();
            GetNumberOfWords();
        }

        static void GetNumberOfWords()
        {
            Console.WriteLine("Server je startovan");
            DateTime lastModified;
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                stopwatch.Start();
                var request = context.Request;
                string filePath = FindFile(request);
                lastModified = File.GetLastWriteTime(filePath);
                if (cache.Contains(filePath) && tuple.Item2 > lastModified)
                {
                    tuple = new Tuple<List<string>, DateTime>(words, DateTime.Now);
                    int brojReci = (int)cache.Get(filePath);
                    stopwatch.Stop();
                    Console.WriteLine($"Broj reci iz kesa: {brojReci}");
                }
                else
                {
                    if(cache.Contains(filePath))
                        cache.Remove(filePath);

                    words = File.ReadAllText(filePath).Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                    tuple = new Tuple<List<string>, DateTime>(words, DateTime.Now);
                    count = 0;
                    HttpResponseMessage res = null;
                    int brNiti = Environment.ProcessorCount;
                    Console.WriteLine($"Dostupan broj niti je: {brNiti}");
                    int worker_niti, io_niti;
                    ThreadPool.GetAvailableThreads(out worker_niti, out io_niti);
                    foreach (var word in tuple.Item1)
                    {
                        ThreadPool.QueueUserWorkItem((state) =>
                        {
                            if (word.Length > 5 && char.IsUpper(word[0]))
                            {
                                count++;
                            }
                        }, null);
                    }
                    bool Finished = false;
                    while (!Finished)
                    {
                        Thread.Sleep(1000);
                        Finished = ThreadPool.PendingWorkItemCount == 0;
                    }
                    stopwatch.Stop();
                    cache.Set(filePath, count, DateTimeOffset.UtcNow.AddHours(1));
                    Console.WriteLine($"Broj reci sa vise od 5 karaktera i prvim velikim slovom je: {count}");
                }
                Console.WriteLine($"Vreme za brojanje:{stopwatch.Elapsed}");
                stopwatch.Reset();
            }
        }
        static string FindFile(HttpListenerRequest request)
        {
            string rootFolder = Path.GetFullPath(".");
            string filename = request.Url.Segments.Last();
            string filePath = Directory.GetFiles(rootFolder, filename, SearchOption.AllDirectories).FirstOrDefault();
            if (filePath.Length > 0)
                return filePath;
            else
                return null;
        }
    }
}
