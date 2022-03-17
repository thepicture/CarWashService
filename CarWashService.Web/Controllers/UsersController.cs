using CarWashService.Web.Models.Entities;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace CarWashService.Web.Controllers
{
    public class UsersController : ApiController
    {
        private CarWashBaseEntities db = new CarWashBaseEntities();


        [ResponseType(typeof(User))]
        [Route("api/users/register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(User user)
        {
            if (user == null)
            {
                ModelState.AddModelError("User", "user can't be null");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(user.Login))
                {
                    ModelState.AddModelError("Login", "login must be provided");
                }

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    ModelState.AddModelError("Login", "email must be provided");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (await db
                .User
                .AnyAsync(u =>
                    u.Login.ToLower()
                    == user.Login.ToLower()))
            {
                return Conflict();
            }

            if (await db
              .User
              .AnyAsync(u =>
                  u.Email.ToLower()
                  == user.Email.ToLower()))
            {
                return Conflict();
            }

            db.User.Add(user);
            await db.SaveChangesAsync();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("api/users/login")]
        public IHttpActionResult Login()
        {
            var identity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            string role = identity
                .FindFirst(ClaimTypes.Role)
                .Value;
            return Ok(role);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}