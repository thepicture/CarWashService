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
    public class RegistrationDataStore : IDataStore<SerializedUser>
    {
        public async Task<bool> AddItemAsync(SerializedUser item)
        {
            if (item.Id == 0)
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
                    DependencyService
                        .Get<IFeedbackService>()
                        .InformError(validationErrors);
                    return false;
                }
            }

            string jsonIdentity = JsonConvert.SerializeObject(item);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                       .PostAsync(
                        new Uri(client.BaseAddress + "users/register"),
                        new StringContent(jsonIdentity,
                                          Encoding.UTF8,
                                          "application/json"));
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        if (item.Id == 0)
                        {
                            DependencyService
                                .Get<IFeedbackService>()
                                .Inform("Вы зарегистрированы.");
                        }
                        else
                        {
                            DependencyService
                                .Get<IFeedbackService>()
                                .Inform("Фото изменено.");
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        DependencyService
                               .Get<IFeedbackService>()
                               .Inform("Пользователь с таким логином "
                                       + "уже есть.");
                    }
                    else
                    {
                        DependencyService
                          .Get<IFeedbackService>()
                          .InformError(response);
                        Debug.WriteLine(response);
                    }
                    return response.StatusCode == HttpStatusCode.NoContent;
                }
                catch (Exception ex)
                {
                    DependencyService
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

        public Task<SerializedUser> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SerializedUser>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(SerializedUser item)
        {
            throw new NotImplementedException();
        }
    }
}
