using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public IRegistrator<SerializedUser> Registrator =>
            DependencyService.Get<IRegistrator<SerializedUser>>();
        private UserTypeHelper userType;

        public UserTypeHelper UserType
        {
            get => userType;
            set => SetProperty(ref userType, value);
        }


        private string firstName;

        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }

        private string lastName;

        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }

        private string patronymic;

        public string Patronymic { get => patronymic; set => SetProperty(ref patronymic, value); }

        private string password;

        public string Password { get => password; set => SetProperty(ref password, value); }

        private string passportNumber;

        public string PassportNumber { get => passportNumber; set => SetProperty(ref passportNumber, value); }

        private string passportSeries;

        public string PassportSeries { get => passportSeries; set => SetProperty(ref passportSeries, value); }

        private Command registerCommand;

        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                {
                    registerCommand = new Command(RegisterAsync);
                }

                return registerCommand;
            }
        }

        private async void RegisterAsync()
        {
            StringBuilder validationErrors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                validationErrors.AppendLine("Введите ваше имя");
            }
            if (string.IsNullOrWhiteSpace(LastName))
            {
                validationErrors.AppendLine("Введите вашу фамилию");
            }
            if (string.IsNullOrWhiteSpace(Login))
            {
                validationErrors.AppendLine("Введите логин");
            }
            if (string.IsNullOrWhiteSpace(Password))
            {
                validationErrors.AppendLine("Введите пароль");
            }
            if (string
                .IsNullOrWhiteSpace(PassportNumber)
                || !int.TryParse(PassportNumber, out _))
            {
                validationErrors.AppendLine("Укажите корректный номер " +
                    "паспорта до 6 цифр");
            }
            if (string
                .IsNullOrWhiteSpace(PassportSeries)
                || !int.TryParse(PassportSeries, out _))
            {
                validationErrors.AppendLine("Укажите корректную серию " +
                    "паспорта до 4 цифр");
            }

            if (UserType == null)
            {
                validationErrors.AppendLine("Укажите тип пользователя");
            }

            if (validationErrors.Length > 0)
            {
                await FeedbackService.InformError(
                    validationErrors.ToString());
                return;
            }

            var identity = new SerializedUser
            {
                Login = Login,
                Password = Password,
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Patronymic = Patronymic,
                PassportNumber = PassportNumber,
                PassportSeries = PassportSeries,
                UserTypeId = userType.Id
            };

            bool isRegistered = false;
            try
            {
                isRegistered = await Registrator
                .IsRegisteredAsync(identity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                await FeedbackService.Inform("Подключение к интернету " +
                     "отсутствует, проверьте подключение " +
                     "и попробуйте ещё раз");
                return;
            }
            if (isRegistered)
            {
                await FeedbackService.Inform("Вы зарегистрированы");
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                await FeedbackService.InformError("Не удалось " +
                    "зарегистрировать. " +
                    "Вероятно, политика компании изменилась. " +
                    "Обратитесь к системному администратору");
            }
        }

        private string login;

        public RegisterViewModel()
        {
            UserTypes = new List<UserTypeHelper>
            {
                new UserTypeHelper
                {
                    Id = 1,
                    Name = "Администратор",
                },
                  new UserTypeHelper
                {
                    Id = 2,
                    Name = "Клиент",
                },
                    new UserTypeHelper
                {
                    Id = 3,
                    Name = "Сотрудник",
                }
            };
            UserType = UserTypes.First();
        }

        public string Login { get => login; set => SetProperty(ref login, value); }

        private string email;

        public string Email { get => email; set => SetProperty(ref email, value); }

        private IEnumerable<UserTypeHelper> userTypes;

        public IEnumerable<UserTypeHelper> UserTypes
        {
            get => userTypes;
            set => SetProperty(ref userTypes, value);
        }
    }
}