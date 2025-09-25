namespace Cor.HRMM.Models.Entities;

public class JobDesc : BaseEntity
{
    public string JobTitle { get; set; } = default!;
    public string Desc { get; set; } = default!;
}
