// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Services\FileThumbnailService.cs

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cor.FileManagement.Services;

public interface IFileThumbnailService
{
    Task<string?> GenerateThumbnailAsync(string filePath, string outputPath);
    string GetIconForFileType(string fileType);
}

public class FileThumbnailService : IFileThumbnailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileThumbnailService> _logger;

    public FileThumbnailService(IConfiguration configuration, ILogger<FileThumbnailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }



  public async Task<string?> GenerateThumbnailAsync(string filePath, string outputPath)
  {
      try
      {
          // Validate input parameters
          if (string.IsNullOrEmpty(filePath))
          {
              _logger.LogWarning("File path is null or empty");
              return null;
          }

          if (string.IsNullOrEmpty(outputPath))
          {
              _logger.LogWarning("Output path is null or empty");
              return null;
          }

          // Check if file exists
          if (!File.Exists(filePath))
          {
              _logger.LogWarning("File not found: {FilePath}", filePath);
              return null;
          }

          // Check file extension
          var extension = Path.GetExtension(filePath).ToLower();
          var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };

          if (!imageExtensions.Contains(extension))
          {
              _logger.LogInformation("File type not supported for thumbnail generation: {Extension}", extension);
              return null;
          }

          // ✅ Get wwwroot path
         var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // ✅ Clean output path
                var cleanOutputPath = outputPath
                    .Replace("wwwroot/", "")
                    .Replace("wwwroot\\", "")
                    .TrimStart('/', '\\');

                // ✅ Build full thumbnail directory path
                var fullThumbnailDir = Path.Combine(wwwrootPath, cleanOutputPath);

                // ✅ Ensure directory exists
                if (!Directory.Exists(fullThumbnailDir))
                {
                    Directory.CreateDirectory(fullThumbnailDir);
                }


          // ✅ Generate thumbnail
          using var image = await Image.LoadAsync(filePath);

          var thumbnailWidth = _configuration.GetValue<int>("FileStorage:ThumbnailWidth", 200);
          var thumbnailHeight = _configuration.GetValue<int>("FileStorage:ThumbnailHeight", 200);

          image.Mutate(x => x
              .Resize(new ResizeOptions
              {
                  Mode = ResizeMode.Max,
                  Size = new Size(thumbnailWidth, thumbnailHeight)
              }));

          // ✅ Generate thumbnail filename
          var thumbnailFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_thumb{extension}";
          var fullThumbnailPath = Path.Combine(fullThumbnailDir, thumbnailFileName);

          // ✅ Save thumbnail
          await image.SaveAsync(fullThumbnailPath);

          // ✅ FIX: Manually construct the relative path
          // Get the relative path from wwwroot to the thumbnail
            var relativePath = Path.GetRelativePath(wwwrootPath, fullThumbnailPath)
                    .Replace('\\', '/'); // ✅ CONVERT BACKSLASHES TO FORWARD SLASHES

          // OR use this alternative approach:
          // var relativePath = Path.GetRelativePath(wwwrootPath, fullThumbnailPath)
          //     .Replace('\\', '/');

          _logger.LogInformation("✅ Thumbnail generated successfully: {ThumbnailPath}", relativePath);

          // ✅ Returns: "thumbnails/filename_thumb.png"
          return relativePath;
      }
      catch (FileNotFoundException ex)
      {
          _logger.LogError(ex, "File not found when generating thumbnail: {FilePath}", filePath);
          return null;
      }
      catch (IOException ex)
      {
          _logger.LogError(ex, "IO error generating thumbnail for: {FilePath}", filePath);
          return null;
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error generating thumbnail for: {FilePath}", filePath);
          return null;
      }
  }

    public string GetIconForFileType(string fileType)
    {
        if (string.IsNullOrEmpty(fileType))
            return "📄";

        var lowerFileType = fileType.ToLower();

        // Document types
        if (lowerFileType.Contains("pdf"))
            return "📄";

        if (lowerFileType.Contains("image") ||
            lowerFileType.Contains("jpg") ||
            lowerFileType.Contains("jpeg") ||
            lowerFileType.Contains("png") ||
            lowerFileType.Contains("gif") ||
            lowerFileType.Contains("bmp") ||
            lowerFileType.Contains("tiff") ||
            lowerFileType.Contains("webp") ||
            lowerFileType.Contains("svg"))
            return "🖼️";

        if (lowerFileType.Contains("word") ||
            lowerFileType.Contains("document") ||
            lowerFileType.Contains("docx") ||
            lowerFileType.Contains("doc") ||
            lowerFileType.Contains("rtf"))
            return "📝";

        if (lowerFileType.Contains("excel") ||
            lowerFileType.Contains("sheet") ||
            lowerFileType.Contains("xlsx") ||
            lowerFileType.Contains("xls") ||
            lowerFileType.Contains("csv"))
            return "📊";

        if (lowerFileType.Contains("powerpoint") ||
            lowerFileType.Contains("presentation") ||
            lowerFileType.Contains("pptx") ||
            lowerFileType.Contains("ppt"))
            return "📑";

        if (lowerFileType.Contains("text") ||
            lowerFileType.Contains("txt") ||
            lowerFileType.Contains("log"))
            return "📃";

        if (lowerFileType.Contains("zip") ||
            lowerFileType.Contains("archive") ||
            lowerFileType.Contains("rar") ||
            lowerFileType.Contains("7z") ||
            lowerFileType.Contains("tar") ||
            lowerFileType.Contains("gz"))
            return "📦";

        if (lowerFileType.Contains("audio") ||
            lowerFileType.Contains("mp3") ||
            lowerFileType.Contains("wav") ||
            lowerFileType.Contains("flac") ||
            lowerFileType.Contains("aac"))
            return "🎵";

        if (lowerFileType.Contains("video") ||
            lowerFileType.Contains("mp4") ||
            lowerFileType.Contains("avi") ||
            lowerFileType.Contains("mkv") ||
            lowerFileType.Contains("mov") ||
            lowerFileType.Contains("wmv"))
            return "🎬";

        if (lowerFileType.Contains("code") ||
            lowerFileType.Contains("json") ||
            lowerFileType.Contains("xml") ||
            lowerFileType.Contains("html") ||
            lowerFileType.Contains("css") ||
            lowerFileType.Contains("js") ||
            lowerFileType.Contains("cs") ||
            lowerFileType.Contains("java") ||
            lowerFileType.Contains("py"))
            return "💻";

        // Default icon
        return "📄";
    }
}