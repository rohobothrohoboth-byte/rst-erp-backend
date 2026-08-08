using Microsoft.AspNetCore.Http;

namespace Profile.Domain.DTOs;

public class EmpModRes
{
    public Guid Id { get; set; }
}

public class CertSerDto
{
    public Guid Id { get; set; }
    public string HasCert { get; set; } = default!; // enum.YesNo
    public IFormFile? File { get; set; } = default!;
}

public class MyBioModDto
{
    public Guid Id { get; set; }
    public string BirthLocation { get; set; } = default!;
    public string MotherFullName { get; set; } = default!;
    public string HasBirthCert { get; set; } = default!; // enum.YesNo
    public string HasMarriageCert { get; set; } = default!; // enum.YesNo
    public IFormFile? File1 { get; set; } = default!;
    public IFormFile? File2 { get; set; } = default!;
}

public class MyFinanceModDto
{
    public Guid Id { get; set; }
    public string Tin { get; set; } = default!;
    public string BankAccountNo { get; set; } = default!;
    public string PensionNumber { get; set; } = default!;
}

public class MyEmContModDto
{
    public Guid EmployeeId { get; set; }
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
    //Address
    public string AddressType { get; set; } = default!; // enum.AddressType (0/1)
    public string? Country { get; set; }
    public string Region { get; set; } = default!;
    public string? Subcity { get; set; }
    public string? Zone { get; set; }
    public string? Woreda { get; set; }
    public string? Kebele { get; set; }
    public string? HouseNo { get; set; }
    public string Telephone { get; set; } = default!;
    public string? PoBox { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
}

public class MyFamilyAddDto
{
    public Guid EmployeeId { get; set; } = default!; //Employee
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
}

public class MyFamilyModDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string MiddleName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Gender { get; set; } = default!; // enum.Gender
    public string Nationality { get; set; } = default!;
    public string Relation { get; set; } = default!; //enum.Relation
}

public class EmpRevDto
{
    public Guid Id { get; set; }
    public bool Decision { get; set; } = true; // True = Accept, False = Deny
}