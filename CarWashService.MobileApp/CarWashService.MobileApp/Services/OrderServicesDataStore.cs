using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Services
{
    public class OrderServicesDataStore : IDataStore<IEnumerable<SerializedService>>
    {
        public Task<bool> AddItemAsync(IEnumerable<SerializedService> item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerializedService>> GetItemAsync(string id)
        {
            using (HttpClient client = new HttpClient(App.ClientHandler))
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Basic",
                                                AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .GetAsync($"orders/{id}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        SerializedOrder order = JsonConvert
                            .DeserializeObject<SerializedOrder>(
                                await response.Content.ReadAsStringAsync());
                        List<SerializedService> currentServices =
                            new List<SerializedService>();
                        foreach (int serviceId in order.Services)
                        {
                            SerializedService service = await DependencyService
                                .Get<IDataStore<SerializedService>>()
                                .GetItemAsync(
                                    serviceId.ToString());
                            currentServices.Add(service);
                        }
                        return currentServices;
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                    }
                }
                catch (Exception ex)
                {
                    await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(ex);
                    Debug.WriteLine(ex);
                };
            }
            return new List<SerializedService>();
        }

        public Task<IEnumerable<IEnumerable<SerializedService>>> GetItemsAsync(
            bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(IEnumerable<SerializedService> item)
        {
            throw new NotImplementedException();
        }
    }
}
