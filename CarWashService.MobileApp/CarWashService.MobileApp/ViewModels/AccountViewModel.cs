using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.ViewModels;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
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

        internal void OnAppearing()
        {
            IsRefreshing = true;
        }

        private Command exitLoginCommand;

        public SerializedUser User
        {
            get => user;
            set => SetProperty(ref user, value);
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
                AppIdentity.Invalidate();
                AppShell.LoadLoginAndRegisterShell();
            }
        }

        private Command refreshCommand;

        public ICommand RefreshCommand
        {
            get
            {
                if (refreshCommand == null)
                {
                    refreshCommand = new Command(RefreshAsync);
                }

                return refreshCommand;
            }
        }

        private async void RefreshAsync()
        {
            var credentials = new LoginAndPasswordFromBasicDecoder()
                .Decode();
            var authenticator = new ApiAuthenticator();
            string login = credentials[0];
            string password = credentials[1];
            if (await authenticator.IsCorrectAsync(login,
                                                   password))
            {
                User = authenticator.User;
                IsRefreshing = false;
            }
        }

        private Command changePictureCommand;
        private SerializedUser user;

        public ICommand ChangePictureCommand
        {
            get
            {
                if (changePictureCommand == null)
                {
                    changePictureCommand = new Command(ChangePictureAsync);
                }

                return changePictureCommand;
            }
        }

        private async void ChangePictureAsync()
        {
            FileResult result = await MediaPicker
                .PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите фото аккаунта"
                });
            if (result == null)
            {
                return;
            }
            Stream imageStream = await result.OpenReadAsync();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var newUser = AppIdentity.User;
                newUser.ImageBytes = imageBytes;
                if (await RegistrationDataStore.AddItemAsync(newUser))
                {
                    IsRefreshing = true;
                }
            }
        }
    }
}