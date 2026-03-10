using System.Text.Json.Serialization;

namespace Cor.Module.Models.DTOs;

public class CompListDto : BaseDTO
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string BranchCount { get; set; } = default!;
    [JsonIgnore]
    public int CountBra { get; set; }
}

public class AddCompDto
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
}

public class EditCompDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}