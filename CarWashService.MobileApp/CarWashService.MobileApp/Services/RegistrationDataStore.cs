using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Services
{
    public class RegistrationDataStore : IDataStore<SerializedRegistrationUser>
    {
        public async Task<bool> AddItemAsync(SerializedRegistrationUser item)
        {
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(item.FirstName))
            {
                _ = validationErrors.AppendLine("Введите ваше имя.");
            }
            if (string.IsNullOrWhiteSpace(item.LastName))
            {
                _ = validationErrors.AppendLine("Введите вашу фамилию.");
            }
            if (string.IsNullOrWhiteSpace(item.Login))
            {
                _ = validationErrors.AppendLine("Введите логин.");
            }
            if (string.IsNullOrWhiteSpace(item.Password))
            {
                _ = validationErrors.AppendLine("Введите пароль.");
            }
            if (string.IsNullOrWhiteSpace(item.Email)
                || !Regex.IsMatch(item.Email, @"\w+@\w+\.\w{2,}"))
            {
                _ = validationErrors.AppendLine("Укажите почту в " +
                    "формате <aaa>@<bbb>.<cc>.");
            }
            if (string
                .IsNullOrWhiteSpace(item.PassportNumber)
                || !int.TryParse(item.PassportNumber, out _))
            {
                _ = validationErrors.AppendLine("Укажите корректный номер " +
                    "паспорта до 6 цифр.");
            }
            if (string
                .IsNullOrWhiteSpace(item.PassportSeries)
                || !int.TryParse(item.PassportSeries, out _))
            {
                _ = validationErrors.AppendLine("Укажите корректную серию " +
                    "паспорта до 4 цифр.");
            }

            if (item.UserTypeId == 0)
            {
                _ = validationErrors.AppendLine("Укажите тип пользователя.");
            }

            if (validationErrors.Length > 0)
            {
                await DependencyService
                    .Get<IFeedbackService>()
                    .InformError(validationErrors);
                return false;
            }

            string jsonIdentity = JsonConvert.SerializeObject(item);
            using (HttpClient client = new HttpClient(App.ClientHandler))
            {
                client.Timeout = App.HttpClientTimeout;
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                       .PostAsync("users/register",
                        new StringContent(jsonIdentity,
                                          Encoding.UTF8,
                                          "application/json"));
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Вы зарегистрированы.");
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        await DependencyService
                               .Get<IFeedbackService>()
                               .Inform("Пользователь "
                               + "с введёнными логином "
                               + "или почтой"
                               + "уже есть.");
                    }
                    else
                    {
                        await DependencyService
                          .Get<IFeedbackService>()
                          .InformError(response);
                        Debug.WriteLine(response);
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

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<SerializedRegistrationUser> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SerializedRegistrationUser>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(SerializedRegistrationUser item)
        {
            throw new NotImplementedException();
        }
    }
}
