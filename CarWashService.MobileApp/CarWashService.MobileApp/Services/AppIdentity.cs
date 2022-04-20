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
        }
        public static SerializedUser User
        {
            get
            {
                if (App.User != null)
                {
                    return App.User;
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
                if (value == null)
                {
                    App.User = null;
                    _ = SecureStorage.Remove("User");
                }
                else
                {
                    value.ImageBytes = null;
                    App.User = value;
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
                if (App.AuthorizationValue != null)
                {
                    return App.AuthorizationValue;
                }
                else
                {
                    return SecureStorage.GetAsync("Identity").Result;
                }
            }
            set
            {
                App.AuthorizationValue = value;
                _ = SecureStorage.SetAsync("Identity", value);
            }
        }
    }
}
