namespace CarWashService.Web.Models.Entities.Serialized
{
    public class SerializedCity
    {
        public SerializedCity(City city)
        {
            Id = city.Id;
            Name = city.Name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
