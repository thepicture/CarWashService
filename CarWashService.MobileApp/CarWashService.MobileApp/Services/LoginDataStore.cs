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
    public class LoginDataStore : IDataStore<SerializedLoginUser>
    {
        public async Task<bool> AddItemAsync(SerializedLoginUser item)
        {
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(item.Login))
            {
                _ = validationErrors.AppendLine("Введите логин.");
            }
            if (string.IsNullOrWhiteSpace(item.Password))
            {
                _ = validationErrors.AppendLine("Введите пароль.");
            }

            if (validationErrors.Length > 0)
            {
                await DependencyService
                            .Get<IFeedbackService>()
                            .Inform(validationErrors);
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
                        .GetAsync("users/login");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        SerializedLoginUser serializedLoginUser =
                        JsonConvert.DeserializeObject<SerializedLoginUser>(
                            await response.Content.ReadAsStringAsync());
                        App.User = serializedLoginUser;
                        if (item.IsRememberMe)
                        {
                            AppIdentity.User = serializedLoginUser;
                        }
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Вы авторизованы "
                            + $"как {AppIdentity.User.UserTypeName.ToLower()}.");
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        await DependencyService
                          .Get<IFeedbackService>()
                          .InformError("Неверный логин или пароль.");
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

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SerializedLoginUser> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SerializedLoginUser>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(SerializedLoginUser item)
        {
            throw new NotImplementedException();
        }
    }
}
