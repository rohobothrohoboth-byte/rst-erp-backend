using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profile.Domain.Entities;

namespace Profile.Utility.Persistence;

public abstract class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.DateAdd).IsRequired().HasColumnType("timestamp with time zone");
        b.Property(x => x.DateMod).HasColumnType("timestamp with time zone");
        b.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        b.Property(x => x.xmin).HasColumnName("xmin").HasColumnType("xid").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate();
        b.HasIndex(x => x.IsDeleted);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class AddressConfig : BaseEntityConfig<Address>
{
    public override void Configure(EntityTypeBuilder<Address> b)
    {
        base.Configure(b);
        b.Property(x => x.AddressType).HasMaxLength(2).IsRequired();
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.Region).HasMaxLength(100);
        b.Property(x => x.Subcity).HasMaxLength(100);
        b.Property(x => x.Zone).HasMaxLength(50);
        b.Property(x => x.Woreda).HasMaxLength(50);
        b.Property(x => x.Kebele).HasMaxLength(50);
        b.Property(x => x.HouseNo).HasMaxLength(50);
        b.Property(x => x.Telephone).HasMaxLength(30);
        b.Property(x => x.PoBox).HasMaxLength(50);
        b.Property(x => x.Fax).HasMaxLength(30);
        b.Property(x => x.Email).HasMaxLength(100);
        b.Property(x => x.Website).HasMaxLength(100);
        b.HasIndex(x => x.Country);
        b.HasIndex(x => x.Email);
    }
}

public class EmeContactConfig : BaseEntityConfig<EmergencyContact>
{
    public override void Configure(EntityTypeBuilder<EmergencyContact> b)
    {
        base.Configure(b);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.MiddleName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Gender).HasMaxLength(20).IsRequired();
        b.Property(x => x.Nationality).HasMaxLength(100).IsRequired();
        b.Property(x => x.Relation).HasMaxLength(20).IsRequired();
        b.HasIndex(x => x.EmployeeId);
        b.HasIndex(x => new { x.Id, x.EmployeeId }).IsUnique(true);
        b.HasIndex(x => new { x.Id, x.AddressId }).IsUnique(true);
    }
}

