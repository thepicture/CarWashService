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
    public class OrderDataStore : IDataStore<SerializedOrder>
    {
        public async Task<bool> AddItemAsync(SerializedOrder item)
        {
            StringBuilder validationErrors = new StringBuilder();
            if (item.AppointmentDateTimeAsDateTime.TimeOfDay
                < DateTime.Parse(App.CurrentBranch.WorkFrom).TimeOfDay
              || item.AppointmentDateTimeAsDateTime.TimeOfDay
              > DateTime.Parse(App.CurrentBranch.WorkTo).TimeOfDay)
            {
                _ = validationErrors.AppendLine("В указанное вами время " +
                    "филиал не работает. Он работает с " +
                    $"{DateTime.Parse(App.CurrentBranch.WorkFrom).TimeOfDay:hh\\:mm} до " +
                    $"{DateTime.Parse(App.CurrentBranch.WorkTo).TimeOfDay:hh\\:mm}.");
            }
            if (item.AppointmentDateTimeAsDateTime < DateTime.Now)
            {
                _ = validationErrors.AppendLine("Дата назначения " +
                    "должна быть позднее текущей даты.");
            }
            if (validationErrors.Length > 0)
            {
                await DependencyService
                    .Get<IFeedbackService>()
                    .InformError(validationErrors);
                return false;
            }
            item.AppointmentDate = item.AppointmentDateTimeAsDateTime.ToString();
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    string orderJson = JsonConvert.SerializeObject(item);
                    HttpResponseMessage response = await client
                        .PostAsync("orders",
                                   new StringContent(orderJson,
                                                     Encoding.UTF8,
                                                     "application/json"));
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Заказ оформлен.");
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                        Debug.WriteLine(response);
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
                    .Ask("Удалить заказ?"))
            {
                return false;
            }
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .DeleteAsync($"orders/{id}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Заказ удалён.");
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                        Debug.WriteLine(response);
                    }
                    return response.StatusCode == HttpStatusCode.OK;
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

        public Task<SerializedOrder> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerializedOrder>> GetItemsAsync(
            bool forceRefresh = false)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .GetAsync("orders");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert
                            .DeserializeObject<IEnumerable<SerializedOrder>>(
                            await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                        Debug.WriteLine(response);
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
            return new List<SerializedOrder>();
        }

        public async Task<bool> UpdateItemAsync(SerializedOrder item)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = App.HttpClientTimeout;
                client.DefaultRequestHeaders.Authorization =
                     new AuthenticationHeaderValue("Basic",
                                                   AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .GetAsync($"orders/{item.Id}/confirm");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Заказ подтверждён.");
                        return true;
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                        Debug.WriteLine(response);
                    }
                    return response.StatusCode == HttpStatusCode.OK;
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
    }
}
