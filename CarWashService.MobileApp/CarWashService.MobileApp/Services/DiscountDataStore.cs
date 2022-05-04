using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(item.DiscountPercentAsString)
                || !int.TryParse(item.DiscountPercentAsString, out int value)
                || value < 0
                || value > 100)
            {
                _ = validationErrors.AppendLine("Процент - " +
                    "это положительное целое число " +
                    "в диапазоне от 0 до 100.");
            }
            if (item.WorkFromAsDate >= item.WorkToAsDate)
            {
                _ = validationErrors.AppendLine("Дата окончания " +
                    "должна быть позднее даты начала.");
            }
            if (item.WorkFromAsDate == DateTime.MinValue)
            {
                _ = validationErrors.AppendLine("Укажите корректную " +
                    "дату начала.");
            }
            if (item.WorkToAsDate == DateTime.MinValue)
            {
                _ = validationErrors.AppendLine("Укажите корректную " +
                    "дату окончания.");
            }
            if (validationErrors.Length > 0)
            {
                await DependencyService
                    .Get<IFeedbackService>()
                    .InformError(validationErrors);
                return false;
            }
            item.DiscountPercent = int.Parse(item.DiscountPercentAsString);
            using (HttpClient client = new HttpClient(App.ClientHandler))
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    string discountJson = JsonConvert.SerializeObject(item);
                    HttpResponseMessage response = await client
                        .PostAsync("servicediscounts",
                                   new StringContent(discountJson,
                                                     Encoding.UTF8,
                                                     "application/json"));
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        string action = item.Id == 0
                            ? "добавлена"
                            : "изменена";
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform($"Скидка {action}.");
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
                    Debug.WriteLine(ex);
                    await DependencyService
                        .Get<IFeedbackService>()
                        .InformError(ex);
                    return false;
                }
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (!await DependencyService
                            .Get<IFeedbackService>()
                            .Ask("Удалить скидку?"))
            {
                return false;
            }
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
                        .DeleteAsync($"servicediscounts?id={id}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Скидка удалена.");
                    }
                    else
                    {
                        await DependencyService
                           .Get<IFeedbackService>()
                           .InformError(response);
                    }
                    return response.StatusCode == HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    await DependencyService
                        .Get<IFeedbackService>()
                        .InformError(ex);
                    return false;
                }
            }
        }

        public Task<SerializedDiscount> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerializedDiscount>> GetItemsAsync(
            bool forceRefresh = false)
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
                        .GetAsync("serviceDiscounts");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject
                            <IEnumerable<SerializedDiscount>>(
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
                    Debug.WriteLine(ex);
                    await DependencyService
                        .Get<IFeedbackService>()
                        .InformError(ex);
                }
                return new List<SerializedDiscount>();
            }
        }

        public Task<bool> UpdateItemAsync(SerializedDiscount item)
        {
            throw new NotImplementedException();
        }
    }
}
