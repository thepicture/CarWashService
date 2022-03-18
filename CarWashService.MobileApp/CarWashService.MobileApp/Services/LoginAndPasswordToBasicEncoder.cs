using System;
using System.Text;

namespace CarWashService.MobileApp.Services
{
    public class LoginAndPasswordToBasicEncoder
    {
        public string Encode(string login, string password)
        {
            string loginAndPassword = string.Format("{0}:{1}",
                                           login,
                                           password);
            string encodedLoginAndPassword = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(loginAndPassword));
            return encodedLoginAndPassword;
        }
    }
}
