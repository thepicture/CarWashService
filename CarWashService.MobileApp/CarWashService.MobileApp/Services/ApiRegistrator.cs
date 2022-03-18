using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class ApiRegistrator : IRegistrator<SerializedUser>
    {
        public async Task<bool> IsRegisteredAsync(SerializedUser identity)
        {
            using (WebClient client = new WebClient())
            {
                string jsonIdentity = JsonConvert.SerializeObject(identity);
                byte[] encodedJsonIdentity = Encoding.UTF8
                    .GetBytes(jsonIdentity);
                client.BaseAddress = (App.Current as App).BaseUrl;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                try
                {
                    byte[] response = await client
                        .UploadDataTaskAsync("api/users/register", encodedJsonIdentity);
                    return true;
                }
                catch (WebException ex)
                {
                    if ((ex.Response as HttpWebResponse).StatusCode
                        == HttpStatusCode.BadRequest)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        throw;
                    }
                    else if ((ex.Response as HttpWebResponse).StatusCode
                        == HttpStatusCode.Conflict)
                    {
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
