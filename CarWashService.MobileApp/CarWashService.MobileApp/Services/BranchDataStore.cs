using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CarWashService.MobileApp.Services
{
    public class BranchDataStore : IDataStore<SerializedBranch>
    {
        private int _responseId;
        public async Task<bool> AddItemAsync(SerializedBranch item)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    string branchJson = JsonConvert.SerializeObject(item);
                    HttpResponseMessage response = await client
                        .PostAsync(new Uri(client.BaseAddress + "branches"),
                                   new StringContent(branchJson,
                                                     Encoding.UTF8,
                                                     "application/json"));
                    string content = await response.Content.ReadAsStringAsync();
                    _responseId = Convert.ToInt32(content
                                     .Replace("\"", "")
                        );
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return await Task.FromResult(false);
                }
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(SerializedBranch item)
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            return await Task.FromResult(false);
        }

        public async Task<SerializedBranch> GetItemAsync(string id)
        {
            return await Task.FromResult(new SerializedBranch
            {
                Id = _responseId
            });
        }

        public async Task<IEnumerable<SerializedBranch>> GetItemsAsync
            (bool forceRefresh = false)
        {
            return await Task.FromResult<IEnumerable<SerializedBranch>>(null);
        }
    }
}