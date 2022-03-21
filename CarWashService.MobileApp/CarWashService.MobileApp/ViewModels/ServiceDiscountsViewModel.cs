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
                    client.BaseAddress = new Uri((App.Current as App).BaseUrl);
                    try
                    {
                        int serviceId = (App.Current as App).CurrentService.Id;
                        string response = await client
                            .GetAsync(new Uri(client.BaseAddress + $"servicediscounts/{serviceId}"))
                            .Result
                            .Content
                            .ReadAsStringAsync();
                        return JsonConvert.DeserializeObject
                        <IEnumerable<SerializedDiscount>>(response);
                    }
                    catch (HttpRequestException ex)
                    {
                        Debug.WriteLine(ex.StackTrace);
                        return null;
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
    }
}