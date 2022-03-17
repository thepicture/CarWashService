using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {

        private Command userType;

        public ICommand UserType
        {
            get
            {
                if (userType == null)
                {
                    userType = new Command(PerformUserType);
                }

                return userType;
            }
        }

        private void PerformUserType()
        {
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
                    registerCommand = new Command(Register);
                }

                return registerCommand;
            }
        }

        private void Register()
        {
        }

        private string login;

        public RegisterViewModel()
        {
        }

        public string Login { get => login; set => SetProperty(ref login, value); }
    }
}