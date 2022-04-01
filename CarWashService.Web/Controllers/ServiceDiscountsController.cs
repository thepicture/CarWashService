using CarWashService.Web.Models.Entities;
using CarWashService.Web.Models.Entities.Serialized;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace CarWashService.Web.Controllers
{
    public class ServiceDiscountsController : ApiController
    {
        private readonly CarWashBaseEntities db = new CarWashBaseEntities();

        // GET: api/ServiceDiscounts
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public IHttpActionResult GetServiceDiscount()
        {
            IList<SerializedDiscount> discounts = db.ServiceDiscount
                .ToList()
                .ConvertAll(sd => new SerializedDiscount(sd));
            return Ok(discounts);
        }

        // GET: api/ServiceDiscounts?serviceId=5
        [ResponseType(typeof(ServiceDiscount))]
        [Route("api/servicediscounts/{serviceId}")]
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        [HttpGet]
        public async Task<IHttpActionResult> GetServiceDiscounts(int serviceId)
        {
            Service service = await db.Service.FindAsync(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(
                service.ServiceDiscount
                .ToList()
                .ConvertAll(sd =>
                {
                    return new SerializedDiscount(sd);
                }));
        }

        // PUT: api/ServiceDiscounts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutServiceDiscount(int id, ServiceDiscount serviceDiscount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != serviceDiscount.Id)
            {
                return BadRequest();
            }

            db.Entry(serviceDiscount).State = EntityState.Modified;

            try
            {
                _ = await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceDiscountExists(id))
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

        // POST: api/ServiceDiscounts
        [ResponseType(typeof(ServiceDiscount))]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IHttpActionResult> PostServiceDiscount(ServiceDiscount serviceDiscount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _ = db.ServiceDiscount.Add(serviceDiscount);
            _ = await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = serviceDiscount.Id }, serviceDiscount.Id);
        }

        // DELETE: api/ServiceDiscounts/5
        [ResponseType(typeof(Nullable))]
        [Authorize(Roles = "Сотрудник")]
        public async Task<IHttpActionResult> DeleteServiceDiscount(int id)
        {
            ServiceDiscount serviceDiscount = await db.ServiceDiscount.FindAsync(id);
            if (serviceDiscount == null)
            {
                return NotFound();
            }

            _ = db.ServiceDiscount.Remove(serviceDiscount);
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

        private bool ServiceDiscountExists(int id)
        {
            return db.ServiceDiscount.Count(e => e.Id == id) > 0;
        }
    }
}