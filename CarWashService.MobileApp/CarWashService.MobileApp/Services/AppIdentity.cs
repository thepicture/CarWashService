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
                    _ = SecureStorage.Remove("Role");
                }
                else
                {
                    _ = SecureStorage.SetAsync("Role", value);
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
                    _ = SecureStorage.Remove("Identity");
                }
                else
                {
                    _ = SecureStorage.SetAsync("Identity", value);
                }
            }
        }
    }
}
