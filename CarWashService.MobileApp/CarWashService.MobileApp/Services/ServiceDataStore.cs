using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Services
{
    public class ServiceDataStore : IDataStore<SerializedService>
    {
        public async Task<bool> AddItemAsync(SerializedService item)
        {
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                _ = validationErrors.AppendLine("Введите наименование " +
                    "услуги.");
            }
            if (string.IsNullOrWhiteSpace(item.PriceString)
                || !int.TryParse(item.PriceString, out _)
                || int.Parse(item.PriceString) <= 0)
            {
                _ = validationErrors.AppendLine("Стоимость - " +
                    "это положительное целое число в рублях.");
            }
            if (item.ServiceTypes.FirstOrDefault() == null)
            {
                _ = validationErrors.AppendLine("Выберите тип услуги.");
            }

            if (validationErrors.Length > 0)
            {
                await DependencyService
                    .Get<IFeedbackService>()
                    .InformError(validationErrors);
                return false;
            }
            item.Price = int.Parse(item.PriceString);
            using (HttpClient client = DependencyService.Get<IHttpFactoryService>().GetInstance())
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    string serviceJson = JsonConvert.SerializeObject(item);
                    HttpResponseMessage response = await client
                        .PostAsync("services",
                                   new StringContent(serviceJson,
                                                     Encoding.UTF8,
                                                     "application/json"));
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        string action = item.Id == 0
                            ? "добавлена"
                            : "изменена";
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform($"Услуга { action}.");
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                    }
                    return response.StatusCode == HttpStatusCode.Created;
                }
                catch (Exception ex)
                {
                    await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(ex);
                    Debug.WriteLine(ex);
                    return false;
                }
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (!await DependencyService
                .Get<IFeedbackService>()
                .Ask("Удалить услугу?"))
            {
                return false;
            }
            using (HttpClient client = DependencyService.Get<IHttpFactoryService>().GetInstance())
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .DeleteAsync($"services?id={id}");
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Услуга удалена.");
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                    }
                    return response.StatusCode == HttpStatusCode.NoContent;
                }
                catch (Exception ex)
                {
                    await DependencyService
                           .Get<IFeedbackService>()
                           .InformError(ex);
                    Debug.WriteLine(ex);
                    return false;
                }
            }
        }

        public async Task<SerializedService> GetItemAsync(string id)
        {
            try
            {
                using (HttpClient client = DependencyService.Get<IHttpFactoryService>().GetInstance())
                {
                    client.Timeout = App.HttpClientTimeout;
                    client.DefaultRequestHeaders.Authorization =
                      new AuthenticationHeaderValue("Basic",
                                                    AppIdentity.AuthorizationValue);
                    client.BaseAddress = new Uri(App.BaseUrl);
                    HttpResponseMessage response = await client
                      .GetAsync($"services/{id}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert
                            .DeserializeObject<SerializedService>(
                                await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                    }
                }
            }
            catch (Exception ex)
            {
                await DependencyService
                       .Get<IFeedbackService>()
                       .InformError(ex);
                Debug.WriteLine(ex);
            }
            return null;
        }

        public async Task<IEnumerable<SerializedService>> GetItemsAsync
        (bool forceRefresh = false)
        {
            using (HttpClient client = DependencyService.Get<IHttpFactoryService>().GetInstance())
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                        AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .GetAsync("services");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert
                            .DeserializeObject<IEnumerable<SerializedService>>(
                                await response.Content.ReadAsStringAsync());
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
                }
            }
            return new List<SerializedService>();
        }

        public Task<bool> UpdateItemAsync(SerializedService item)
        {
            throw new NotImplementedException();
        }
    }
}
