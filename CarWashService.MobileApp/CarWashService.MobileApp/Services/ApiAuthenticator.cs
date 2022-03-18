using System.Diagnostics;
using System.Net;
using System.Text;
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
            using (WebClient client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.Authorization,
                                   encodedLoginAndPassword);
                client.BaseAddress = (App.Current as App).BaseUrl;
                try
                {
                    byte[] response = await client
                        .DownloadDataTaskAsync("api/users/login");
                    Role = Encoding.UTF8
                        .GetString(response)
                        .Replace("\"", "");
                    return true;
                }
                catch (WebException ex)
                {
                    if ((ex.Response as HttpWebResponse).StatusCode
                        != HttpStatusCode.Unauthorized)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        throw;
                    }
                    return false;
                }
            }
        }
    }
}
