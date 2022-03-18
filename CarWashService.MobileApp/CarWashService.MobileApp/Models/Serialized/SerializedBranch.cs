using System.ComponentModel;

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

    }
}
