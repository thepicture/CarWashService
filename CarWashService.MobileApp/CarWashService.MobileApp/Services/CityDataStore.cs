using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CarWashService.MobileApp.Services
{
    class CityDataStore : IDataStore<SerializedCity>
    {
        public Task<bool> AddItemAsync(SerializedCity item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SerializedCity> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerializedCity>> GetItemsAsync
            (bool forceRefresh = false)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization,
                                   await SecureStorage
                                    .GetAsync("Identity"));
                client.BaseAddress = (App.Current as App).BaseUrl;
                try
                {
                    string response = await client
                        .DownloadStringTaskAsync("api/cities");
                    IEnumerable<SerializedCity> cities = JsonConvert
                        .DeserializeObject
                        <IEnumerable<SerializedCity>>(response);
                    return await Task.FromResult(cities);
                }
                catch (WebException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return await Task.FromResult
                        <IEnumerable<SerializedCity>>(null);
                }
            }
        }

        public Task<bool> UpdateItemAsync(SerializedCity item)
        {
            throw new NotImplementedException();
        }
    }
}
