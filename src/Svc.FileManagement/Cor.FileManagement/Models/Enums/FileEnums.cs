// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\Enums\FileEnums.cs

namespace Cor.FileManagement.Models.Enums;

public enum FolderType
{
    Company,
    Department,
    Personal,
    Shared,
    Archive,
    Project,
    Finance,
    HR,
    Operations
}

public enum SharingLevel
{
    Private,
    Company,
    Department,
    Public,
    Specific
}

public enum PermissionType
{
    Read,
    Write,
    Delete,
    FullControl,
    Admin
}

public enum DocumentType
{
    PDF,
    Image,
    Word,
    Excel,
    PowerPoint,
    Text,
    Archive,
    Other
}

public enum StorageProvider
{
    Local,
    Azure,
    AWS,
    GCP,
    Database
}

public enum AccessAction
{
    View,
    Download,
    Upload,
    Delete,
    Share,
    Archive,
    Restore,
    Edit
}