public class EmpBioConfig : BaseEntityConfig<EmpBio>
{
    public override void Configure(EntityTypeBuilder<EmpBio> b)
    {
        base.Configure(b);
        b.Property(x => x.BirthLocation).HasMaxLength(150);
        b.Property(x => x.MotherFullName).HasMaxLength(150);
        b.Property(x => x.HasBirthCert).HasMaxLength(10);
        b.Property(x => x.HasMarriageCert).HasMaxLength(10);
        b.Property(x => x.MaritalStatus).HasMaxLength(10);
        b.HasIndex(x => x.EmployeeId).IsUnique();
        b.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpBio>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.AddressId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmpCertConfig : BaseEntityConfig<EmpCert>
{
    public override void Configure(EntityTypeBuilder<EmpCert> b)
    {
        base.Configure(b);
        b.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        b.Property(x => x.FileSize).IsRequired();
        b.Property(x => x.CertType).IsRequired();
        b.HasIndex(x => x.FileName);
        b.HasIndex(x => x.ContentType);
        b.HasIndex(x => x.CertType);
        b.HasIndex(x => x.EmployeeId);
    }
}

public class EmpCertBirthConfig : BaseEntityConfig<EmpCertBirth>
{
    public override void Configure(EntityTypeBuilder<EmpCertBirth> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.HasIndex(x => x.EmpCertId).IsUnique();
    }
}

public class EmpCertMarriageConfig : BaseEntityConfig<EmpCertMarriage>
{
    public override void Configure(EntityTypeBuilder<EmpCertMarriage> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.HasIndex(x => x.EmpCertId).IsUnique();
    }
}

public class EmpFamilyConfig : BaseEntityConfig<EmpFamily>
{
    public override void Configure(EntityTypeBuilder<EmpFamily> b)
    {
        base.Configure(b);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.MiddleName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Gender).HasMaxLength(20).IsRequired();
        b.Property(x => x.Nationality).HasMaxLength(100).IsRequired();
        b.Property(x => x.Relation).HasMaxLength(20).IsRequired();
        b.HasIndex(x => x.EmployeeId);
    }
}

public class EmpFinanceConfig : BaseEntityConfig<EmpFinance>
{
    public override void Configure(EntityTypeBuilder<EmpFinance> b)
    {
        base.Configure(b);
        b.Property(x => x.Tin).HasMaxLength(50);
        b.Property(x => x.BankAccountNo).HasMaxLength(50);
        b.Property(x => x.PensionNumber).HasMaxLength(50);
        b.HasIndex(x => x.EmployeeId).IsUnique();
        b.HasIndex(x => x.Tin).IsUnique(false);
        b.HasIndex(x => x.BankAccountNo);
        b.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpFinance>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmpGuarantorConfig : BaseEntityConfig<EmpGuarantor>
{
    public override void Configure(EntityTypeBuilder<EmpGuarantor> b)
    {
        base.Configure(b);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.MiddleName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Gender).HasMaxLength(20).IsRequired();
        b.Property(x => x.Nationality).HasMaxLength(100).IsRequired();
        b.Property(x => x.Relation).HasMaxLength(20).IsRequired();
        b.HasIndex(x => x.EmployeeId);
        b.HasIndex(x => new { x.Id, x.EmployeeId }).IsUnique(true);
        b.HasIndex(x => new { x.Id, x.AddressId }).IsUnique(true);
    }
}

public class EmpGuarantorFileConfig : BaseEntityConfig<EmpGuarantorFile>
{
    public override void Configure(EntityTypeBuilder<EmpGuarantorFile> b)
    {
        base.Configure(b);
        b.Property(x => x.EmpGuarantorId).IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.EmpGuarantor).WithMany().HasForeignKey(x => x.EmpGuarantorId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.EmpGuarantorId);
        b.HasIndex(x => x.FileMetaDataId);
        b.HasIndex(x => new { x.EmpGuarantorId, x.FileMetaDataId }).IsUnique();
    }
}

public class EmpGuarantorFileBlobConfig : BaseEntityConfig<EmpGuarantorFileBlob>
{
    public override void Configure(EntityTypeBuilder<EmpGuarantorFileBlob> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpGuarantorFileBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmployeeConfig : BaseEntityConfig<Employee>
{
    public override void Configure(EntityTypeBuilder<Employee> b)
    {
        base.Configure(b);
        b.Property(x => x.Code).HasMaxLength(10).IsRequired().HasDefaultValueSql("'EMP' || LPAD(nextval('emp_code_seq')::text, 7, '0')").ValueGeneratedOnAdd().Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        b.Property(x => x.EmploymentType).HasMaxLength(20).IsRequired();
        b.Property(x => x.EmploymentNature).HasMaxLength(20);
        b.Property(x => x.WorkArrangement).HasMaxLength(20);
        b.Property(x => x.EmploymentDate).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.PersonId).IsUnique();
        b.HasIndex(x => x.DepartmentId);
        b.HasIndex(x => x.PositionId);
        b.HasIndex(x => x.JobGradeId);
        b.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmpPensionCardConfig : BaseEntityConfig<EmpPensionCard>
{
    public override void Configure(EntityTypeBuilder<EmpPensionCard> b)
    {
        base.Configure(b);
        b.Property(x => x.RegistrationDate).IsRequired();
        b.Property(x => x.SentDate);
        b.Property(x => x.ReceivedDate);
        b.Property(x => x.IsReceived).HasMaxLength(10).IsRequired();
        b.Property(x => x.IsSent).HasMaxLength(10).IsRequired();
        b.Property(x => x.EmployeeId).IsRequired();
        b.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpPensionCard>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.EmployeeId).IsUnique();
        b.HasIndex(x => x.IsReceived);
        b.HasIndex(x => x.IsSent);
    }
}

public class EmpPhotoConfig : BaseEntityConfig<EmpPhoto>
{
    public override void Configure(EntityTypeBuilder<EmpPhoto> b)
    {
        base.Configure(b);
        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.Property(x => x.ThumbnailId).IsRequired();
        b.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpPhoto>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Thumbnail).WithMany().HasForeignKey(x => x.ThumbnailId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.EmployeeId).IsUnique();
        b.HasIndex(x => x.FileMetaDataId);
        b.HasIndex(x => x.ThumbnailId);
    }
}

public class EmpPhotoBlobConfig : BaseEntityConfig<EmpPhotoBlob>
{
    public override void Configure(EntityTypeBuilder<EmpPhotoBlob> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpPhotoBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmpPhotoThumbnailConfig : BaseEntityConfig<EmpPhotoThumbnail>
{
    public override void Configure(EntityTypeBuilder<EmpPhotoThumbnail> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpPhotoThumbnail>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmpSalaryConfig : BaseEntityConfig<EmpSalary>
{
    public override void Configure(EntityTypeBuilder<EmpSalary> b)
    {
        base.Configure(b);
        b.Property(es => es.BaseSalary).IsRequired().HasColumnType("double precision");
        b.Property(js => js.Currency).IsRequired().HasMaxLength(15);
        b.Property(js => js.SalaryPayFreq).IsRequired().HasMaxLength(15);
        b.Property(es => es.EffectiveFrom).IsRequired();
        b.HasIndex(es => es.EffectiveFrom);
        b.HasIndex(es => es.JgStepId);
        b.HasIndex(es => es.EmployeeId);
        b.HasOne(es => es.Employee).WithMany(e => e.EmpSalary).HasForeignKey(es => es.EmployeeId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmpSignConfig : BaseEntityConfig<EmpSign>
{
    public override void Configure(EntityTypeBuilder<EmpSign> b)
    {
        base.Configure(b);
        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpSign>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.EmployeeId).IsUnique();
        b.HasIndex(x => x.FileMetaDataId);
    }
}

public class EmpSignBlobConfig : BaseEntityConfig<EmpSignBlob>
{
    public override void Configure(EntityTypeBuilder<EmpSignBlob> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpSignBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmpStampConfig : BaseEntityConfig<EmpStamp>
{
    public override void Configure(EntityTypeBuilder<EmpStamp> b)
    {
        base.Configure(b);
        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpStamp>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        b.HasIndex(x => x.EmployeeId).IsUnique();
        b.HasIndex(x => x.FileMetaDataId);
    }
}

public class EmpStampBlobConfig : BaseEntityConfig<EmpStampBlob>
{
    public override void Configure(EntityTypeBuilder<EmpStampBlob> b)
    {
        base.Configure(b);
        b.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        b.Property(x => x.FileMetaDataId).IsRequired();
        b.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpStampBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        b.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class FileMetaDataConfig : BaseEntityConfig<FileMetaData>
{
    public override void Configure(EntityTypeBuilder<FileMetaData> b)
    {
        base.Configure(b);
        b.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        b.Property(x => x.FileSize).IsRequired();
        b.HasIndex(x => x.FileName);
        b.HasIndex(x => x.ContentType);
    }
}

public class PersonConfig : BaseEntityConfig<Person>
{
    public override void Configure(EntityTypeBuilder<Person> b)
    {
        base.Configure(b);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.FirstNameAm).HasMaxLength(100);
        b.Property(x => x.MiddleName).HasMaxLength(100);
        b.Property(x => x.MiddleNameAm).HasMaxLength(100);
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastNameAm).HasMaxLength(100);
        b.Property(x => x.Gender).HasMaxLength(20).IsRequired();
        b.Property(x => x.Nationality).HasMaxLength(100);
        b.HasIndex(x => x.Gender);
    }
}

public class EmpContractConfig : BaseEntityConfig<EmpContract>
{
    public override void Configure(EntityTypeBuilder<EmpContract> b)
    {
        base.Configure(b);
        b.Property(x => x.ContractNumber).HasMaxLength(30).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.ContractType).HasMaxLength(50).IsRequired();
        b.Property(x => x.TerminationReason).HasMaxLength(500);
        b.Property(x => x.DocumentRef).HasMaxLength(255);
        b.Property(x => x.Notes).HasMaxLength(1000);
        b.HasIndex(x => x.ContractNumber);
        b.HasIndex(x => x.EmployeeId);
        b.HasIndex(x => x.Status);
        b.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmpPromotionConfig : BaseEntityConfig<EmpPromotion>
{
    public override void Configure(EntityTypeBuilder<EmpPromotion> b)
    {
        base.Configure(b);
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.Reason).HasMaxLength(500);
        b.Property(x => x.Comments).HasMaxLength(1000);
        b.HasIndex(x => x.EmployeeId);
        b.HasIndex(x => x.Status);
        b.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmpTransferConfig : BaseEntityConfig<EmpTransfer>
{
    public override void Configure(EntityTypeBuilder<EmpTransfer> b)
    {
        base.Configure(b);
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.Reason).HasMaxLength(500);
        b.Property(x => x.Comments).HasMaxLength(1000);
        b.HasIndex(x => x.EmployeeId);
        b.HasIndex(x => x.Status);
        b.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
    }
}