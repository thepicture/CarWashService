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
    public class UserImageDataStore : IDataStore<byte[]>
    {
        public Task<bool> AddItemAsync(byte[] item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetItemAsync(string id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic",
                        AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                       .GetAsync("users/image");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        try
                        {
                            byte[] imageBytes =
                                JsonConvert.DeserializeObject<byte[]>(
                                    await response.Content.ReadAsStringAsync()
                                    );
                            return imageBytes;
                        }
                        catch (Exception ex)
                        {
                            _ = DependencyService
                                .Get<IFeedbackService>()
                                .InformError("Не удалось "
                                + "десереализовать фото: "
                                + response);
                            Debug.WriteLine(ex);
                        }
                    }
                    else
                    {
                        _ = DependencyService
                          .Get<IFeedbackService>()
                          .InformError(response);
                        Debug.WriteLine(response);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    _ = DependencyService
                         .Get<IFeedbackService>()
                         .InformError(ex);
                    Debug.WriteLine(ex);
                    return null;
                }
            }
        }

        public Task<IEnumerable<byte[]>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateItemAsync(byte[] item)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic",
                        AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpRequestMessage request = new HttpRequestMessage(
                        new HttpMethod("PATCH"), "users/image")
                    {
                        Content = new StringContent(
                            Convert.ToBase64String(item),
                            Encoding.UTF8,
                            "application/json")
                    };
                    HttpResponseMessage response = await client
                       .SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        await DependencyService
                            .Get<IFeedbackService>()
                            .Inform("Фото изменено.");
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
    }
}
