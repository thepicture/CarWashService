using CarWashService.Web.Models.Entities;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CarWashService.Web.Models.AuthenticationModels
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext
                .ActionDescriptor
                .GetCustomAttributes<AllowAnonymousAttribute>()
                .Any())
            {
                return;
            }
            if (actionContext
                .Request
                .Headers
                .Authorization == null)
            {
                actionContext.Response = actionContext
                    .Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                if (actionContext
                    .Request
                    .Headers
                    .Authorization
                    .Scheme != "Basic")
                {
                    actionContext.Response = actionContext
                 .Request
                 .CreateResponse(HttpStatusCode.Unauthorized);
                }
                string authenticationToken = actionContext
                    .Request
                    .Headers
                    .Authorization
                    .Parameter;
                string decodedAuthenticationString = Encoding
                    .UTF8
                    .GetString(
                    Convert.FromBase64String(authenticationToken)
                    );
                string[] loginAndPassword = decodedAuthenticationString
                    .Split(':');
                string login = loginAndPassword[0];
                string password = loginAndPassword[1];
                if (SimpleAuthenticator.IsAuthenticated(login,
                                                        password,
                                                        out User user))
                {
                    GenericIdentity identity = new GenericIdentity(login);
                    Claim roleClaim = new Claim(ClaimTypes.Role,
                                                user.UserType.Name);
                    identity.AddClaim(roleClaim);
                    Thread.CurrentPrincipal =
                        new GenericPrincipal(identity,
                                             new string[]
                                             {
                                                 user.UserType.Name
                                             });
                    if (HttpContext.Current.User != null)
                    {
                        HttpContext.Current.User = Thread.CurrentPrincipal;
                    }
                }
                else
                {
                    actionContext.Response = actionContext
                  .Request
                  .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            base.OnAuthorization(actionContext);
        }
    }
}