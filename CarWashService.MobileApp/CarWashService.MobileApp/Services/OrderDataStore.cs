using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class OrderDataStore : IDataStore<SerializedOrder>
    {
        public async Task<bool> AddItemAsync(SerializedOrder item)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    string discountJson = JsonConvert.SerializeObject(item);
                    HttpResponseMessage response = await client
                        .PostAsync(new Uri(client.BaseAddress + "orders"),
                                   new StringContent(discountJson,
                                                     Encoding.UTF8,
                                                     "application/json"));
                    return response.StatusCode == System.Net.HttpStatusCode.Created;
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return await Task.FromResult(false);
                }
            }
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SerializedOrder> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SerializedOrder>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(SerializedOrder item)
        {
            throw new NotImplementedException();
        }
    }
}
