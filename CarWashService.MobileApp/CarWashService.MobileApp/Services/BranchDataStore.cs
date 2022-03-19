using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CarWashService.MobileApp.Services
{
    public class BranchDataStore : IDataStore<SerializedBranch>
    {
        public async Task<bool> AddItemAsync(SerializedBranch item)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization,
                                   await SecureStorage
                                    .GetAsync("Identity"));
                client.Headers.Add(HttpRequestHeader.ContentType,
                                 "application/json");
                client.BaseAddress = (App.Current as App).BaseUrl;
                try
                {
                    byte[] encodedBranch = Encoding.UTF8
                        .GetBytes(
                        JsonConvert.SerializeObject(item));
                    byte[] response = await client
                        .UploadDataTaskAsync("api/branches", encodedBranch);
                }
                catch (WebException ex)
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
            return await Task.FromResult<SerializedBranch>(null);
        }

        public async Task<IEnumerable<SerializedBranch>> GetItemsAsync
            (bool forceRefresh = false)
        {
            return await Task.FromResult<IEnumerable<SerializedBranch>>(null);
        }
    }
}