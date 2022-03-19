using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace CarWashService.MobileApp.Services
{
    public class LoginAndPasswordFromBasicDecoder
    {
        public async Task<string[]> DecodeAsync()
        {
            string authorizationValue = await SecureStorage
                .GetAsync("Identity");
            string encodedLoginAndPassword = authorizationValue
                .Split(' ')[1];
            string loginAndPassword = Encoding.UTF8.GetString(
                Convert.FromBase64String(encodedLoginAndPassword));
            return loginAndPassword.Split(':');
        }
    }
}
