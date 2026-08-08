using System.ComponentModel.DataAnnotations;

namespace Cor.HRMM.Models.DTOs;

public class PositionEduListDto : BaseDto
{
    public Guid PositionId { get; set; }
    public Guid EducationQualId { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public string EducationQual { get; set; } = string.Empty;
    public string EducationLevelStr { get; set; } = string.Empty;
}

public class PositionEduAddDto
{
    [Required(ErrorMessage = "PositionId is required")]
    public Guid PositionId { get; set; }
    
    [Required(ErrorMessage = "EducationQualId is required")]
    public Guid EducationQualId { get; set; }
    
    [Required(ErrorMessage = "EducationLevel is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "EducationLevel must be between 1 and 100 characters")]
    public string EducationLevel { get; set; } = string.Empty;
}

public class PositionEduModDto
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "PositionId is required")]
    public Guid PositionId { get; set; }
    
    [Required(ErrorMessage = "EducationQualId is required")]
    public Guid EducationQualId { get; set; }
    
    [Required(ErrorMessage = "EducationLevel is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "EducationLevel must be between 1 and 100 characters")]
    public string EducationLevel { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "RowVersion is required")]
    public string RowVersion { get; set; } = string.Empty;
}