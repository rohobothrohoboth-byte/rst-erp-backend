// Cor.FileManagement/Options/FileStorageOptions.cs
 namespace Cor.FileManagement.Options;

public class FileStorageOptions
{
    public string UploadPath { get; set; } = "uploads";
    public int MaxFileSizeMB { get; set; } = 10;
    public int MaxFileSize => MaxFileSizeMB * 1024 * 1024;
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    public string ThumbnailPath { get; set; } = "thumbnails";
    public bool CompressImages { get; set; } = true;
    public int ThumbnailWidth { get; set; } = 200;
    public int ThumbnailHeight { get; set; } = 200;
    public int ImageQuality { get; set; } = 80;
    public string StorageProvider { get; set; } = "Local";
    public int MaxUploadFilesPerRequest { get; set; } = 10;
    public int MaxFolderDepth { get; set; } = 5;
    public int MaxItemsPerFolder { get; set; } = 1000;
}

// Cor.FileManagement/Options/SecurityOptions.cs
public class SecurityOptions
{
    public bool RequireAuthentication { get; set; } = true;
    public int SessionTimeoutMinutes { get; set; } = 60;
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutDurationMinutes { get; set; } = 15;
    public int ApiKeyExpiryDays { get; set; } = 365;
    public bool EncryptionEnabled { get; set; } = true;
}

// Cor.FileManagement/Options/PreviewOptions.cs
public class PreviewOptions
{
    public bool Enabled { get; set; } = true;
    public int MaxFileSizeForPreview { get; set; } = 52428800;
    public string[] SupportedTypes { get; set; } = Array.Empty<string>();
    public string PdfViewer { get; set; } = "iframe";
    public bool EnableFullscreen { get; set; } = true;
}