using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public interface IListConverter<TInput, TOutput>
    {
        Task<IEnumerable<TOutput>> ConvertAllAsync(IEnumerable<TInput> input);
    }
}
