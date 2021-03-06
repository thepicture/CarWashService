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
        public override bool IsRefreshing
        {
            get => base.IsRefreshing;
            set
            {
                base.IsRefreshing = value;
                if (value && !IsBusy)
                {
                    IsRefreshing = false;
                }
            }
        }

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
            IsRefreshing = true;
            SerializedRegistrationUser identity =
                new SerializedRegistrationUser
                {
                    Login = Login,
                    Password = Password,
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Patronymic = Patronymic,
                    UserTypeId = UserType?.Id ?? 0,
                    ImageBytes = CompressedImageBytes
                };
            if (IsCustomer)
            {
                identity.UserTypeId = 2;
            }

            if (await RegistrationDataStore.AddItemAsync(identity))
            {
                AppShell.LoadLoginAndRegisterShell();
            }
            IsRefreshing = false;
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

        private Command enterEmployeeCodeCommand;

        public ICommand EnterEmployeeCodeCommand
        {
            get
            {
                if (enterEmployeeCodeCommand == null)
                    enterEmployeeCodeCommand = new Command(EnterEmployeeCodeAsync);

                return enterEmployeeCodeCommand;
            }
        }

        private async void EnterEmployeeCodeAsync()
        {
            string enteredEmployeeCode = await App.Current.MainPage.DisplayPromptAsync(
                        "Ввести код персонала",
                        "Для обеспечения регистрации как сотрудник или администратор " +
                        "введите код персонала.",
                        keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(enteredEmployeeCode))
            {
                return;
            }
            if (string.Equals(enteredEmployeeCode, App.EmployeeCode))
            {
                IsCustomer = false;
                await FeedbackService.Inform("Вы можете изменить тип должности.");
            }
            else
            {
                await FeedbackService.InformError("Неверный код.");
            }
        }

        private bool isCustomer = true;

        public bool IsCustomer
        {
            get => isCustomer;
            set => SetProperty(ref isCustomer, value);
        }
    }
}