using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class ApiRegistrator : IRegistrator<SerializedUser>
    {
        public async Task<bool> IsRegisteredAsync(SerializedUser identity)
        {
            string jsonIdentity = JsonConvert.SerializeObject(identity);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                       .PostAsync(new Uri(client.BaseAddress + "users/register"),
                                  new StringContent(jsonIdentity,
                                                    Encoding.UTF8,
                                                    "application/json"));
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        return false;
                    }
                    if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        return false;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        return false;
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return true;
                    }
                    else
                    {
                        throw new HttpRequestException();
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
