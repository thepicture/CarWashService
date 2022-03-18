using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public interface IAuthenticator
    {
        string Role { get; }
        Task<bool> IsCorrectAsync(string login, string password);
    }
}
