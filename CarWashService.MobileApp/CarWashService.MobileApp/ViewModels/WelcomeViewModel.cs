using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {

        private Command continueCommand;

        public ICommand ContinueCommand
        {
            get
            {
                if (continueCommand == null)
                    continueCommand = new Command(Continue);

                return continueCommand;
            }
        }

        private void Continue()
        {
            AppShell.LoadLoginAndRegisterShell();
        }
    }
}