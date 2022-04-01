using CarWashService.Web.Models.Entities;
using CarWashService.Web.Models.Entities.Serialized;
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
    public class ServicesController : ApiController
    {
        private readonly CarWashBaseEntities db = new CarWashBaseEntities();

        // GET: api/Services
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public IHttpActionResult GetService()
        {
            List<SerializedService> services = db.Service
                .ToList()
                .ConvertAll(s => new SerializedService(s));
            return Ok(services);
        }

        // GET: api/Services/5
        [ResponseType(typeof(SerializedService))]
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public async Task<IHttpActionResult> GetService(int id)
        {
            Service service = await db.Service.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(new SerializedService(service));
        }

        // GET: api/Services/5/orders
        [ResponseType(typeof(Service))]
        [Authorize(Roles = "Администратор, Сотрудник")]
        [Route("api/services/{serviceId}/orders")]
        public async Task<IHttpActionResult> GetServiceOrders(int serviceId)
        {
            Service service = await db.Service.FindAsync(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(
                service.Order.ToList()
                .ConvertAll(o => new SerializedOrder(o)));
        }

        // PUT: api/Services/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutService(int id, Service service)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != service.Id)
            {
                return BadRequest();
            }

            db.Entry(service).State = EntityState.Modified;

            try
            {
                _ = await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
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

        // POST: api/Services
        [ResponseType(typeof(SerializedService))]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IHttpActionResult> PostService(SerializedService serializedService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service service = new Service
            {
                Id = serializedService.Id,
                Name = serializedService.Name,
                Price = serializedService.Price,
                Description = serializedService.Description,
            };


            if (serializedService.Id == 0)
            {
                List<ServiceType> types = new List<ServiceType>();
                foreach (string typeName in serializedService.ServiceTypes)
                {
                    service.ServiceType.Add(
                        db.ServiceType.First(t => t.TypeName == typeName));
                }
                _ = db.Service.Add(service);
            }
            else
            {
                db.Service.Find(serializedService.Id).ServiceType.Clear();
                db.Service.Find(serializedService.Id).ServiceType
                    .Add(
                        db.ServiceType.ToList()
                            .First(t =>
                                t.TypeName == serializedService.ServiceTypes.First()));
                Service serviceFromDb = db.Service.Find(serializedService.Id);
                db.Entry(
                    serviceFromDb)
                    .CurrentValues.SetValues(service);
            }
            _ = await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi",
                                  new { id = service.Id },
                                  new SerializedService(service));
        }

        // DELETE: api/Services/5
        [ResponseType(typeof(Service))]
        [Authorize(Roles = "Администратор")]
        public async Task<IHttpActionResult> DeleteService(int id)
        {
            Service service = await db.Service.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _ = db.Service.Remove(service);
            _ = await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ServiceExists(int id)
        {
            return db.Service.Count(e => e.Id == id) > 0;
        }
    }
}