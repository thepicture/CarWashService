using CarWashService.Web.Models.Entities;
using CarWashService.Web.Models.Entities.Serialized;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace CarWashService.Web.Controllers
{
    public class OrdersController : ApiController
    {
        private readonly CarWashBaseEntities db = new CarWashBaseEntities();

        // GET: api/Orders
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public IHttpActionResult GetOrder()
        {
            List<Order> orders = db.Order.ToList();
            string userName = HttpContext.Current.User.Identity.Name;
            if (db.User.First(u => u.Login == userName).UserType.Name == "Клиент")
            {
                orders = orders
                    .Where(o =>
                    {
                        return o.User1.Login.ToLower()
                               == userName.ToLower();
                    })
                    .ToList();
            }
            return Ok(
                orders.ConvertAll(o =>
                {
                    return new SerializedOrder(o);
                }));
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public async Task<IHttpActionResult> GetOrder(int id)
        {
            Order order = await db.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(new SerializedOrder(order));
        }

        // GET: api/Orders/5/confirm
        [Authorize(Roles = "Администратор, Сотрудник")]
        [Route("api/orders/{orderId}/confirm")]
        [HttpGet]
        public async Task<IHttpActionResult> ConfirmOrder(int orderId)
        {
            Order order = await db.Order.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.IsConfirmed = true;
            User user = await db.User.FirstOrDefaultAsync(u =>
           u.Login.ToLower()
           == HttpContext.Current.User.Identity.Name.ToLower());
            order.SellerId = user.Id;

            _ = await db.SaveChangesAsync();

            return Ok(new SerializedOrder(order));
        }

        // PUT: api/Orders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                _ = await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [ResponseType(typeof(SerializedOrder))]
        [Authorize(Roles = "Клиент")]
        public async Task<IHttpActionResult> PostOrder
            (SerializedOrder serializedOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = await db.User.FirstOrDefaultAsync(u =>
            u.Login.ToLower()
            == HttpContext.Current.User.Identity.Name.ToLower());
            Order order = new Order
            {
                ClientId = user.Id,
                CreationDate = DateTime.Now,
                BranchId = serializedOrder.BranchId,
                AppointmentDate = DateTime.Parse(serializedOrder.AppointmentDate)
            };


            if (serializedOrder.Services != null
                && serializedOrder.Services.Count() > 0)
            {
                foreach (int serviceId in serializedOrder.Services)
                {
                    order.Service.Add(
                        await db.Service.FindAsync(serviceId
                        )
                    );
                }
            }

            _ = db.Order.Add(order);
            _ = await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi",
                                  new { id = order.Id },
                                  new SerializedOrder(order));
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IHttpActionResult> DeleteOrder(int id)
        {
            Order order = await db.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _ = db.Order.Remove(order);
            _ = await db.SaveChangesAsync();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Order.Count(e => e.Id == id) > 0;
        }
    }
}