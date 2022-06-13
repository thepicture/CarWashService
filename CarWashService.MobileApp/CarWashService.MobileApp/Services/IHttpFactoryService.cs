using System.Net.Http;

namespace CarWashService.MobileApp.Services
{
    public interface IHttpFactoryService
    {
        HttpClient GetInstance();
    }
}
