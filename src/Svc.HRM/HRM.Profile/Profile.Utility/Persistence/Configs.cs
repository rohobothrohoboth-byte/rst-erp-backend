using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Profile.Domain.Entities;

namespace Profile.Utility.Persistence;

public abstract class BaseEntityConfig<T> : IEntityTypeConfiguration<T> where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();
        b.Property(x => x.DateAdd).IsRequired();
        b.Property(x => x.DateMod);
        b.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        b.Property(x => x.RowVersion).IsRowVersion().IsConcurrencyToken();
        b.HasIndex(x => x.IsDeleted);
        b.HasIndex(x => x.Id);
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class AddressConfig : BaseEntityConfig<Address>
{
    public override void Configure(EntityTypeBuilder<Address> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.AddressType).HasMaxLength(2).IsRequired();
        builder.Property(x => x.Country).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Region).HasMaxLength(100);
        builder.Property(x => x.Subcity).HasMaxLength(100);
        builder.Property(x => x.Zone).HasMaxLength(50);
        builder.Property(x => x.Woreda).HasMaxLength(50);
        builder.Property(x => x.Kebele).HasMaxLength(50);
        builder.Property(x => x.HouseNo).HasMaxLength(50);
        builder.Property(x => x.Telephone).HasMaxLength(30);
        builder.Property(x => x.PoBox).HasMaxLength(50);
        builder.Property(x => x.Fax).HasMaxLength(30);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.Website).HasMaxLength(100);
        builder.HasIndex(x => x.Country);
        builder.HasIndex(x => x.Email);
    }
}

public class EmeContactConfig : BaseEntityConfig<EmergencyContact>
{
    public override void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        base.Configure(builder);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.PersonId);
        builder.HasIndex(x => x.RelationId);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.EmployeeId, x.PersonId }).IsUnique(false);
    }
}

public class EmpBioConfig : BaseEntityConfig<EmpBio>
{
    public override void Configure(EntityTypeBuilder<EmpBio> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.BirthLocation).HasMaxLength(150);
        builder.Property(x => x.MotherFullName).HasMaxLength(150);
        builder.Property(x => x.HasBirthCert).HasMaxLength(10);
        builder.Property(x => x.HasMarriageCert).HasMaxLength(10);
        builder.Property(x => x.MaritalStatus).HasMaxLength(10);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpBio>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Address).WithMany().HasForeignKey(x => x.AddressId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmpFamilyConfig : BaseEntityConfig<EmpFamily>
{
    public override void Configure(EntityTypeBuilder<EmpFamily> builder)
    {
        base.Configure(builder);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.PersonId);
        builder.HasIndex(x => x.RelationId);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.EmployeeId, x.PersonId }).IsUnique(false);
    }
}

public class EmpFinanceConfig : BaseEntityConfig<EmpFinance>
{
    public override void Configure(EntityTypeBuilder<EmpFinance> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Tin).HasMaxLength(50);
        builder.Property(x => x.BankAccountNo).HasMaxLength(50);
        builder.Property(x => x.PensionNumber).HasMaxLength(50);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasIndex(x => x.Tin).IsUnique(false);
        builder.HasIndex(x => x.BankAccountNo);
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpFinance>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmpGuarantorConfig : BaseEntityConfig<EmpGuarantor>
{
    public override void Configure(EntityTypeBuilder<EmpGuarantor> builder)
    {
        base.Configure(builder);
        builder.HasIndex(x => x.EmployeeId);
        builder.HasIndex(x => x.PersonId);
        builder.HasIndex(x => x.RelationId);
        builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.EmployeeId, x.PersonId }).IsUnique(false);
    }
}

public class EmpGuarantorFileConfig : BaseEntityConfig<EmpGuarantorFile>
{
    public override void Configure(EntityTypeBuilder<EmpGuarantorFile> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.EmpGuarantorId).IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.EmpGuarantor).WithMany().HasForeignKey(x => x.EmpGuarantorId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.EmpGuarantorId);
        builder.HasIndex(x => x.FileMetaDataId);
        builder.HasIndex(x => new { x.EmpGuarantorId, x.FileMetaDataId }).IsUnique();
    }
}

