//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CarWashService.Web.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.Service = new HashSet<Service>();
        }
    
        public int Id { get; set; }
        public Nullable<int> SellerId { get; set; }
        public int ClientId { get; set; }
        public int BranchId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public bool IsConfirmed { get; set; }
        public System.DateTime AppointmentDate { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Service> Service { get; set; }
    }
}
