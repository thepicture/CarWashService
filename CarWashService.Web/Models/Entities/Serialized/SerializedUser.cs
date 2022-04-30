namespace CarWashService.Web.Models.Entities.Serialized
{
    public class SerializedUser
    {
        public SerializedUser()
        {
        }

        public SerializedUser(User user)
        {
            Id = user.Id;
            Login = user.Login;
            Email = user.Email;
            UserTypeName = user.UserType.Name;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Patronymic = user.Patronymic;
            PassportNumber = user.PassportNumber;
            PassportSeries = user.PassportSeries;
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string UserTypeName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string PassportNumber { get; set; }
        public string PassportSeries { get; set; }
        public byte[] ImageBytes { get; set; }
        public int UserTypeId { get; set; }
    }
}