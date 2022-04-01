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
    public class DiscountDataStore : IDataStore<SerializedDiscount>
    {
        public async Task<bool> AddItemAsync(SerializedDiscount item)
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
                        .PostAsync(new Uri(client.BaseAddress + "servicediscounts"),
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
                        .InformError("Создание акции отменено: " + ex.StackTrace);
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
                        .InformError("Акция уже добавлена: " + ex.StackTrace);
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
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .DeleteAsync($"servicediscounts?id={id}");
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return false;
                }
            }
        }

        public Task<SerializedDiscount> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SerializedDiscount>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(SerializedDiscount item)
        {
            throw new NotImplementedException();
        }
    }
}
