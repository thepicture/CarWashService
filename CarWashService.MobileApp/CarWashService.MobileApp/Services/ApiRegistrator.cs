using CarWashService.MobileApp.Models.Serialized;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class ApiRegistrator : IRegistrator<SerializedUser>
    {
        public async Task<bool> IsRegisteredAsync(SerializedUser identity)
        {
            return await Task.FromResult(false);
        }
    }
}
