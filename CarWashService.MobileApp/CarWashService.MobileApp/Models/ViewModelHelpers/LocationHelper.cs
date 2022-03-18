using CarWashService.MobileApp.Models.Serialized;
using Xamarin.Forms.Maps;

namespace CarWashService.MobileApp.Models.ViewModelHelpers
{
    public class LocationHelper
    {
        public string Address { get; set; }
        public string Description { get; set; }
        public Position Position { get; set; }
        public SerializedBranch Branch { get; set; }
    }
}
