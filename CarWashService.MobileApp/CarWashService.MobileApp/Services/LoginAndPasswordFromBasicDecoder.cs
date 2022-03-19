using System;
using System.Text;

namespace CarWashService.MobileApp.Services
{
    public class LoginAndPasswordFromBasicDecoder
    {
        public string[] Decode()
        {
            string loginAndPassword = Encoding.UTF8.GetString(
                Convert.FromBase64String(AppIdentity.AuthorizationValue));
            return loginAndPassword.Split(':');
        }
    }
}
