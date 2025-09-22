namespace Cor.HRMM.Models.Entities;

public class PositionClass
{
    public string Name { get; set; } = default!;
    public string NameA { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Gender { get; set; } = default!; //PositionGender
    public int MinAge { get; set; } = default!;
    public int MaxAge { get; set; } = default!;
    public decimal WorkingHours { get; set; } = default!;
    public Guid SalaryScaleId { get; set; } = default!; //SalaryScale
    public Guid PositionClassCategoryId { get; set; } = default!; //PositionClassCategory
    public Guid PositionClassTypeId { get; set; } = default!; //Lup.PositionClassType

    //******************************************//

    public SalaryScale SalaryScale { get; set; } = null!;
    public PositionClassCategory PositionClassCategory { get; set; } = null!;
}
