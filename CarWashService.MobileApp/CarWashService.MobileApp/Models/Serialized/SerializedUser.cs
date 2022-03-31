using Newtonsoft.Json;
using System.IO;
using Xamarin.Forms;

namespace CarWashService.MobileApp.Models.Serialized
{
    public class SerializedUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int UserTypeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string PassportNumber { get; set; }
        public string PassportSeries { get; set; }
        public byte[] ImageBytes { get; set; }
        public string UserTypeName { get; set; }
        [JsonIgnore]
        public ImageSource ImageSource
        {
            get
            {
                if (ImageBytes == null)
                {
                    return null;
                }

                return ImageSource.FromStream(() =>
                {
                    return new MemoryStream(ImageBytes);
                });
            }
        }
    }
}
