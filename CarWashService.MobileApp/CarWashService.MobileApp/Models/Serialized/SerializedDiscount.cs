using System;

namespace CarWashService.MobileApp.Models.Serialized
{
    public class SerializedDiscount
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int DiscountPercent { get; set; }
        public string Description { get; set; }
        public DateTime WorkFromAsDate => DateTime.Parse(WorkFrom);
        public DateTime WorkToAsDate => DateTime.Parse(WorkTo);
        public string WorkFrom { get; set; }
        public string WorkTo { get; set; }
    }
}
