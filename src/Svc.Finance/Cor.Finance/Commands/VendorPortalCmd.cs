// Commands/VendorPortalCmd.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Models.Entities;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Cor.Finance.Queries;
namespace Cor.Finance.Commands;

// ============ PORTAL VENDOR USER COMMANDS ============
public class AddPortalVendorUserCmd : IRequest<PortalVendorUserDto>
{
    public AddPortalVendorUserDto AddDto { get; set; } = new();
}

public class EditPortalVendorUserCmd : IRequest<PortalVendorUserDto>
{
    public EditPortalVendorUserDto EditDto { get; set; } = new();
}

public class DeletePortalVendorUserCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

// ============ PORTAL INVOICE COMMANDS ============
public class AddPortalInvoiceCmd : IRequest<PortalInvoiceDto>
{
    public AddPortalInvoiceDto AddDto { get; set; } = new();
}

public class EditPortalInvoiceCmd : IRequest<PortalInvoiceDto>
{
    public EditPortalInvoiceDto EditDto { get; set; } = new();
}

public class DeletePortalInvoiceCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class ApprovePortalInvoiceCmd : IRequest<PortalInvoiceDto>
{
    public Guid Id { get; set; }
}

public class RejectPortalInvoiceCmd : IRequest<PortalInvoiceDto>
{
    public Guid Id { get; set; }
    public string? Reason { get; set; }
}

// ============ PORTAL NOTIFICATION COMMANDS ============
public class SendPortalNotificationCmd : IRequest<PortalNotificationDto>
{
    public SendPortalNotificationDto SendDto { get; set; } = new();
}

public class MarkPortalNotificationReadCmd : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class MarkAllPortalNotificationsReadCmd : IRequest<bool>
{
    public Guid VendorId { get; set; }
}

// ============ DTOs ============




// ============ HANDLERS ============
public class AddPortalVendorUserHandler : IRequestHandler<AddPortalVendorUserCmd, PortalVendorUserDto>
{
    private readonly FinanceDbContext _context;

    public AddPortalVendorUserHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalVendorUserDto> Handle(AddPortalVendorUserCmd request, CancellationToken ct)
    {
        var exists = await _context.VendorPortalUsers
            .AnyAsync(x => x.Email == request.AddDto.Email && !x.IsDeleted, ct);
        if (exists)
            throw new InvalidOperationException($"Portal vendor user with email '{request.AddDto.Email}' already exists");

        var user = new VendorPortalUser
        {
            Id = Guid.NewGuid(),
            VendorId = request.AddDto.VendorId,
            Email = request.AddDto.Email,
            Phone = request.AddDto.Phone,
            Role = request.AddDto.Role ?? "Submitter",
            Status = "Pending",
            DateAdd = DateTime.UtcNow
        };

        _context.VendorPortalUsers.Add(user);
        await _context.SaveChangesAsync(ct);

        return await new GetPortalVendorByIdHandler(_context).Handle(
            new GetPortalVendorByIdQry { Id = user.Id }, ct);
    }
}

public class AddPortalInvoiceHandler : IRequestHandler<AddPortalInvoiceCmd, PortalInvoiceDto>
{
    private readonly FinanceDbContext _context;

    public AddPortalInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalInvoiceDto> Handle(AddPortalInvoiceCmd request, CancellationToken ct)
    {
        // Check if vendor exists
        var vendor = await _context.Vendors
            .FirstOrDefaultAsync(x => x.Id == request.AddDto.VendorId && !x.IsDeleted, ct);
        if (vendor == null)
            throw new InvalidOperationException($"Vendor with ID '{request.AddDto.VendorId}' not found");

        var invoice = new PortalInvoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = request.AddDto.InvoiceNumber,
            VendorId = request.AddDto.VendorId,
            Amount = request.AddDto.Amount,
            InvoiceDate = request.AddDto.InvoiceDate,
            DueDate = request.AddDto.DueDate,
            Status = "Submitted",
            SubmittedAt = DateTime.UtcNow,
            Notes = request.AddDto.Notes,
            DateAdd = DateTime.UtcNow
        };

        _context.PortalInvoices.Add(invoice);
        await _context.SaveChangesAsync(ct);

        // Send notification to finance team
        await SendNotificationToFinanceTeam(invoice, ct);

        return await new GetPortalInvoiceByIdHandler(_context).Handle(
            new GetPortalInvoiceByIdQry { Id = invoice.Id }, ct);
    }

    private async Task SendNotificationToFinanceTeam(PortalInvoice invoice, CancellationToken ct)
    {
        var notification = new PortalNotification
        {
            Id = Guid.NewGuid(),
            VendorId = invoice.VendorId,
            Type = "Invoice_Submitted",
            Title = "New Invoice Submitted",
            Message = $"Invoice {invoice.InvoiceNumber} for {invoice.Amount:C} has been submitted",
            IsRead = false,
            Link = $"/finance/invoices/{invoice.Id}",
            DateAdd = DateTime.UtcNow
        };

        _context.PortalNotifications.Add(notification);
        await _context.SaveChangesAsync(ct);
    }
}

