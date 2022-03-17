using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public interface IRegistrator<TIdentity>
    {
        Task<bool> IsRegisteredAsync(TIdentity identity);
    }
}
