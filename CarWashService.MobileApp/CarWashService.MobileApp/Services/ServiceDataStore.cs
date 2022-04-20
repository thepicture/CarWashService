using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    class ServiceDataStore : IDataStore<SerializedService>
    {
        public async Task<bool> AddItemAsync(SerializedService item)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    string serviceJson = JsonConvert.SerializeObject(item);
                    HttpResponseMessage response = await client
                        .PostAsync(new Uri(client.BaseAddress + "services"),
                                   new StringContent(serviceJson,
                                                     Encoding.UTF8,
                                                     "application/json"));
                    string content = await response.Content.ReadAsStringAsync();
                    return response.StatusCode ==
                        System.Net.HttpStatusCode.Created;
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .DeleteAsync($"services?id={id}");
                    return response.StatusCode == System.Net.HttpStatusCode.NoContent;
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return false;
                }
            }
        }

        public async Task<SerializedService> GetItemAsync(string id)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                      new AuthenticationHeaderValue("Basic",
                                                    AppIdentity.AuthorizationValue);
                    client.BaseAddress = new Uri(App.BaseUrl);
                    string response = await client
                      .GetAsync($"services/{id}")
                      .Result
                      .Content
                      .ReadAsStringAsync();
                    SerializedService service = JsonConvert
                        .DeserializeObject<SerializedService>(response);
                    return service;
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.StackTrace);
                return null;
            };
        }

        public async Task<IEnumerable<SerializedService>> GetItemsAsync
        (bool forceRefresh = false)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                        AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
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
