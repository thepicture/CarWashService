using CarWashService.Web.Models.Entities;
using System.Linq;

namespace CarWashService.Web.Models.AuthenticationModels
{
    public class SimpleAuthenticator
    {
        public static bool IsAuthenticated(string login, string password)
        {
            using (CarWashBaseEntities context =
                new CarWashBaseEntities())
            {
                return context
                    .User
                    .Any(u => u.Login.Equals(login,
                                             System.StringComparison.OrdinalIgnoreCase)
                              && u.Password == password);
            }
        }
    }
}