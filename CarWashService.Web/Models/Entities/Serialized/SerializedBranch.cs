using System.Collections.Generic;
using System.Linq;

namespace CarWashService.Web.Models.Entities.Serialized
{
    public class SerializedBranch
    {
        public SerializedBranch()
        {
        }

        public SerializedBranch(Branch branch)
        {
            Id = branch.Id;
            Title = branch.Title;
            WorkFrom = branch.WorkFrom.ToString();
            WorkTo = branch.WorkTo.ToString();
            StreetName = branch.Address.StreetName;
            CityName = branch.Address.City.Name;
            ServiceIds = branch.Service.Select(s => s.Id);
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string WorkFrom { get; set; }
        public string WorkTo { get; set; }
        public string StreetName { get; set; }
        public string CityName { get; set; }
        public IEnumerable<int> ServiceIds { get; set; }
    }

}