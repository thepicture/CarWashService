using CarWashService.Web.Models.Entities;
using CarWashService.Web.Models.Entities.Serialized;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace CarWashService.Web.Controllers
{
    public class UsersController : ApiController
    {
        private readonly CarWashBaseEntities db = new CarWashBaseEntities();


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
                    ModelState.AddModelError("Email", "email must be provided");
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
            _ = db.User.Add(user);

            _ = await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/users/image")]
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        [HttpPatch]
        public async Task<IHttpActionResult> PatchImageAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User userFromDatabase = await db.User.FirstAsync(u =>
                u.Login == HttpContext.Current.User.Identity.Name);
            userFromDatabase.ImageBytes = Convert.FromBase64String(
                await Request.Content.ReadAsStringAsync());
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("api/users/image")]
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(byte[]))]
        public IHttpActionResult GetImageAsync()
        {
            byte[] imageBytes = db.User.First(u =>
                u.Login == HttpContext.Current.User.Identity.Name)
                .ImageBytes;
            return Ok(imageBytes);
        }

        [HttpPost]
        [Route("api/users/login")]
        [ResponseType(
            typeof(SerializedUser))]
        [AllowAnonymous]
        public IHttpActionResult IsAuthenticatedAsync(User loginUser)
        {
            if (!IsUserWithLoginAndPasswordFound(loginUser))
            {
                return Unauthorized();
            }
            else
            {
                User user = db.User.AsNoTracking()
                    .First(u => u.Login == loginUser.Login && u.Password == loginUser.Password);

                return Ok(new SerializedUser(user));
            }
        }

        private bool IsUserWithLoginAndPasswordFound(User loginUser)
        {
            return db.User.AsNoTracking()
                .Any(u => u.Login == loginUser.Login && u.Password == loginUser.Password);
        }

        [HttpGet]
        [Route("api/users/{userId}/contacts")]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IHttpActionResult> GetContacts(int userId)
        {
            User user = await db
                .User
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }
            System.Collections.Generic.IEnumerable<string> phones =
                user.UserPhone.Select(p => p.PhoneNumber);
            return Ok(new
            {
                Phones = phones,
            });
        }

        [HttpPost]
        [Route("api/users/{userId}/contacts")]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IHttpActionResult> PostContacts(int userId,
                                                          SerializedContacts contacts)
        {
            User user = await db
                .User
                .FirstOrDefaultAsync(u => u.Id == userId);
            int totalChangesCount = 0;
            if (user == null)
            {
                return NotFound();
            }
            if (contacts == null)
            {
                return BadRequest();
            }
            foreach (UserPhone phone in contacts.UserPhones)
            {
                if (user.UserPhone
                    .Any(p => p.PhoneNumber == phone.PhoneNumber))
                {
                    continue;
                }
                user.UserPhone.Add(phone);
                totalChangesCount++;
            }
            _ = await db.SaveChangesAsync();
            return Ok(totalChangesCount);
        }

        [HttpGet]
        [Route("api/myorders")]
        [Authorize(Roles = "Сотрудник, Клиент")]
        public async Task<IHttpActionResult> GetMyOrders()
        {
            User user = await db
                .User
                .FirstOrDefaultAsync(u =>
                u.Login == Thread.CurrentPrincipal.Identity.Name);
            ClaimsIdentity identity =
                (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
            string role = identity.FindFirst(ClaimTypes.Role).Value;
            switch (role)
            {
                case "Сотрудник":
                    return
                        Ok(
                            user.Order.ToList()
                            .ConvertAll(o => new SerializedOrder(o)));
                case "Клиент":
                    return
                        Ok(
                            user.Order1.ToList()
                            .ConvertAll(o => new SerializedOrder(o)));
                default:
                    return NotFound();
            }
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