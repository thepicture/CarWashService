using CarWashService.Web.Models.Entities;
using System;
using System.Data.Entity;
using System.Linq;

namespace CarWashService.Web.Models.AuthenticationModels
{
    public class SimpleAuthenticator
    {
        public static bool IsAuthenticated(string login,
                                           string password,
                                           out User user)
        {
            using (CarWashBaseEntities context =
                new CarWashBaseEntities())
            {
                user = context
                    .User
                    .Include(u => u.UserType)
                    .FirstOrDefault(
                        u => u.Login.Equals(login,
                                            StringComparison.OrdinalIgnoreCase)
                              && u.Password == password);
                return user != null;
            }
        }
    }
}