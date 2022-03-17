using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class ApiAuthenticator : IAuthenticator
    {
        public async Task<bool> IsCorrectAsync(string login, string password)
        {
            string loginAndPassword = string.Format("{0}:{1}",
                                                    login,
                                                    password);
            string encodedLoginAndPassword = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(loginAndPassword));
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic",
                                                  encodedLoginAndPassword);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    HttpResponseMessage response =
                         await client
                        .PostAsync($"/users/login", null);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
