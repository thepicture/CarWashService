using CarWashService.Web.Models.Entities;
using CarWashService.Web.Models.Entities.Serialized;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CarWashService.Web.Controllers
{
    public class CitiesController : ApiController
    {
        private CarWashBaseEntities db = new CarWashBaseEntities();

        // GET: api/Cities
        [Authorize(Roles = "Администратор, Сотрудник, Клиент")]
        public IHttpActionResult GetCity()
        {
            List<SerializedCity> cities = db.City
                .ToList()
                .ConvertAll(c => new SerializedCity(c));
            return Ok(cities);
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