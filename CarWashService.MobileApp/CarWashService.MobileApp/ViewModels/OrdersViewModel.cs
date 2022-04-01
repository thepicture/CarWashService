using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                    _ = Task.Run(() =>
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
            try
            {
                IEnumerable<SerializedOrder> items = await OrderDataStore
                        .GetItemsAsync();
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    items = items
                        .Where(s =>
                        {
                            return s.ServiceNames.Any(sn =>
                            {
                                return sn.StartsWith(SearchText,
                                                     StringComparison.OrdinalIgnoreCase);
                            });
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                await FeedbackService.Inform("У вас нет интернет " +
                    "подключения. Включён оффлайн-режим.");
            }
        }

        internal void OnAppearing()
        {
            Orders = new ObservableCollection<SerializedOrder>();
            _ = Task.Run(() =>
              {
                  LoadOrdersAsync();
              });
        }

        public ObservableCollection<SerializedOrder> Orders
        {
            get => orders;
            set => _ = SetProperty(ref orders, value);
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

        private async void AcceptOrderAsync(object parameter)
        {
            int orderId = (parameter as SerializedOrder).Id;
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                     new AuthenticationHeaderValue("Basic",
                                                   AppIdentity.AuthorizationValue);
                client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                try
                {
                    HttpResponseMessage response = await client
                        .GetAsync(new Uri(client.BaseAddress + $"orders/{orderId}/confirm"));
                    if (response.StatusCode != HttpStatusCode.Unauthorized)
                    {
                        await FeedbackService
                            .Inform("Заказ успешно подтверждён.");
                        LoadOrdersAsync();
                    }
                    else
                    {
                        await FeedbackService
                            .InformError("Не удалось подтвердить заказ. " +
                            "Обратитесь в службу поддержки.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine(ex.StackTrace);
                    await FeedbackService
                          .Inform("Проверьте подключение к сети " +
                          "и попробуйте ещё раз.");
                }
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
            (App.Current as App).CurrentOrder = parameter as SerializedOrder;
            await Shell.Current.GoToAsync(
                $"{nameof(MakeOrderPage)}");
        }

        private Command<SerializedOrder> deleteOrderCommand;

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
            if (await FeedbackService.Ask("Удалить заказ?"))
            {
                if (await OrderDataStore
                    .DeleteItemAsync(order.Id
                    .ToString()))
                {
                    await FeedbackService.Inform("Заказ удалён.");
                    LoadOrdersAsync();
                }
                else
                {
                    await FeedbackService.InformError("Не удалось удалить заказ.");
                }
            }
        }
    }
}