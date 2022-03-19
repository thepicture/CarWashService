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
        public string Role
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
                                                   encodedLoginAndPassword.Split(' ')[1]);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl + "/");
                try
                {
                    HttpResponseMessage response = await client
                        .GetAsync(new Uri(client.BaseAddress + "users/login"));
                    if (response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        Role = content.Replace("\"", "");
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
