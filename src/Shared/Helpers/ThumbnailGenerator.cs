using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Helpers;

public static class ThumbnailGenerator
{
    public static byte[] GenerateThumbnail(MemoryStream inputStream, int width = 150, int height = 150)
    {
        inputStream.Position = 0;
        using var image = Image.Load(inputStream);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Crop,
            Size = new Size(width, height)
        }));

        var outputStream = new MemoryStream();
        image.Save(outputStream, new PngEncoder()); // always save thumbnail as PNG
        outputStream.Position = 0;
        return outputStream.ToArray();
        //return outputStream;
    }

    public static bool IsImage(string contentType) => contentType.StartsWith("image/");
}