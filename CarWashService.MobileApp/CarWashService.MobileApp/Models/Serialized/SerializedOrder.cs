using System.Collections.Generic;

namespace CarWashService.MobileApp.Models.Serialized
{
    public class SerializedOrder
    {
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
