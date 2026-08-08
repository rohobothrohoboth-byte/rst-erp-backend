// Cor.CRM/Commands/ContactCmd.cs

using Cor.CRM.Interfaces;
using Cor.CRM.Models.DTOs;
using Cor.CRM.Models.Entities;
using Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
namespace Cor.CRM.Commands;

// ============================================================
// COMMANDS
// ============================================================

public class ContactAddCmd : IRequest<ContactDto>
{
    public CreateContactDto Dto { get; set; } = default!;
}

public class ContactModCmd : IRequest<ContactDto>
{
    public Guid Id { get; set; }
    public UpdateContactDto Dto { get; set; } = default!;
}

public class ContactDelCmd : IRequest
{
    public Guid Id { get; set; }
}

// ============================================================
// ADD CONTACT
// ============================================================

public class ContactAddHandler : IRequestHandler<ContactAddCmd, ContactDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContactAddHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ContactDto> Handle(ContactAddCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            // If this contact is marked as primary, update any existing primary contacts
            if (request.Dto.IsPrimary && request.Dto.CustomerId.HasValue)
            {
                var existingPrimary = await _uow.Set<Contact>()
                    .FirstOrDefaultAsync(x => x.CustomerId == request.Dto.CustomerId && x.IsPrimary && !x.IsDeleted, ct);

                if (existingPrimary != null)
                {
                    existingPrimary.IsPrimary = false;
                    await _uow.Update(existingPrimary);
                }
            }

            var contact = new Contact
            {
                Id = Guid.CreateVersion7(),
                FirstName = request.Dto.FirstName,
                LastName = request.Dto.LastName,
                Email = request.Dto.Email,
                Phone = request.Dto.Phone,
                Mobile = request.Dto.Mobile,
                Title = request.Dto.Title,
                Department = request.Dto.Department,
                CustomerId = request.Dto.CustomerId,
                IsPrimary = request.Dto.IsPrimary,
                IsDecisionMaker = request.Dto.IsDecisionMaker,
                Notes = request.Dto.Notes,
                AcceptsEmail = request.Dto.AcceptsEmail,
                AcceptsSMS = request.Dto.AcceptsSMS,
                AcceptsCalls = request.Dto.AcceptsCalls,
                AcceptsMarketing = request.Dto.AcceptsMarketing,
                PreferredContactMethod = request.Dto.PreferredContactMethod,
                LinkedIn = request.Dto.LinkedIn,
                Twitter = request.Dto.Twitter,
                Facebook = request.Dto.Facebook,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _uow.Add(contact, ct);
            await _uow.Commit(ct);

            _logger.LogInformation("Contact created: {ContactId} - {ContactName}", contact.Id, contact.FullName);

            return new ContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                FullName = contact.FullName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Title = contact.Title,
                Department = contact.Department,
                CustomerId = contact.CustomerId,
                IsPrimary = contact.IsPrimary,
                IsDecisionMaker = contact.IsDecisionMaker,
                Notes = contact.Notes,
                AcceptsEmail = contact.AcceptsEmail,
                AcceptsSMS = contact.AcceptsSMS,
                AcceptsCalls = contact.AcceptsCalls,
                AcceptsMarketing = contact.AcceptsMarketing,
                PreferredContactMethod = contact.PreferredContactMethod,
                LinkedIn = contact.LinkedIn,
                Twitter = contact.Twitter,
                Facebook = contact.Facebook,
                IsActive = contact.IsActive,
                CreatedAt = contact.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// UPDATE CONTACT
// ============================================================

public class ContactModHandler : IRequestHandler<ContactModCmd, ContactDto>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContactModHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<ContactDto> Handle(ContactModCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var contact = await _uow.Set<Contact>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (contact == null)
            {
                throw new DomainException($"Contact with id [{request.Id}] NOT FOUND.");
            }

            // If this contact is being marked as primary, update existing primary
            if (request.Dto.IsPrimary.HasValue && request.Dto.IsPrimary.Value && contact.CustomerId.HasValue)
            {
                var existingPrimary = await _uow.Set<Contact>()
                    .FirstOrDefaultAsync(x => x.CustomerId == contact.CustomerId && x.IsPrimary && x.Id != request.Id && !x.IsDeleted, ct);

                if (existingPrimary != null)
                {
                    existingPrimary.IsPrimary = false;
                    await _uow.Update(existingPrimary);
                }
            }

            if (request.Dto.FirstName != null)
                contact.FirstName = request.Dto.FirstName;
            if (request.Dto.LastName != null)
                contact.LastName = request.Dto.LastName;
            if (request.Dto.Email != null)
                contact.Email = request.Dto.Email;
            if (request.Dto.Phone != null)
                contact.Phone = request.Dto.Phone;
            if (request.Dto.Mobile != null)
                contact.Mobile = request.Dto.Mobile;
            if (request.Dto.Title != null)
                contact.Title = request.Dto.Title;
            if (request.Dto.Department != null)
                contact.Department = request.Dto.Department;
            if (request.Dto.CustomerId.HasValue)
                contact.CustomerId = request.Dto.CustomerId;
            if (request.Dto.IsPrimary.HasValue)
                contact.IsPrimary = request.Dto.IsPrimary.Value;
            if (request.Dto.IsDecisionMaker.HasValue)
                contact.IsDecisionMaker = request.Dto.IsDecisionMaker.Value;
            if (request.Dto.Notes != null)
                contact.Notes = request.Dto.Notes;
            if (request.Dto.AcceptsEmail.HasValue)
                contact.AcceptsEmail = request.Dto.AcceptsEmail.Value;
            if (request.Dto.AcceptsSMS.HasValue)
                contact.AcceptsSMS = request.Dto.AcceptsSMS.Value;
            if (request.Dto.AcceptsCalls.HasValue)
                contact.AcceptsCalls = request.Dto.AcceptsCalls.Value;
            if (request.Dto.AcceptsMarketing.HasValue)
                contact.AcceptsMarketing = request.Dto.AcceptsMarketing.Value;
            if (request.Dto.PreferredContactMethod != null)
                contact.PreferredContactMethod = request.Dto.PreferredContactMethod;
            if (request.Dto.LinkedIn != null)
                contact.LinkedIn = request.Dto.LinkedIn;
            if (request.Dto.Twitter != null)
                contact.Twitter = request.Dto.Twitter;
            if (request.Dto.Facebook != null)
                contact.Facebook = request.Dto.Facebook;
            if (request.Dto.IsActive.HasValue)
                contact.IsActive = request.Dto.IsActive.Value;

            contact.UpdatedAt = DateTime.UtcNow;

            await _uow.Update(contact);
            await _uow.Commit(ct);

            _logger.LogInformation("Contact updated: {ContactId} - {ContactName}", contact.Id, contact.FullName);

            return new ContactDto
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                FullName = contact.FullName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Title = contact.Title,
                Department = contact.Department,
                CustomerId = contact.CustomerId,
                IsPrimary = contact.IsPrimary,
                IsDecisionMaker = contact.IsDecisionMaker,
                Notes = contact.Notes,
                AcceptsEmail = contact.AcceptsEmail,
                AcceptsSMS = contact.AcceptsSMS,
                AcceptsCalls = contact.AcceptsCalls,
                AcceptsMarketing = contact.AcceptsMarketing,
                PreferredContactMethod = contact.PreferredContactMethod,
                LinkedIn = contact.LinkedIn,
                Twitter = contact.Twitter,
                Facebook = contact.Facebook,
                IsActive = contact.IsActive,
                UpdatedAt = contact.UpdatedAt,
                CreatedAt = contact.CreatedAt
            };
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}

// ============================================================
// DELETE CONTACT
// ============================================================

public class ContactDelHandler : IRequestHandler<ContactDelCmd>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogService _logger;

    public ContactDelHandler(IUnitOfWork uow, ILogService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(ContactDelCmd request, CancellationToken ct)
    {
        await _uow.Begin(ct);
        try
        {
            var contact = await _uow.Set<Contact>()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

            if (contact == null)
            {
                throw new DomainException($"Contact with id [{request.Id}] NOT FOUND.");
            }

            await _uow.Delete(contact);
            await _uow.Commit(ct);

            _logger.LogInformation("Contact deleted: {ContactId} - {ContactName}", contact.Id, contact.FullName);
        }
        catch
        {
            await _uow.Rollback(ct);
            throw;
        }
    }
}