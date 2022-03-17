using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public interface IFeedbackService
    {
        Task Inform(string message);
        Task<bool> Ask(string question);
        Task Warn(string warning);
        Task InformError(string description);
    }
}
