using CarWashService.MobileApp.Models.Serialized;
using CarWashService.MobileApp.Models.ViewModelHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CarWashService.MobileApp.Services
{
    public class BranchToLocationHelperListConverter : IListConverter<SerializedBranch, LocationHelper>
    {
        public async Task<IEnumerable<LocationHelper>> ConvertAllAsync(IEnumerable<SerializedBranch> branches)
        {
            Queue<SerializedBranch> queue = new Queue<SerializedBranch>(branches);

            List<LocationHelper> locationHelpers = new List<LocationHelper>();
            while (queue.Count > 0)
            {
                try
                {
                    LocationHelper locationHelper = await CreateParkingHelperAsync(queue.Peek());
                    locationHelpers.Add(locationHelper);
                    _ = queue.Dequeue();
                }
                catch (Exception ex)
                {
                    Debug.Fail("Can't create a location helper for id " + queue.Peek().Id + ", " + ex);
                }
            }
            return locationHelpers;
        }

        private async Task<LocationHelper> CreateParkingHelperAsync(SerializedBranch branch)
        {
            Geocoder geoCoder = new Geocoder();
            IEnumerable<Position> approximateLocations =
                        await geoCoder
                        .GetPositionsForAddressAsync(
                        string.Format("{0}, {1}, {2}",
                                      branch.StreetName,
                                      branch.CityName,
                                      "Россия")
                        );
            Position position = approximateLocations
                .FirstOrDefault();
            var locationHelper = new LocationHelper
            {
                Address = $"{branch.StreetName}, " +
                $"{branch.CityName}",
                Description = "С "
                + TimeSpan.Parse(branch.WorkFrom)
                .ToString(@"hh\:mm")
                + " по "
                + TimeSpan.Parse(branch.WorkTo)
                .ToString(@"hh\:mm"),
                Position = position,
                Branch = branch
            };
            return locationHelper;
        }
    }
}
