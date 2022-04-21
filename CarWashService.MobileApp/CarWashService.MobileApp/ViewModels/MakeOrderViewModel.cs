using CarWashService.MobileApp.Models.Serialized;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class MakeOrderViewModel : BaseViewModel
    {
        private bool isNew;

        private ObservableCollection<SerializedService> servicesOfOrder;

        internal void OnAppearing()
        {
            IsNew = App.CurrentOrder == null;
            if (App.CurrentOrder != null)
            {
                _ = Task.Run(() =>
                {
                    _ = LoadServicesOfOrderAsync();
                })
                    .ContinueWith((task) =>
                {
                    TotalPrice = ServicesOfOrder.Sum(s => s.Price);
                    AppointmentDateTime = DateTime.Parse(
                        App.CurrentOrder.AppointmentDate);
                });
            }
            else
            {
                foreach (SerializedService serviceOfOrder in App.CurrentServices)
                {
                    ServicesOfOrder.Add(serviceOfOrder);
                }
                TotalPrice = ServicesOfOrder.Sum(s => s.Price);
            }
        }

        private async Task LoadServicesOfOrderAsync()
        {
            ServicesOfOrder.Clear();
            IEnumerable<SerializedService> currentServicesOfOrder =
                await OrderServicesDataStore
                .GetItemAsync(
                    App.CurrentOrder.Id.ToString());
            foreach (SerializedService serviceOfOrder in currentServicesOfOrder)
            {
                servicesOfOrder.Add(serviceOfOrder);
            }
        }

        public ObservableCollection<SerializedService> ServicesOfOrder
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
            SerializedOrder order = new SerializedOrder
            {
                AppointmentDateTimeAsDateTime = AppointmentDateTime,
                Services = ServicesOfOrder.Select(s => s.Id),
                BranchId = CurrentBranch.Id
            };
            if (await OrderDataStore.AddItemAsync(order))
            {
                await Shell.Current.GoToAsync("..");
            }
        }

        private IEnumerable<SerializedBranch> branches;

        public IEnumerable<SerializedBranch> Branches
        {
            get => branches;
            set => SetProperty(ref branches, value);
        }

        public SerializedBranch CurrentBranch => App.CurrentBranch;

        private DateTime appointmentDateTime = DateTime.Now.AddHours(1);

        public DateTime AppointmentDateTime
        {
            get => appointmentDateTime;
            set => SetProperty(ref appointmentDateTime, value);
        }

        private decimal totalPrice;

        public decimal TotalPrice
        {
            get => totalPrice;
            set => SetProperty(ref totalPrice, value);
        }
        public bool IsNew
        {
            get => isNew;
            set => SetProperty(ref isNew, value);
        }

        private Command deleteOrderCommand;

        public MakeOrderViewModel()
        {
            ServicesOfOrder = new ObservableCollection<SerializedService>();
        }

        public ICommand DeleteOrderCommand
        {
            get
            {
                if (deleteOrderCommand == null)
                {
                    deleteOrderCommand = new Command(DeleteOrderAsync);
                }

                return deleteOrderCommand;
            }
        }

        private async void DeleteOrderAsync()
        {
            if (await OrderDataStore
                .DeleteItemAsync(App
                .CurrentOrder
                .Id
                .ToString()))
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}