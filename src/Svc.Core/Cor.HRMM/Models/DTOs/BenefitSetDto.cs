namespace Cor.HRMM.Models.DTOs;

public class BenefitSetListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public double Benefit { get; set; } = default!;
     public double BenefitValue { get; set; } = default!;
    public string Per { get; set; } = default!; // enum. Per
    public string PerStr { get; set; } = default!;
    public string BenefitStr => $"{Benefit:#,##0.##}";
}

public class BenefitSetAddDto
{
    public string Name { get; set; } = default!;
    public double BenefitValue { get; set; } = default!;
    public string Per { get; set; } = default!; // enum. Per
}

public class BenefitSetModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public double BenefitValue { get; set; } = default!;
    public string Per { get; set; } = default!; // enum. Per
    public string RowVersion { get; set; } = default!;
}
