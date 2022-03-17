namespace CarWashService.Web.Models.Entities.Serialized
{
    public class SerializedOrder
    {
        public SerializedOrder()
        {
        }

        public SerializedOrder(Order order)
        {
            Id = order.Id;
            SellerId = order.SellerId;
            ClientId = order.ClientId;
            BranchId = order.BranchId;
            Date = order.Date.ToString();
            IsConfirmed = order.IsConfirmed;
        }

        public int Id { get; set; }
        public int SellerId { get; set; }
        public int ClientId { get; set; }
        public int BranchId { get; set; }
        public string Date { get; set; }
        public bool IsConfirmed { get; set; }

    }
}