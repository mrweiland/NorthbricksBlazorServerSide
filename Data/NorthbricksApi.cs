using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlazorTestServerSide.Data
{
    public class NorthbricksApi
    {
        static readonly HttpClient client = new HttpClient();
        public async Task<Bank[]> GetBanks()
        {
            Banks model = null;
            HttpResponseMessage response = await client.GetAsync("https://api.northbricks.io/api/v1/banks");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            model = JsonConvert.DeserializeObject<Banks>(responseBody);
            

            return model.banks.ToArray();

        }
    }
}
