using CarWashService.MobileApp.Models.Serialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
                    Task.Run(() =>
                    {
                        LoadOrdersAsync();
                    });
                }
            }
        }

        private async void LoadOrdersAsync()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Orders.Clear();
            });
            IEnumerable<SerializedOrder> items = await OrderDataStore
                    .GetItemsAsync();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                items = items
                    .Where(s =>
                    {
                        bool isClientApproves = s.ClientFullName.ToLower()
                        .Contains(
                            SearchText.ToLower());
                        bool isSellerApproves = s.SellerFullName != null
                            && s.SellerFullName.ToLower()
                       .Contains(
                           SearchText.ToLower());
                        return isClientApproves || isSellerApproves;
                    });
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (SerializedOrder item in items)
                {
                    Orders.Add(item);
                }
            });
        }

        internal void OnAppearing()
        {
            Orders = new ObservableCollection<SerializedOrder>();
            Task.Run(() =>
            {
                LoadOrdersAsync();
            });
        }

        public ObservableCollection<SerializedOrder> Orders
        {
            get => orders;
            set
            {
                SetProperty(ref orders, value);
            }
        }

        private Command acceptOrderCommand;

        public ICommand AcceptOrderCommand
        {
            get
            {
                if (acceptOrderCommand == null)
                {
                    acceptOrderCommand = new Command(AcceptOrderAsync);
                }

                return acceptOrderCommand;
            }
        }

        private async void AcceptOrderAsync()
        {
        }

        private Command goToOrderPageCommand;
        private ObservableCollection<SerializedOrder> orders;

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

        private async void GoToOrderPageAsync()
        {
        }
    }
}