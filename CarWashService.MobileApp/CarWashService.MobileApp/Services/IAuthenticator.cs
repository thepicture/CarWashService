using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public interface IAuthenticator
    {
        Task<bool> IsCorrectAsync(string login, string password);
    }
}
