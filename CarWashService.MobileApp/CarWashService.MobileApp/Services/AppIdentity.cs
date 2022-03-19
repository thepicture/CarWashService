using Xamarin.Essentials;

namespace CarWashService.MobileApp.Services
{
    public static class AppIdentity
    {
        public static string Role
        {
            get
            {
                if ((App.Current as App).Role != null)
                {
                    return (App.Current as App).Role;
                }
                else
                {
                    return SecureStorage.GetAsync("Role").Result;
                }
            }
            set
            {
                (App.Current as App).Role = value;
                if (value == null)
                {
                    SecureStorage.Remove("Role");
                }
                else
                {
                    SecureStorage.SetAsync("Role", value);
                }
            }
        }
        public static string AuthorizationValue
        {
            get
            {
                if ((App.Current as App).Identity != null)
                {
                    return (App.Current as App).Identity;
                }
                else
                {
                    return SecureStorage.GetAsync("Identity").Result;
                }
            }
            set
            {
                (App.Current as App).Identity = value;
                if (value == null)
                {
                    SecureStorage.Remove("Identity");
                }
                else
                {
                    SecureStorage.SetAsync("Identity", value);
                }
            }
        }
    }
}
