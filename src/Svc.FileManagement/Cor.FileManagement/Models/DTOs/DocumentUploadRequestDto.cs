// E:\untitled46\RST_ERP\src\Svc.FileManagement\Cor.FileManagement\Models\DTOs\DocumentUploadRequestDto.cs

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Cor.FileManagement.Models.DTOs;

public class DocumentUploadRequestDto
{
    // ✅ Use lowercase 'file' to match frontend
    public IFormFile? file { get; set; }

    // ✅ Use lowercase for all properties to match frontend
    public string? module { get; set; }
    public string? fileName { get; set; }
    public Guid? referenceId { get; set; }
    public string? documentType { get; set; }
    public string? category { get; set; }
    public string? description { get; set; }
    public bool isPublic { get; set; } = false;
    public bool isShared { get; set; } = false;
    public string? sharingLevel { get; set; } = "Private";
    public string? folderId { get; set; }
}