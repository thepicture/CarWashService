using System.Collections.Generic;
using System.Linq;

namespace CarWashService.Web.Models.Entities.Serialized
{
    public class SerializedService
    {
        public SerializedService()
        {
        }

        public SerializedService(Service service)
        {
            Id = service.Id;
            Name = service.Name;
            Description = service.Description;
            Price = service.Price;
            ServiceTypes = service.ServiceType
                .Select(t => t.TypeName);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public virtual IEnumerable<string> ServiceTypes { get; set; }
    }
}