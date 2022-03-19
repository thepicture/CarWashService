using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    class ServiceDataStore : IDataStore<SerializedService>
    {
        public Task<bool> AddItemAsync(SerializedService item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SerializedService> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerializedService>> GetItemsAsync
            (bool forceRefresh = false)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                        AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    string response = await client
                        .GetAsync(new Uri(client.BaseAddress + "services"))
                        .Result
                        .Content.ReadAsStringAsync();
                    return JsonConvert
                        .DeserializeObject<IEnumerable<SerializedService>>(response);
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return null;
                }
            }
        }

        public Task<bool> UpdateItemAsync(SerializedService item)
        {
            throw new NotImplementedException();
        }
    }
}
