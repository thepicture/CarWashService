namespace CarWashService.MobileApp.Services
{
    public interface IImageResizer
    {
        /// <summary>
        /// Resizes the image.
        /// </summary>
        /// <param name="imageData">The image input bytes.</param>
        /// <param name="width">A new width.</param>
        /// <param name="height">A new height.</param>
        /// <param name="quality">A quality of the resized image.</param>
        /// <returns>The new resized (and compressed) image.</returns>
        byte[] ResizeImage(byte[] imageData,
                                  float width,
                                  float height,
                                  int quality);
    }
}
