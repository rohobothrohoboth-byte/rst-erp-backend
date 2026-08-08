// Recruit.App/Commands/PublicCmd.cs

using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recruit.App.Interfaces;
using Recruit.App.Queries;
using Recruit.Domain.DTOs;
using Recruit.Domain.Entities;

namespace Recruit.App.Commands;

/// <summary>
/// Command to register an external applicant
/// </summary>
public class RegisterExternalApplicantCmd : IRequest<ExternalApplicantRegistrationResponseDto>
{
    public ExternalApplicantRegistrationDto Dto { get; set; } = default!;
}

/// <summary>
/// Command to apply for a job externally
/// </summary>
public class JobAppExtAddCmd : IRequest<JobAppListDto>
{
    public JobAppExtAddDto AddDto { get; set; } = default!;
}

/// <summary>
/// Command to update external applicant profile
/// </summary>
public class UpdateExternalApplicantCmd : IRequest<ExternalApplicantRegistrationResponseDto>
{
    public ExternalApplicantUpdateDto Dto { get; set; } = default!;
}

/// <summary>
/// Handler for registering external applicants
/// </summary>
public class RegisterExternalApplicantHandler : IRequestHandler<RegisterExternalApplicantCmd, ExternalApplicantRegistrationResponseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public RegisterExternalApplicantHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<ExternalApplicantRegistrationResponseDto> Handle(RegisterExternalApplicantCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // Check if applicant already exists with this email
            var existingContact = await _uow.Set<ApplicantContact>()
                .FirstOrDefaultAsync(c => c.Email == request.Dto.Email && !c.IsDeleted, ct);

            if (existingContact != null)
            {
                throw new DomainException($"An applicant with email '{request.Dto.Email}' already exists.");
            }

            // Create ApplicantPerson
            var person = new ApplicantPerson
            {
                Id = Guid.CreateVersion7(),
                FirstName = request.Dto.FirstName,
                FirstNameAm = request.Dto.FirstNameAm?? string.Empty,
                MiddleName = request.Dto.MiddleName,
                MiddleNameAm = request.Dto.MiddleNameAm?? string.Empty,
                LastName = request.Dto.LastName,
                LastNameAm = request.Dto.LastNameAm?? string.Empty,
                Gender = request.Dto.Gender,
                Nationality = request.Dto.Nationality,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(person, ct);

            // Create ApplicantContact
            var contact = new ApplicantContact
            {
                Id = Guid.CreateVersion7(),
                Email = request.Dto.Email,
                Phone = request.Dto.Phone,
                AlternatePhone = request.Dto.AlternatePhone,
                PoBox = request.Dto.PoBox?? string.Empty,
                Fax = request.Dto.Fax?? string.Empty,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(contact, ct);

            // Create ApplicantAddress (optional)
            Guid? addressId = null;
            if (!string.IsNullOrEmpty(request.Dto.Country) || !string.IsNullOrEmpty(request.Dto.Region))
            {
                var address = new ApplicantAddress
                {
                    Id = Guid.CreateVersion7(),
                    Country = request.Dto.Country ?? "N/A",
                    Region = request.Dto.Region?? string.Empty,
                    Zone = request.Dto.Zone?? string.Empty,
                    Woreda = request.Dto.Woreda?? string.Empty,
                    Subcity = request.Dto.Subcity?? string.Empty,
                    Kebele = request.Dto.Kebele?? string.Empty,
                    HouseNo = request.Dto.HouseNo?? string.Empty,
                    AddressType = request.Dto.AddressType ?? "Residence",
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };
                await _uow.Add(address, ct);
                addressId = address.Id;
            }

            // ✅ Create Applicant - PersonId and ContactId are Guid, not nullable
            var applicant = new Applicant
            {
                Id = Guid.CreateVersion7(),
                PersonId = person.Id,
                ContactId = contact.Id,
                AddressId = addressId ?? Guid.Empty,
                RegisteredDate = DateTime.UtcNow,
                RegisteredBy = "EXTERNAL",
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(applicant, ct);

            await _uow.Commit(ct);

            var fullName = $"{request.Dto.FirstName} {request.Dto.MiddleName} {request.Dto.LastName}";

            return new ExternalApplicantRegistrationResponseDto
            {
                ApplicantId = applicant.Id,
                PersonId = person.Id,
                ContactId = contact.Id,
                AddressId = addressId,
                FullName = fullName,
                Email = request.Dto.Email,
                Message = "Applicant registered successfully."
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

/// <summary>
/// Handler for external job applications
/// </summary>
public class JobAppExtAddHandler : IRequestHandler<JobAppExtAddCmd, JobAppListDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public JobAppExtAddHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<JobAppListDto> Handle(JobAppExtAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // Validate applicant exists
            var applicant = await _uow.Set<Applicant>()
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.ApplicantId && !x.IsDeleted, ct);

            if (applicant == null)
                throw new DomainException($"Applicant with Id {request.AddDto.ApplicantId} NOT FOUND.");

            // Validate job posting exists
            var jobPosting = await _uow.Set<JobPosting>()
                .FirstOrDefaultAsync(x => x.Id == request.AddDto.JobPostingId && !x.IsDeleted, ct);

            if (jobPosting == null)
                throw new DomainException($"Job Posting with Id {request.AddDto.JobPostingId} NOT FOUND.");

            // Check if already applied
            var existingApp = await _uow.Set<JobApplication>()
                .FirstOrDefaultAsync(x => x.ApplicantId == request.AddDto.ApplicantId &&
                                          x.JobPostingId == request.AddDto.JobPostingId &&
                                          !x.IsDeleted, ct);

            if (existingApp != null)
                throw new DomainException("You have already applied for this position.");

            // Create Job Application
            var jobApp = new JobApplication
            {
                Id = Guid.CreateVersion7(),
                ApplicantId = request.AddDto.ApplicantId,
                JobPostingId = request.AddDto.JobPostingId,
                Status = BoolToStr.EnumToString(ApplicationStatus.Applied),
                PostType = jobPosting.PostType,
                AppliedDate = DateTime.UtcNow,
                DateAdd = DateTime.UtcNow,
                DateMod = null,
                IsDeleted = false
            };
            await _uow.Add(jobApp, ct);

            // Create Cover Letter
            if (!string.IsNullOrEmpty(request.AddDto.CoverLetter))
            {
                var coverLetter = new CoverLetter
                {
                    JobAppId = jobApp.Id,
                    Content = request.AddDto.CoverLetter,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };
                await _uow.Add(coverLetter, ct);
            }

            // Create Resume if uploaded
            if (request.AddDto.File != null)
            {
                var resume = new Resume
                {
                    FileName = request.AddDto.File.FileName,
                    ContentType = request.AddDto.File.ContentType,
                    FileSize = request.AddDto.File.Length,
                    JobAppId = jobApp.Id,
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };
                await _uow.Add(resume, ct);

                using var ms = new MemoryStream();
                await request.AddDto.File.CopyToAsync(ms, ct);
                ms.Position = 0;
                var blob = new ResumeBlob
                {
                    ResumeId = resume.Id,
                    Data = ms.ToArray(),
                    DateAdd = DateTime.UtcNow,
                    DateMod = null,
                    IsDeleted = false
                };
                await _uow.Add(blob, ct);
            }

            await _uow.Commit(ct);

            // Get the created application
            var response = await _med.Send(new JobAppByIdQry { Id = jobApp.Id }, ct);
            return response ?? new JobAppListDto();
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

/// <summary>
/// Handler for updating external applicant profile
/// </summary>
public class UpdateExternalApplicantHandler : IRequestHandler<UpdateExternalApplicantCmd, ExternalApplicantRegistrationResponseDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMediator _med;

    public UpdateExternalApplicantHandler(IUnitOfWork uow, IMediator med)
    {
        _uow = uow;
        _med = med;
    }

    public async Task<ExternalApplicantRegistrationResponseDto> Handle(UpdateExternalApplicantCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var applicant = await _uow.Set<Applicant>()
                .FirstOrDefaultAsync(x => x.Id == request.Dto.ApplicantId && !x.IsDeleted, ct);

            if (applicant == null)
                throw new DomainException($"Applicant with Id {request.Dto.ApplicantId} NOT FOUND.");

            // Update Person
            var person = await _uow.Set<ApplicantPerson>()
                .FirstOrDefaultAsync(x => x.Id == applicant.PersonId && !x.IsDeleted, ct);

            if (person != null)
            {
                if (!string.IsNullOrEmpty(request.Dto.FirstName))
                    person.FirstName = request.Dto.FirstName;
                if (!string.IsNullOrEmpty(request.Dto.MiddleName))
                    person.MiddleName = request.Dto.MiddleName;
                if (!string.IsNullOrEmpty(request.Dto.LastName))
                    person.LastName = request.Dto.LastName;
                if (!string.IsNullOrEmpty(request.Dto.Gender))
                    person.Gender = request.Dto.Gender;
                if (!string.IsNullOrEmpty(request.Dto.Nationality))
                    person.Nationality = request.Dto.Nationality;
                person.DateMod = DateTime.UtcNow;
                await _uow.Update(person);
            }

            // Update Contact
            var contact = await _uow.Set<ApplicantContact>()
                .FirstOrDefaultAsync(x => x.Id == applicant.ContactId && !x.IsDeleted, ct);

            if (contact != null)
            {
                if (!string.IsNullOrEmpty(request.Dto.Email))
                    contact.Email = request.Dto.Email;
                if (!string.IsNullOrEmpty(request.Dto.Phone))
                    contact.Phone = request.Dto.Phone;
                contact.DateMod = DateTime.UtcNow;
                await _uow.Update(contact);
            }

            await _uow.Commit(ct);

            // ✅ PersonId and ContactId are Guid (not nullable), so use them directly
            return new ExternalApplicantRegistrationResponseDto
            {
                ApplicantId = applicant.Id,
                PersonId = applicant.PersonId,
                ContactId = applicant.ContactId,
                AddressId = applicant.AddressId,
                FullName = $"{person?.FirstName ?? ""} {person?.MiddleName ?? ""} {person?.LastName ?? ""}".Trim(),
                Email = contact?.Email ?? "",
                Message = "Profile updated successfully."
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}