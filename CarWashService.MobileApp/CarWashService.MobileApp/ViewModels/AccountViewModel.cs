﻿using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.ViewModels;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public class AccountViewModel : BaseViewModel
    {
        private string login;

        public string Login
        {
            get => login;
            set => SetProperty(ref login, value);
        }

        private Command exitLoginCommand;

        public AccountViewModel()
        {
            _ = Task.Run(() =>
              {
                  string[] loginAndPassword = new LoginAndPasswordFromBasicDecoder()
                      .Decode();
                  Login = loginAndPassword[0];
              });
        }

        public ICommand ExitLoginCommand
        {
            get
            {
                if (exitLoginCommand == null)
                {
                    exitLoginCommand = new Command(ExitLoginAsync);
                }

                return exitLoginCommand;
            }
        }

        private async void ExitLoginAsync()
        {
            if (await FeedbackService.Ask("Выйти из аккаунта?"))
            {
                AppIdentity.AuthorizationValue = null;
                AppIdentity.Role = null;
                (AppShell.Current as AppShell).LoadLoginAndRegisterShell();
            }
        }
    }
}