using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Services;
using CarWashService.MobileApp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.ViewModels
{
    public class ServiceDiscountsViewModel : BaseViewModel
    {
        internal void OnAppearing()
        {
            _ = Task.Run(() =>
              {
                  LoadDiscounts();
              });
        }

        private async void LoadDiscounts()
        {
            Discounts = await Task.Run(async () =>
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic",
                                                      AppIdentity.AuthorizationValue);
                    client.BaseAddress = new Uri(App.BaseUrl);
                    try
                    {
                        int serviceId = App.CurrentService.Id;
                        HttpResponseMessage response = await client
                            .GetAsync(new Uri(client.BaseAddress + $"servicediscounts/{serviceId}"));
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                FeedbackService.InformError("Сервер "
                                    + "ответил ошибкой "
                                    + response.StatusCode + ".");
                            });
                            return new List<SerializedDiscount>();
                        }
                        try
                        {
                            return JsonConvert.DeserializeObject
                        <IEnumerable<SerializedDiscount>>(
                                await response.Content.ReadAsStringAsync());
                        }
                        catch (Exception ex)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                FeedbackService.InformError("Не удалось " +
                                "разобрать ответ сервера: " + ex.StackTrace);
                            });
                            return new List<SerializedDiscount>();
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            FeedbackService.InformError("Ошибка запроса: "
                            + ex.StackTrace);
                        });
                        return new List<SerializedDiscount>();
                    }
                    catch (TaskCanceledException ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            FeedbackService.InformError("Запрос отменён: "
                                + ex.StackTrace);
                        });
                        return new List<SerializedDiscount>();
                    }
                    catch (ArgumentNullException ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            FeedbackService
                            .InformError("Запрос был пустой: " + ex.StackTrace);
                        });
                        return new List<SerializedDiscount>();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            FeedbackService
                            .InformError("Акции получены " +
                            "больше одного раза: " + ex.StackTrace);
                        });
                        return new List<SerializedDiscount>();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            FeedbackService
                            .InformError("Неизвестная ошибка: " + ex.StackTrace);
                        });
                        return new List<SerializedDiscount>();
                    }
                }
            });
        }

        private IEnumerable<SerializedDiscount> discounts;

        public IEnumerable<SerializedDiscount> Discounts
        {
            get => discounts;
            set => SetProperty(ref discounts, value);
        }

        private Command goToAddDiscountPage;

        public ICommand GoToAddDiscountPage
        {
            get
            {
                if (goToAddDiscountPage == null)
                {
                    goToAddDiscountPage = new Command(PerformGoToAddDiscountPageAsync);
                }

                return goToAddDiscountPage;
            }
        }

        private async void PerformGoToAddDiscountPageAsync()
        {
            await Shell.Current.GoToAsync(
                $"{nameof(AddDiscountPage)}");
        }

        private Command<SerializedDiscount> deleteDiscountCommand;

        public Command<SerializedDiscount> DeleteDiscountCommand
        {
            get
            {
                if (deleteDiscountCommand == null)
                {
                    deleteDiscountCommand = new Command<SerializedDiscount>
                        (DeleteDiscountAsync);
                }

                return deleteDiscountCommand;
            }
        }

        private async void DeleteDiscountAsync(SerializedDiscount discount)
        {
            if (await FeedbackService.Ask("Удалить скидку?"))
            {
                if (await DiscountDataStore
                    .DeleteItemAsync(discount.Id
                    .ToString()))
                {
                    await FeedbackService.Inform("Скидка удалена.");
                    LoadDiscounts();
                }
                else
                {
                    await FeedbackService.InformError("Не удалось удалить скидку.");
                }
            }
        }

        private Command<SerializedDiscount> goToDiscountPageCommand;

        public Command<SerializedDiscount> GoToDiscountPageCommand
        {
            get
            {
                if (goToDiscountPageCommand == null)
                {
                    goToDiscountPageCommand = new Command<SerializedDiscount>(GoToDiscountPageAsync);
                }

                return goToDiscountPageCommand;
            }
        }

        private async void GoToDiscountPageAsync(SerializedDiscount discount)
        {
            await Shell.Current.Navigation.PushAsync(
                new AddDiscountPage(
                    new AddDiscountViewModel(discount)));
        }
    }
}