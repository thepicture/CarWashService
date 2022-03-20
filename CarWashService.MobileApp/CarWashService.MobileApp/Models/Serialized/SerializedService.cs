using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Models.Serialized
{
    public class SerializedService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public virtual IEnumerable<string> ServiceTypes { get; set; }
    }
}
