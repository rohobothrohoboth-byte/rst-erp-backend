// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Services\FileValidationService.cs

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Cor.FileManagement.Options;

namespace Cor.FileManagement.Services;

public interface IFileValidationService
{
    bool ValidateFile(IFormFile file, out string errorMessage);
    bool IsValidExtension(string fileName);
    bool IsValidSize(long fileSize);
    string GetFileExtension(string fileName);
    string GetFileType(string fileName);
    string GetDocumentType(string fileType);
    string[] GetAllowedExtensions();
    long GetMaxFileSize();
}

public class FileValidationService : IFileValidationService
{
    private readonly FileStorageOptions _fileStorageOptions;
    private readonly List<string> _allowedExtensions;
    private readonly long _maxFileSize;

    // ✅ Option 1: Using IOptions (Recommended)
    public FileValidationService(IOptions<FileStorageOptions> fileStorageOptions)
    {
        _fileStorageOptions = fileStorageOptions.Value;
        _allowedExtensions = _fileStorageOptions.AllowedExtensions?.ToList()
            ?? new List<string> { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
        _maxFileSize = _fileStorageOptions.MaxFileSize;
    }

    // ✅ Option 2: Keep IConfiguration as fallback
    // public FileValidationService(IConfiguration configuration)
    // {
    //     _allowedExtensions = configuration.GetSection("FileStorage:AllowedExtensions")
    //         .Get<List<string>>() ?? new List<string> { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt" };
    //     _maxFileSize = configuration.GetValue<long>("FileStorage:MaxFileSize", 10485760);
    // }

    public bool ValidateFile(IFormFile file, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (file == null || file.Length == 0)
        {
            errorMessage = "File is empty";
            return false;
        }

        if (!IsValidSize(file.Length))
        {
            errorMessage = $"File size exceeds {_maxFileSize / 1024 / 1024}MB limit";
            return false;
        }

        var extension = GetFileExtension(file.FileName);
        if (!IsValidExtension(extension))
        {
            errorMessage = $"File type {extension} is not allowed. Allowed: {string.Join(", ", _allowedExtensions)}";
            return false;
        }

        return true;
    }

    public bool IsValidExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        var extension = GetFileExtension(fileName);
        return _allowedExtensions.Contains(extension);
    }

    public bool IsValidSize(long fileSize)
    {
        return fileSize <= _maxFileSize;
    }

    public string GetFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    public string GetFileType(string fileName)
    {
        var extension = GetFileExtension(fileName);
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".bmp" => "image/bmp",
            ".ico" => "image/x-icon",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            ".7z" => "application/x-7z-compressed",
            ".mp4" => "video/mp4",
            ".avi" => "video/x-msvideo",
            ".mov" => "video/quicktime",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".flac" => "audio/flac",
            _ => "application/octet-stream"
        };
    }

    public string GetDocumentType(string fileType)
    {
        if (string.IsNullOrEmpty(fileType))
            return "Other";

        var type = fileType.ToLowerInvariant();

        if (type.Contains("pdf"))
            return "PDF";
        if (type.Contains("image") || type.Contains("jpeg") || type.Contains("png") || type.Contains("gif") || type.Contains("svg"))
            return "Image";
        if (type.Contains("word") || type.Contains("document") || type.Contains("docx") || type.Contains("doc"))
            return "Document";
        if (type.Contains("excel") || type.Contains("sheet") || type.Contains("xlsx") || type.Contains("xls") || type.Contains("csv"))
            return "Spreadsheet";
        if (type.Contains("powerpoint") || type.Contains("presentation") || type.Contains("pptx") || type.Contains("ppt"))
            return "Presentation";
        if (type.Contains("text") || type.Contains("txt"))
            return "Text";
        if (type.Contains("video") || type.Contains("mp4") || type.Contains("avi") || type.Contains("mov"))
            return "Video";
        if (type.Contains("audio") || type.Contains("mp3") || type.Contains("wav") || type.Contains("flac"))
            return "Audio";
        if (type.Contains("zip") || type.Contains("rar") || type.Contains("7z") || type.Contains("archive"))
            return "Archive";
        return "Other";
    }

    public string[] GetAllowedExtensions()
    {
        return _allowedExtensions.ToArray();
    }

    public long GetMaxFileSize()
    {
        return _maxFileSize;
    }
}