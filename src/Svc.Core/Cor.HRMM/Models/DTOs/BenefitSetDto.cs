namespace Cor.HRMM.Models.DTOs;

public class BenefitSetListDto : BaseDto
{
    public string Name { get; set; } = default!;
    public decimal Benefit { get; set; } = default!;
    public string BenefitStr => $"{Benefit:#,##0.##}";
}

public class BenefitSetAddDto
{
    public string Name { get; set; } = default!;
    public decimal BenefitValue { get; set; } = default!;
}

public class BenefitSetModDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal BenefitValue { get; set; } = default!;
    public string RowVersion { get; set; } = default!;
}