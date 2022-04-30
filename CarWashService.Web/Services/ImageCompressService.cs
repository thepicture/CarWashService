using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace CarWashService.Web
{
    public static class ImageResizerService
    {
        public static byte[] Resize(byte[] inputBytes, int qualityPercent)
        {
            if (inputBytes == null)
            {
                return null;
            }
            Image image;
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                image = Image.FromStream(inputStream);
                ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageDecoders()
                  .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality,
                                                                  qualityPercent);
                byte[] outputBytes;
                using (MemoryStream outputStream = new MemoryStream())
                {
                    image.Save(outputStream, jpegEncoder, encoderParameters);
                    outputBytes = outputStream.ToArray();
                    return outputBytes;
                }
            }
        }
    }
}