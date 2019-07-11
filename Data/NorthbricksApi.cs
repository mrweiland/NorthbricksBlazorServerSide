using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
namespace BlazorTestServerSide.Data
{
    public class NorthbricksApi
    {
        static readonly HttpClient client = new HttpClient();
        public async Task<string> GetBanks()
        {
            var rng = new Random();
            HttpResponseMessage response = await client.GetAsync("https://api.northbricks.io/api/v1/banks");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            

            return responseBody;

        }
    }
}
