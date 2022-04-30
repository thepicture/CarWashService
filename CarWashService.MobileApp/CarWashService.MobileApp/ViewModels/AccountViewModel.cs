using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
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
            if (IsBusy)
            {
                return;
            }
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
            SerializedUser currentUser = AppIdentity.User;
            currentUser.ImageBytes =
                await UserImageDataStore.GetItemAsync("");
            User = currentUser;
            IsRefreshing = false;
        }

        private Command changePictureCommand;
        private SerializedUser user;

        public AccountViewModel()
        {
            User = AppIdentity.User;
        }

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
            IsBusy = true;
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
            if (imageStream.Length > 500 * 1024)
            {
                await FeedbackService
                    .InformError("Фото аккаунта "
                    + "должно быть в размере"
                    + "не более 500кб.");
                return;
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                if (await UserImageDataStore.UpdateItemAsync(imageBytes))
                {
                    IsRefreshing = true;
                }
            }
            IsBusy = false;
        }
    }
}