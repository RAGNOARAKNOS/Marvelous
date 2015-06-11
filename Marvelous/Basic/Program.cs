using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    class Program
    {

        public class Rootobject
        {
            public string code { get; set; }
            public string status { get; set; }
            public string copyright { get; set; }
            public string attributionText { get; set; }
            public string attributionHTML { get; set; }
            public Data data { get; set; }
            public string etag { get; set; }
        }

        public class Data
        {
            public string offset { get; set; }
            public string limit { get; set; }
            public string total { get; set; }
            public string count { get; set; }
            public Result[] results { get; set; }
        }

        public class Result
        {
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string modified { get; set; }
            public string resourceURI { get; set; }
            public Url[] urls { get; set; }
            public Thumbnail thumbnail { get; set; }
            public Comics comics { get; set; }
            public Stories stories { get; set; }
            public Events events { get; set; }
            public Series series { get; set; }
        }

        public class Thumbnail
        {
            public string path { get; set; }
            public string extension { get; set; }
        }

        public class Comics
        {
            public string available { get; set; }
            public string returned { get; set; }
            public string collectionURI { get; set; }
            public Item[] items { get; set; }
        }

        public class Item
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Stories
        {
            public string available { get; set; }
            public string returned { get; set; }
            public string collectionURI { get; set; }
            public Item1[] items { get; set; }
        }

        public class Item1
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        public class Events
        {
            public string available { get; set; }
            public string returned { get; set; }
            public string collectionURI { get; set; }
            public Item2[] items { get; set; }
        }

        public class Item2
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Series
        {
            public string available { get; set; }
            public string returned { get; set; }
            public string collectionURI { get; set; }
            public Item3[] items { get; set; }
        }

        public class Item3
        {
            public string resourceURI { get; set; }
            public string name { get; set; }
        }

        public class Url
        {
            public string type { get; set; }
            public string url { get; set; }
        }

        static void Main(string[] args)
        {
            RunAsync().Wait();
        }
        static async Task RunAsync()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://gateway.marvel.com");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //string ts = @"123456789101111111111";
            string ts = ((int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            string publicapi = @"944be256913a7a96f2315c76918ec609";
            string privateapi = @"92e6098644d89d4eb82d611f6ea649e29b385aff";
            int limit = 5;

            string big = ts + privateapi + publicapi;

            Debug.WriteLine(big);

            MD5 md5 = MD5.Create();

            byte[] stringmap = Encoding.UTF8.GetBytes(big);

            byte[] hash = md5.ComputeHash(stringmap);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2")); //Format to hexadecimal 
            }

            string hashstring = sb.ToString();

            StringBuilder call = new StringBuilder();
            call.AppendFormat(@"/v1/public/characters?");
            call.AppendFormat(@"ts={0}&", ts);
            call.AppendFormat(@"apikey={0}&", publicapi);
            call.AppendFormat(@"hash={0}&", hashstring);
            call.AppendFormat(@"limit={0}", limit.ToString());



            Debug.WriteLine(call);

            HttpResponseMessage response = await client.GetAsync(call.ToString());
            Console.ReadKey();
            Console.Write(response.ToString());
            Console.ReadKey();

            Rootobject returns = await response.Content.ReadAsAsync<Rootobject>();
            Console.ReadKey();
        }
    }
}
