using CarWashService.MobileApp.Models.Serialized;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public interface IAuthenticator
    {
        SerializedUser User { get; }
        Task<bool> IsCorrectAsync(string login, string password);
    }
}
