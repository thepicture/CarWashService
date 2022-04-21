using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CarWashService.MobileApp.Models.Serialized
{
    public class SerializedBranch
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string WorkFrom { get; set; }
        public string WorkTo { get; set; }
        public string StreetName { get; set; }
        public string CityName { get; set; }
        public int[] ServiceIds { get; set; }
        public List<string> PhoneNumbers { get; set; } = new List<string>();
        [JsonIgnore]
        public DateTime WorkFromAsDate => DateTime.Parse(WorkFrom);
        [JsonIgnore]
        public DateTime WorkToAsDate => DateTime.Parse(WorkTo);
    }
}
