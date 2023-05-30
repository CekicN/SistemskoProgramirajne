using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Drugi_Projekat
{
    internal class Program
    {
        static Listener listener = new Listener();
        static List<string> words = new List<string>();
        static Cache cache = new Cache("My cache");
        static int count;
        static async Task Main()
        {
            listener.Start($"http://localhost:5050/");
            while(true)
            {   
               GetNumberOfWords();
            }
            listener.Close();
        }

        static void GetNumberOfWords()
        {
            _ = Task.Run(async () =>
            {
                var request = listener.request;
                DateTime lastModified;
                string filePath = FindFile(request);
                if (filePath != null)
                {
                    lastModified = File.GetLastWriteTime(filePath);
                    if (cache.Contains(filePath) && cache.AccessTime > lastModified)
                    {
                        cache.AccessTime = DateTime.Now;
                        Console.WriteLine($"Broj reci iz kesa: {cache.GetValue(filePath)}");
                    }
                    else
                    {

                        words = File.ReadAllText(filePath).Split(new char[] { ' ', '\t', '\n', '\r', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        cache.AccessTime = DateTime.Now;
                        count = 0;
                        foreach (var word in words)
                        {

                            if (word.Length > 5 && char.IsUpper(word[0]))
                            {
                                count++;
                            }
                        }
                        cache.Set(filePath, count);
                        Console.WriteLine($"Broj reci sa vise od 5 karaktera i prvim velikim slovom je: {count}");
                    }
                }
            });
        }
        static string FindFile(HttpListenerRequest request)
        {
            string rootFolder = Path.GetFullPath(".");
            string filename = request.Url.Segments.Last();
            string filePath = Directory.GetFiles(rootFolder, filename, SearchOption.AllDirectories).FirstOrDefault();
            if (filePath == null)
            {
                Console.WriteLine("Fajl ne postoji");
                return null;
            }
                
            if (filePath.Length > 0)
                return filePath;
            else
                return null;
        }
    }
}
