using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class ToastFeedbackService : IFeedbackService
    {
        public async Task<bool> Ask(string question)
        {
            return await App
                .Current
                .MainPage
                .DisplayAlert("Вопрос", question, "Да", "Нет");
        }

        public async Task Inform(string message)
        {
            await App
            .Current
            .MainPage
            .DisplayAlert("Информация", message, "ОК");
        }

        public async Task InformError(string description)
        {
            await App
           .Current
           .MainPage
           .DisplayAlert("Ошибка", description, "ОК");
        }

        public async Task Warn(string warning)
        {
            await App
            .Current
            .MainPage
            .DisplayAlert("Предупреждение", warning, "ОК");
        }
    }
}