public class ApprovePortalInvoiceHandler : IRequestHandler<ApprovePortalInvoiceCmd, PortalInvoiceDto>
{
    private readonly FinanceDbContext _context;

    public ApprovePortalInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalInvoiceDto> Handle(ApprovePortalInvoiceCmd request, CancellationToken ct)
    {
        var invoice = await _context.PortalInvoices
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (invoice == null)
            throw new InvalidOperationException($"Portal invoice with ID '{request.Id}' not found");

        if (invoice.Status != "Submitted" && invoice.Status != "UnderReview")
            throw new InvalidOperationException($"Cannot approve invoice with status '{invoice.Status}'");

        invoice.Status = "Approved";
        invoice.ApprovedAt = DateTime.UtcNow;
        invoice.ApprovedBy = "System"; // Would come from current user
        invoice.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        // Send notification to vendor
        var notification = new PortalNotification
        {
            Id = Guid.NewGuid(),
            VendorId = invoice.VendorId,
            Type = "Invoice_Approved",
            Title = "Invoice Approved",
            Message = $"Your invoice {invoice.InvoiceNumber} has been approved",
            IsRead = false,
            Link = $"/portal/invoices/{invoice.Id}",
            DateAdd = DateTime.UtcNow
        };

        _context.PortalNotifications.Add(notification);
        await _context.SaveChangesAsync(ct);

        return await new GetPortalInvoiceByIdHandler(_context).Handle(
            new GetPortalInvoiceByIdQry { Id = invoice.Id }, ct);
    }
}

public class RejectPortalInvoiceHandler : IRequestHandler<RejectPortalInvoiceCmd, PortalInvoiceDto>
{
    private readonly FinanceDbContext _context;

    public RejectPortalInvoiceHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalInvoiceDto> Handle(RejectPortalInvoiceCmd request, CancellationToken ct)
    {
        var invoice = await _context.PortalInvoices
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (invoice == null)
            throw new InvalidOperationException($"Portal invoice with ID '{request.Id}' not found");

        if (invoice.Status != "Submitted" && invoice.Status != "UnderReview")
            throw new InvalidOperationException($"Cannot reject invoice with status '{invoice.Status}'");

        invoice.Status = "Rejected";
        invoice.Notes = (invoice.Notes ?? "") + $" | Rejected: {request.Reason}";
        invoice.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        // Send notification to vendor
        var notification = new PortalNotification
        {
            Id = Guid.NewGuid(),
            VendorId = invoice.VendorId,
            Type = "Invoice_Rejected",
            Title = "Invoice Rejected",
            Message = $"Your invoice {invoice.InvoiceNumber} has been rejected. Reason: {request.Reason}",
            IsRead = false,
            Link = $"/portal/invoices/{invoice.Id}",
            DateAdd = DateTime.UtcNow
        };

        _context.PortalNotifications.Add(notification);
        await _context.SaveChangesAsync(ct);

        return await new GetPortalInvoiceByIdHandler(_context).Handle(
            new GetPortalInvoiceByIdQry { Id = invoice.Id }, ct);
    }
}

public class SendPortalNotificationHandler : IRequestHandler<SendPortalNotificationCmd, PortalNotificationDto>
{
    private readonly FinanceDbContext _context;

    public SendPortalNotificationHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalNotificationDto> Handle(SendPortalNotificationCmd request, CancellationToken ct)
    {
        var notification = new PortalNotification
        {
            Id = Guid.NewGuid(),
            VendorId = request.SendDto.VendorId,
            Type = request.SendDto.Type ?? "Portal_Update",
            Title = request.SendDto.Title,
            Message = request.SendDto.Message,
            IsRead = false,
            Link = request.SendDto.Link,
            DateAdd = DateTime.UtcNow
        };

        _context.PortalNotifications.Add(notification);
        await _context.SaveChangesAsync(ct);

        return await new GetPortalNotificationByIdHandler(_context).Handle(
            new GetPortalNotificationByIdQry { Id = notification.Id }, ct);
    }
}

public class MarkPortalNotificationReadHandler : IRequestHandler<MarkPortalNotificationReadCmd, bool>
{
    private readonly FinanceDbContext _context;

    public MarkPortalNotificationReadHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(MarkPortalNotificationReadCmd request, CancellationToken ct)
    {
        var notification = await _context.PortalNotifications
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.DateMod = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return true;
    }
}

public class MarkAllPortalNotificationsReadHandler : IRequestHandler<MarkAllPortalNotificationsReadCmd, bool>
{
    private readonly FinanceDbContext _context;

    public MarkAllPortalNotificationsReadHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(MarkAllPortalNotificationsReadCmd request, CancellationToken ct)
    {
        var notifications = await _context.PortalNotifications
            .Where(x => x.VendorId == request.VendorId && !x.IsRead)
            .ToListAsync(ct);

        if (!notifications.Any())
            return true;

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.DateMod = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}