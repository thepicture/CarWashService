using Newtonsoft.Json;
using System;
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
        public IEnumerable<string> ServiceNames { get; set; }
        public string SellerFullName { get; set; }
        public string ClientFullName { get; set; }
        public decimal TotalPrice { get; set; }
        [JsonIgnore]
        public DateTime AppointmentDateTimeAsDateTime { get; set; }
        [JsonIgnore]
        public SerializedBranch Branch { get; set; }
    }
}
