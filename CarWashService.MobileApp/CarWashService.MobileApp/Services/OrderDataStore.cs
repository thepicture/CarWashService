using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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
                client.BaseAddress = new Uri(App.BaseUrl);
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
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<IFeedbackService>()
                            .InformError("Ошибка запроса: " + ex.StackTrace);
                    });
                    return await Task.FromResult(false);
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<IFeedbackService>()
                        .InformError("Транзакция отменена: " + ex.StackTrace);
                    });
                    return await Task.FromResult(false);
                }
                catch (ArgumentNullException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<IFeedbackService>()
                        .InformError("Запрос был пустой: " + ex.StackTrace);
                    });
                    return await Task.FromResult(false);
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<IFeedbackService>()
                        .InformError("Транзакция уже началась: " + ex.StackTrace);
                    });
                    return await Task.FromResult(false);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DependencyService.Get<IFeedbackService>()
                        .InformError("Неизвестная ошибка: " + ex.StackTrace);
                    });
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
                        .DeleteAsync($"orders/{id}");
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return false;
                }
            }
        }

        public Task<SerializedOrder> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerializedOrder>> GetItemsAsync(bool forceRefresh = false)
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
                        .GetAsync("orders")
                        .Result
                        .Content
                        .ReadAsStringAsync();
                    return JsonConvert.DeserializeObject
                        <IEnumerable<SerializedOrder>>(response);
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return null;
                }
            }
        }

        public Task<bool> UpdateItemAsync(SerializedOrder item)
        {
            throw new NotImplementedException();
        }
    }
}
