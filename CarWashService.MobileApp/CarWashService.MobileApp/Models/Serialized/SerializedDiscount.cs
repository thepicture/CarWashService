using Newtonsoft.Json;
using System;

namespace CarWashService.MobileApp.Models.Serialized
{
    public class SerializedDiscount
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int DiscountPercent { get; set; }
        [JsonIgnore]
        public string DiscountPercentAsString { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public DateTime WorkFromAsDate => DateTime.Parse(WorkFrom);
        [JsonIgnore]
        public DateTime WorkToAsDate => DateTime.Parse(WorkTo);
        public string WorkFrom { get; set; }
        public string WorkTo { get; set; }
    }
}
