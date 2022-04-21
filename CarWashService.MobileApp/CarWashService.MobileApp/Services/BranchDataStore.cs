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
    public class BranchDataStore : IDataStore<SerializedBranch>
    {
        public async Task<bool> AddItemAsync(SerializedBranch item)
        {
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(item.Title))
            {
                _ = validationErrors.AppendLine("Введите наименование " +
                    "филиала.");
            }
            if (string.IsNullOrWhiteSpace(item.CityName))
            {
                _ = validationErrors.AppendLine("Введите город.");
            }
            if (string.IsNullOrWhiteSpace(item.StreetName))
            {
                _ = validationErrors.AppendLine("Введите название улицы.");
            }
            if (item.WorkFromAsDate >= item.WorkToAsDate)
            {
                _ = validationErrors.AppendLine("Время " +
                    "начала работы должно быть " +
                    "раньше времени окончания работы.");
            }
            if (item.PhoneNumbers.Count == 0)
            {
                _ = validationErrors.AppendLine("Укажите хотя бы один контакт.");
            }

            if (validationErrors.Length > 0)
            {
                await DependencyService
                    .Get<IFeedbackService>()
                    .InformError(validationErrors);
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
                    string branchJson = JsonConvert.SerializeObject(item);
                    HttpResponseMessage response = await client
                        .PostAsync("branches",
                                   new StringContent(branchJson,
                                                     Encoding.UTF8,
                                                     "application/json"));
                    string content = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        string action = item.Id == 0
                            ? "добавлен"
                            : "обновлён";
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform($"Филиал {action}.");
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

        public async Task<bool> UpdateItemAsync(SerializedBranch item)
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (!await DependencyService
                .Get<IFeedbackService>()
                .Ask("Удалить филиал? "
                     + "Вместе с ним "
                     + "будут удалены "
                     + "связанные заказы."))
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
                        .DeleteAsync($"branches/{id}");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Филиал удалён.");
                    }
                    else
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .InformError(response);
                    }
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    return false;
                }
            }
        }

        public Task<SerializedBranch> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SerializedBranch>> GetItemsAsync
            (bool forceRefresh = false)
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
                        .GetAsync("branches");
                    string content = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert
                            .DeserializeObject<IEnumerable<SerializedBranch>>(content);
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
            return new List<SerializedBranch>();
        }
    }
}