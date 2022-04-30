using CarWashService.Web.Models.Entities;
using System;
using System.Linq;

namespace CarWashService.Web.Models.AuthenticationModels
{
    public class SimpleAuthenticator
    {
        public static bool IsAuthenticated(string login,
                                           string password,
                                           out string[] role)
        {
            using (CarWashBaseEntities context =
                new CarWashBaseEntities())
            {
                role = context
                    .User
                    .Where(u => u.Login.Equals(login,
                                            StringComparison.OrdinalIgnoreCase)
                              && u.Password == password)
                    .Select(u => u.UserType.Name)
                    .ToArray();
                return role.Count() == 1;
            }
        }
    }
}