using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Leave.Domain.Entities.Local;

public class LocalPosition : LocalBaseEntity
{
    public string Name { get; set; } = default!;
    public string NameAm { get; set; } = default!;
    public int NoOfPosition { get; set; }
    public string IsVacant { get; set; } = default!;
    public Guid DepartmentId { get; set; }
    public Guid? JobGradeId { get; set; }


    [JsonIgnore]
    public LocalDepartment Department { get; set; } = null!;

    [JsonIgnore]
    public LocalJobGrade JobGrade { get; set; } = null!;


}