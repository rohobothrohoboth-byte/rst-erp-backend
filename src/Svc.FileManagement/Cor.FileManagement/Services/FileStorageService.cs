// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Services\FileStorageService.cs

using System.IO;
using System.Threading.Tasks;

namespace Cor.FileManagement.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "uploads");
    Task<byte[]> GetFileAsync(string filePath);
    Task DeleteFileAsync(string filePath);
    Task<string> UpdateFileAsync(Stream fileStream, string existingFilePath, string fileName);
}

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService()
    {
        // ✅ _basePath should be just "wwwroot", not "wwwroot/uploads"
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        // Create the uploads folder if it doesn't exist
        var uploadsPath = Path.Combine(_basePath, "uploads");
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "uploads")
    {
        // ✅ Build the full upload path
        var uploadPath = Path.Combine(_basePath, folder);
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Generate unique filename
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadPath, uniqueFileName);

        // Save the file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        // ✅ Return the relative path (folder/uniqueFileName)
        // Use forward slashes for consistency
        return $"{folder}/{uniqueFileName}".Replace("\\", "/");
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        // ✅ filePath should be like "uploads/filename"
        var fullPath = Path.Combine(_basePath, filePath);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"File not found: {fullPath}");

        return await File.ReadAllBytesAsync(fullPath);
    }

    public Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_basePath, filePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }

    public async Task<string> UpdateFileAsync(Stream fileStream, string existingFilePath, string fileName)
    {
        // Delete existing file
        await DeleteFileAsync(existingFilePath);

        // Save new file
        return await SaveFileAsync(fileStream, fileName);
    }
}