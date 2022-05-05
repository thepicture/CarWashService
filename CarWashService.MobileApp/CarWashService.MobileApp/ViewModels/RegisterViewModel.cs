using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using System.IO;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private UserTypeHelper userType;

        public UserTypeHelper UserType
        {
            get => userType;
            set => SetProperty(ref userType, value);
        }

        private string firstName;

        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        private string lastName;

        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        private string patronymic;

        public string Patronymic
        {
            get => patronymic;
            set => SetProperty(ref patronymic, value);
        }

        private string password;

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private string passportNumber;

        public string PassportNumber
        {
            get => passportNumber;
            set => SetProperty(ref passportNumber, value);
        }

        private string passportSeries;

        public string PassportSeries
        {
            get => passportSeries;
            set => SetProperty(ref passportSeries, value);
        }

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
            IsBusy = true;
            SerializedRegistrationUser identity =
                new SerializedRegistrationUser
                {
                    Login = Login,
                    Password = Password,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Patronymic = Patronymic,
                    PassportNumber = PassportNumber,
                    PassportSeries = PassportSeries,
                    UserTypeId = UserType?.Id ?? 0,
                    ImageBytes = CompressedImageBytes
                };

            if (await RegistrationDataStore.AddItemAsync(identity))
            {
                AppShell.LoadLoginAndRegisterShell();
            }
            IsBusy = false;
        }

        private string login;

        public string Login
        {
            get => login;
            set => SetProperty(ref login, value);
        }

        private string email;

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        private ImageSource accountImage;

        public ImageSource AccountImage
        {
            get => accountImage;
            set => SetProperty(ref accountImage, value);
        }

        private Command selectImageCommand;
        private byte[] imageBytes;
        private byte[] compressedImageBytes;

        public ICommand SelectImageCommand
        {
            get
            {
                if (selectImageCommand == null)
                {
                    selectImageCommand = new Command(SelectImageAsync);
                }

                return selectImageCommand;
            }
        }

        public byte[] ImageBytes
        {
            get => imageBytes;
            set => SetProperty(ref imageBytes, value);
        }
        public byte[] CompressedImageBytes
        {
            get => compressedImageBytes;
            set => SetProperty(ref compressedImageBytes, value);
        }

        private async void SelectImageAsync()
        {
            FileResult result = await MediaPicker
               .PickPhotoAsync(new MediaPickerOptions
               {
                   Title = "Выберите фото аккаунта",
               });
            if (result == null)
            {
                return;
            }
            using (Stream imageStream = await result.OpenReadAsync())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await imageStream.CopyToAsync(memoryStream);
                    ImageBytes = memoryStream.ToArray();
                    CompressedImageBytes = ImageResizer
                        .ResizeImage(ImageBytes,
                                     App.DefaultImageWidth,
                                     App.DefaultImageHeight,
                                     App.DefaultQuality);
                }
            }
            AccountImage = ImageSource.FromStream(() =>
            {
                return new MemoryStream(ImageBytes);
            });
        }
    }
}