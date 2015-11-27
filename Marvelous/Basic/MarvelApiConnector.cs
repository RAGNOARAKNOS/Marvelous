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
            return await Task.Run(() => this.GenerateHashTest());
        }

        private string GenerateHashTest()
        {
            // Create the sum key
            string sumString = this.GenerateTimespan() + this.PrivateKey + this.PublicKey;

            // Create MD5 crypto encoder
            MD5 md5 = MD5.Create();

            byte[] stringMap = Encoding.UTF8.GetBytes(sumString);
            byte[] hash = md5.ComputeHash(stringMap);

            StringBuilder hexHash = new StringBuilder();
            for (int i = 0; i < hexHash.Length; i++)
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

        private string GenerateRequestString()
        {

        }
    }
}
