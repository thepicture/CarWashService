namespace CarWashService.Web.Models.Entities.Serialized
{
    public class SerializedDiscount
    {
        public SerializedDiscount()
        {
        }

        public SerializedDiscount(ServiceDiscount serviceDiscount)
        {
            Id = serviceDiscount.Id;
            ServiceId = serviceDiscount.ServiceId;
            DiscountPercent = serviceDiscount.DiscountPercent;
            Description = serviceDiscount.Description;
            WorkFrom = serviceDiscount.WorkFrom
                .ToString();
            WorkTo = serviceDiscount.WorkTo
                .ToString();
        }
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int DiscountPercent { get; set; }
        public string Description { get; set; }
        public string WorkFrom { get; set; }
        public string WorkTo { get; set; }
    }
}