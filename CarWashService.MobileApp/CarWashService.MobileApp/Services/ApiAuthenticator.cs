using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class ApiAuthenticator : IAuthenticator
    {
        public SerializedUser User
        {
            get;
            private set;
        }

        public async Task<bool> IsCorrectAsync(string login, string password)
        {
            string encodedLoginAndPassword =
                new LoginAndPasswordToBasicEncoder()
                .Encode(login, password);
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                     new AuthenticationHeaderValue("Basic",
                                                   encodedLoginAndPassword);
                client.BaseAddress = new Uri(App.BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .GetAsync(new Uri(client.BaseAddress + "users/login"));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        User = JsonConvert.DeserializeObject<SerializedUser>(content);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }
    }
}
