using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    internal class MarvelApiConnector
    {
        private HttpClient apiClient;
        private Tuple<string, string> apiKeys;

        internal MarvelApiConnector(string privateApi, string publicApi, string address)
        {
            this.apiClient = new HttpClient();
            this.SetupApiClient(address);
            this.apiKeys = new Tuple<string, string>(privateApi, publicApi);
        }

        public string PublicKey
        {
            get
            {
                return this.apiKeys.Item2;
            }
        }

        public string PrivateKey
        {
            get
            {
                return this.apiKeys.Item1;
            }
        }

        private void SetupApiClient(string address)
        {
            this.apiClient.BaseAddress = new Uri(address);
            this.apiClient.DefaultRequestHeaders.Accept.Clear();
            this.apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<string> GenerateHash()
        {
            return await Task.Run(() => this.GenerateHashString());
        }

        private string GenerateHashString()
        {
            // Create the sum key
            string sumString = this.GenerateTimespan() + this.PrivateKey + this.PublicKey;

            // Create MD5 crypto encoder
            MD5 md5 = MD5.Create();

            byte[] stringMap = Encoding.UTF8.GetBytes(sumString);
            byte[] hash = md5.ComputeHash(stringMap);

            StringBuilder hexHash = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                // Format the string into hexadecimal
                hexHash.Append(hash[i].ToString("x2"));
            }

            return hexHash.ToString();
        }

        private string GenerateTimespan()
        {
            return ((int)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds).ToString();
        }

        private string GenerateRequestString(int responseLimit)
        {
            StringBuilder call = new StringBuilder();
            call.AppendFormat(@"/v1/public/comics?");
            call.AppendFormat(@"ts={0}&", this.GenerateTimespan());
            call.AppendFormat(@"apikey={0}&", this.PublicKey);
            call.AppendFormat(@"hash={0}&", this.GenerateHashString());
            call.AppendFormat(@"limit={0}", responseLimit);

            return call.ToString();
        }

        public async Task<HttpResponseMessage> GetApiResponse()
        {
            HttpResponseMessage responseMessage = await this.apiClient.GetAsync(GenerateRequestString(5));
            return responseMessage;
        }

        public async Task<Rootobject> GetAPI()
        {
            HttpResponseMessage test = await GetApiResponse();

            Rootobject returns = await test.Content.ReadAsAsync<Rootobject>();

            return returns;
        }
    }
}
