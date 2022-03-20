using System.Collections.Generic;
using System.Linq;

namespace CarWashService.Web.Models.Entities.Serialized
{
    public class SerializedOrder
    {
        public SerializedOrder()
        {
        }

        public SerializedOrder(Order order)
        {
            Id = order.Id;
            SellerId = order.SellerId;
            ClientId = order.ClientId;
            BranchId = order.BranchId;
            CreationDate = order.CreationDate.ToString();
            AppointmentDate = order.AppointmentDate.ToString();
            IsConfirmed = order.IsConfirmed;
            Services = order.Service.Select(s => s.Id);
        }

        public int Id { get; set; }
        public int? SellerId { get; set; }
        public int ClientId { get; set; }
        public int BranchId { get; set; }
        public string CreationDate { get; set; }
        public string AppointmentDate { get; set; }
        public bool IsConfirmed { get; set; }
        public IEnumerable<int> Services { get; set; }

    }
}