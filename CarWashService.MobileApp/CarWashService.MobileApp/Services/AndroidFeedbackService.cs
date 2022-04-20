using System.Threading.Tasks;

namespace CarWashService.MobileApp.Services
{
    public class AndroidFeedbackService : IFeedbackService
    {
        public async Task<bool> Ask(object question)
        {
            return await App
                .Current
                .MainPage
                .DisplayAlert("Вопрос",
                              question.ToString(),
                              "Да",
                              "Нет");
        }

        public async Task Inform(object message)
        {
            await App
            .Current
            .MainPage
            .DisplayAlert("Информация",
                          message.ToString(),
                          "ОК");
        }

        public async Task InformError(object description)
        {
            await App
           .Current
           .MainPage
           .DisplayAlert("Ошибка",
                         description.ToString(),
                         "ОК");
        }

        public async Task Warn(object warning)
        {
            await App
            .Current
            .MainPage
            .DisplayAlert("Предупреждение",
                          warning.ToString(),
                          "ОК");
        }
    }
}
