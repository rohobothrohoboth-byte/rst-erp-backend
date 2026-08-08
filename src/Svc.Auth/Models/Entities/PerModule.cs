// Svc.Auth/Models/Entities/PerModule.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Svc.Auth.Models.Entities;

[Table("PerModule")]
public class PerModule : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Desc { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Icon { get; set; }  // Add this property

    public int Order { get; set; }  // Add this property

        public ICollection<PerMenu> PerMenus { get; set; } = new List<PerMenu>();
        public ICollection<UserPerModule> UserPerModules { get; set; } = new List<UserPerModule>();
        public ICollection<PositionPerModule> PositionPerModules { get; set; } = new List<PositionPerModule>();
}