public class EmpGuarantorFileBlobConfig : BaseEntityConfig<EmpGuarantorFileBlob>
{
    public override void Configure(EntityTypeBuilder<EmpGuarantorFileBlob> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpGuarantorFileBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmployeeConfig : BaseEntityConfig<Employee>
{
    public override void Configure(EntityTypeBuilder<Employee> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Code).HasMaxLength(50).IsRequired(); builder.Property(x => x.EmploymentType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.EmploymentNature).HasMaxLength(20);
        builder.Property(x => x.WorkArrangement).HasMaxLength(20);
        builder.Property(x => x.EmploymentDate).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.HasIndex(x => x.PersonId).IsUnique();
        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.PositionId);
        builder.HasIndex(x => x.JobGradeId);
        builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class EmpPensionCardConfig : BaseEntityConfig<EmpPensionCard>
{
    public override void Configure(EntityTypeBuilder<EmpPensionCard> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.RegistrationDate).IsRequired();
        builder.Property(x => x.SentDate);
        builder.Property(x => x.ReceivedDate);
        builder.Property(x => x.IsReceived).HasMaxLength(10).IsRequired();
        builder.Property(x => x.IsSent).HasMaxLength(10).IsRequired();
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpPensionCard>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasIndex(x => x.IsReceived);
        builder.HasIndex(x => x.IsSent);
    }
}

public class EmpPhotoConfig : BaseEntityConfig<EmpPhoto>
{
    public override void Configure(EntityTypeBuilder<EmpPhoto> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.Property(x => x.ThumbnailId).IsRequired();
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpPhoto>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Thumbnail).WithMany().HasForeignKey(x => x.ThumbnailId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasIndex(x => x.FileMetaDataId);
        builder.HasIndex(x => x.ThumbnailId);
    }
}

public class EmpPhotoBlobConfig : BaseEntityConfig<EmpPhotoBlob>
{
    public override void Configure(EntityTypeBuilder<EmpPhotoBlob> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpPhotoBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmpPhotoThumbnailConfig : BaseEntityConfig<EmpPhotoThumbnail>
{
    public override void Configure(EntityTypeBuilder<EmpPhotoThumbnail> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpPhotoThumbnail>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmpSignConfig : BaseEntityConfig<EmpSign>
{
    public override void Configure(EntityTypeBuilder<EmpSign> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpSign>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasIndex(x => x.FileMetaDataId);
    }
}

public class EmpSignBlobConfig : BaseEntityConfig<EmpSignBlob>
{
    public override void Configure(EntityTypeBuilder<EmpSignBlob> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpSignBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmpStampConfig : BaseEntityConfig<EmpStamp>
{
    public override void Configure(EntityTypeBuilder<EmpStamp> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.EmployeeId).IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpStamp>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.FileMetaData).WithMany().HasForeignKey(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasIndex(x => x.FileMetaDataId);
    }
}

public class EmpStampBlobConfig : BaseEntityConfig<EmpStampBlob>
{
    public override void Configure(EntityTypeBuilder<EmpStampBlob> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.Data).HasColumnType("bytea").IsRequired();
        builder.Property(x => x.FileMetaDataId).IsRequired();
        builder.HasOne(x => x.FileMetaData).WithOne().HasForeignKey<EmpStampBlob>(x => x.FileMetaDataId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.FileMetaDataId).IsUnique();
    }
}

public class EmpStateConfig : BaseEntityConfig<EmpState>
{
    public override void Configure(EntityTypeBuilder<EmpState> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.IsTerminated).HasMaxLength(10);
        builder.Property(x => x.IsApproved).HasMaxLength(10);
        builder.Property(x => x.IsStandBy).HasMaxLength(10);
        builder.Property(x => x.IsRetired).HasMaxLength(10);
        builder.Property(x => x.IsUnderProbation).HasMaxLength(10);
        builder.HasIndex(x => x.EmployeeId).IsUnique();
        builder.HasOne(x => x.Employee).WithOne().HasForeignKey<EmpState>(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class FileMetaDataConfig : BaseEntityConfig<FileMetaData>
{
    public override void Configure(EntityTypeBuilder<FileMetaData> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FileSize).IsRequired();
        builder.HasIndex(x => x.FileName);
        builder.HasIndex(x => x.ContentType);
    }
}

public class PersonConfig : BaseEntityConfig<Person>
{
    public override void Configure(EntityTypeBuilder<Person> builder)
    {
        base.Configure(builder);
        builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FirstNameAm).HasMaxLength(100);
        builder.Property(x => x.MiddleName).HasMaxLength(100);
        builder.Property(x => x.MiddleNameAm).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastNameAm).HasMaxLength(100);
        builder.Property(x => x.Gender).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Nationality).HasMaxLength(100);
        builder.HasIndex(x => x.Gender);
    }
}