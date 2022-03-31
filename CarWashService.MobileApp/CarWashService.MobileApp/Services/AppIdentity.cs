using CarWashService.MobileApp.Models.Serialized;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace CarWashService.MobileApp.Services
{
    public static class AppIdentity
    {
        public static void Invalidate()
        {
            SecureStorage.RemoveAll();
            AppIdentity.AuthorizationValue = null;
            AppIdentity.User = null;
        }
        public static SerializedUser User
        {
            get
            {
                if ((App.Current as App).User != null)
                {
                    return (App.Current as App).User;
                }
                else
                {
                    return JsonConvert
                        .DeserializeObject<SerializedUser>(
                            SecureStorage.GetAsync("User").Result);
                }
            }
            set
            {
                (App.Current as App).User = value;
                if (value == null)
                {
                    _ = SecureStorage.Remove("User");
                }
                else
                {
                    _ = SecureStorage
                        .SetAsync("User",
                                  JsonConvert.SerializeObject(value));
                }
            }
        }
        public static string Role => User.UserTypeName;
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
