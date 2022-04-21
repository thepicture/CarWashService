using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    LoadOrdersAsync();
                }
            }
        }

        private async void LoadOrdersAsync()
        {
            Orders.Clear();
            IEnumerable<SerializedOrder> currentOrders =
                await OrderDataStore.GetItemsAsync();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                currentOrders = currentOrders
                    .Where(s =>
                    s.ServiceNames.Any(sn =>
                        {
                            return sn.StartsWith(SearchText,
                                                 StringComparison.OrdinalIgnoreCase);
                        }));
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (SerializedOrder currentOrder in currentOrders)
                {
                    Orders.Add(currentOrder);
                }
            });
        }

        internal void OnAppearing()
        {
            LoadOrdersAsync();
        }

        public ObservableCollection<SerializedOrder> Orders
        {
            get => orders;
            set => _ = SetProperty(ref orders, value);
        }

        private Command<SerializedOrder> acceptOrderCommand;

        public Command<SerializedOrder> AcceptOrderCommand
        {
            get
            {
                if (acceptOrderCommand == null)
                {
                    acceptOrderCommand =
                        new Command<SerializedOrder>(AcceptOrderAsync);
                }

                return acceptOrderCommand;
            }
        }

        private async void AcceptOrderAsync(SerializedOrder parameter)
        {
            if (await OrderDataStore.UpdateItemAsync(parameter))
            {
                LoadOrdersAsync();
            }
        }

        private ObservableCollection<SerializedOrder> orders;
        private Command goToOrderPageCommand;

        public ICommand GoToOrderPageCommand
        {
            get
            {
                if (goToOrderPageCommand == null)
                {
                    goToOrderPageCommand = new Command(GoToOrderPageAsync);
                }

                return goToOrderPageCommand;
            }
        }

        private async void GoToOrderPageAsync(object parameter)
        {
            App.CurrentOrder = parameter as SerializedOrder;
            await Shell.Current.GoToAsync(
                $"{nameof(MakeOrderPage)}");
        }

        private Command<SerializedOrder> deleteOrderCommand;

        public OrdersViewModel()
        {
            Orders = new ObservableCollection<SerializedOrder>();
        }

        public Command<SerializedOrder> DeleteOrderCommand
        {
            get
            {
                if (deleteOrderCommand == null)
                {
                    deleteOrderCommand = new Command<SerializedOrder>(DeleteOrderAsync);
                }

                return deleteOrderCommand;
            }
        }

        private async void DeleteOrderAsync(SerializedOrder order)
        {
            if (await OrderDataStore
                .DeleteItemAsync(order.Id
                .ToString()))
            {
                LoadOrdersAsync();
            }
        }
    }
}