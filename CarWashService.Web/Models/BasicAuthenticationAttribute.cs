using CarWashService.Web.Models.AuthenticationModels;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CarWashService.Web.Models
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
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
                if (SimpleAuthenticator.IsAuthenticated(login, password))
                {
                    Thread.CurrentPrincipal =
                        new GenericPrincipal(new GenericIdentity(login),
                                             new string[] { "Admin" });
                }
                else
                {
                    actionContext.Response = actionContext
                  .Request
                  .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
        }
    }
}