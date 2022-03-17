namespace CarWashService.MobileApp.Models.Serialized
{
    public class SerializedUser
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string PassportNumber { get; set; }
        public string PassportSeries { get; set; }
    }
}
