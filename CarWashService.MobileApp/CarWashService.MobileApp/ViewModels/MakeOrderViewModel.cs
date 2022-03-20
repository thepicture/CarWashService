using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp
{
    public class MakeOrderViewModel : BaseViewModel
    {
        private DateTime appointmentDate = DateTime.Now;

        public DateTime AppointmentDate
        {
            get => appointmentDate;
            set => SetProperty(ref appointmentDate, value);
        }

        private IEnumerable<SerializedService> servicesOfOrder;

        internal void OnAppearing()
        {
            if ((App.Current as App).CurrentOrder != null)
            {
                Task.Run(async () =>
                {
                    await LoadServicesOfOrderAsync();
                });
            }
            else
            {
                ServicesOfOrder = (App.Current as App).CurrentServices;
                TotalPrice = ServicesOfOrder.Sum(s => s.Price);
            }
            Task.Run(async () =>
            {
                await LoadBranchesAsync();
            });
        }

        private async Task LoadServicesOfOrderAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Basic",
                                                AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    int orderId = (App.Current as App).CurrentOrder.Id;
                    string response = await client
                        .GetAsync($"orders/{orderId}")
                        .Result
                        .Content
                        .ReadAsStringAsync();
                    SerializedOrder order = JsonConvert
                        .DeserializeObject<SerializedOrder>(response);
                    List<SerializedService> currentServices =
                        new List<SerializedService>();
                    foreach (int id in order.Services)
                    {
                        currentServices.Add(
                            await ServiceDataStore.GetItemAsync(id
                                .ToString()));
                    }
                    ServicesOfOrder = currentServices;
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                };
            }
        }

        private async Task LoadBranchesAsync()
        {
            Branches = await BranchDataStore.GetItemsAsync();
        }

        public IEnumerable<SerializedService> ServicesOfOrder
        {
            get => servicesOfOrder;
            set => SetProperty(ref servicesOfOrder, value);
        }

        private Command saveChangesCommand;

        public ICommand SaveChangesCommand
        {
            get
            {
                if (saveChangesCommand == null)
                {
                    saveChangesCommand = new Command(SaveChangesAsync);
                }

                return saveChangesCommand;
            }
        }

        private async void SaveChangesAsync()
        {
            StringBuilder validationErrors = new StringBuilder();

            if (CurrentBranch == null)
            {
                _ = validationErrors.AppendLine("Укажите филиал");
            }
            else if (AppointmentTime < DateTime.Parse(CurrentBranch.WorkFrom).TimeOfDay
              || AppointmentTime > DateTime.Parse(CurrentBranch.WorkTo).TimeOfDay)
            {
                _ = validationErrors.AppendLine("В указанное вами время " +
                    "филиал не работает. Он работает с " +
                    $"{DateTime.Parse(CurrentBranch.WorkFrom).TimeOfDay:hh\\:mm} до " +
                    $"{DateTime.Parse(CurrentBranch.WorkTo).TimeOfDay:hh\\:mm}");
            }
            if (AppointmentDate < DateTime.Now)
            {
                _ = validationErrors.AppendLine("Дата назначения " +
                    "должна быть позднее текущей даты");
            }
            if (validationErrors.Length > 0)
            {
                await FeedbackService.InformError(
                    validationErrors.ToString());
                return;
            }
            SerializedOrder order = new SerializedOrder
            {
                AppointmentDate = (AppointmentDate + AppointmentTime)
                    .ToString(),
                Services = ServicesOfOrder.Select(s => s.Id),
                BranchId = CurrentBranch.Id
            };
            if (await OrderDataStore.AddItemAsync(order))
            {
                await FeedbackService.Inform("Заказ оформлен");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await FeedbackService.Inform("Не удалось " +
                    "оформить заказ. Проверьте подключение к интернету");
            }
        }

        private IEnumerable<SerializedBranch> branches;

        public IEnumerable<SerializedBranch> Branches
        {
            get => branches;
            set => SetProperty(ref branches, value);
        }

        private SerializedBranch currentBranch;

        public SerializedBranch CurrentBranch
        {
            get => currentBranch;
            set => SetProperty(ref currentBranch, value);
        }

        private TimeSpan appointmentTime = TimeSpan.FromHours(12);

        public TimeSpan AppointmentTime
        {
            get => appointmentTime;
            set => SetProperty(ref appointmentTime, value);
        }

        private decimal totalPrice;

        public decimal TotalPrice
        {
            get => totalPrice;
            set => SetProperty(ref totalPrice, value);
        }
    }
}