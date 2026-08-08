// Queries/VendorPortalQry.cs
using Cor.Finance.Models.DTOs;
using Cor.Finance.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cor.Finance.Queries;

// ============ PORTAL VENDOR USER QUERIES ============
public class GetAllPortalVendorsQry : IRequest<List<PortalVendorUserDto>>
{
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
}

public class GetPortalVendorByIdQry : IRequest<PortalVendorUserDto>
{
    public Guid Id { get; set; }
}

// ============ PORTAL INVOICE QUERIES ============
public class GetAllPortalInvoicesQry : IRequest<List<PortalInvoiceDto>>
{
    public string? Status { get; set; }
    public Guid? VendorId { get; set; }
   public DateTime? FromDate { get; set; } // ✅ ADD THIS
      public DateTime? ToDate { get; set; } // ✅ ADD THIS
}

public class GetPortalInvoiceByIdQry : IRequest<PortalInvoiceDto>
{
    public Guid Id { get; set; }
}

public class GetPortalInvoiceTrackingQry : IRequest<PortalInvoiceTrackingDto>
{
    public Guid Id { get; set; }
}

// ============ PORTAL NOTIFICATION QUERIES ============
public class GetAllPortalNotificationsQry : IRequest<List<PortalNotificationDto>>
{
    public Guid? VendorId { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? FromDate { get; set; } // ✅ ADD THIS
    public DateTime? ToDate { get; set; } // ✅ ADD THIS
}

public class GetPortalNotificationByIdQry : IRequest<PortalNotificationDto>
{
    public Guid Id { get; set; }
}

// ============ DTO ============


// ============ HANDLERS ============
public class GetAllPortalVendorsHandler : IRequestHandler<GetAllPortalVendorsQry, List<PortalVendorUserDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllPortalVendorsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PortalVendorUserDto>> Handle(GetAllPortalVendorsQry request, CancellationToken ct)
    {
        var query = _context.VendorPortalUsers
            .Include(x => x.Vendor)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (request.VendorId.HasValue)
            query = query.Where(x => x.VendorId == request.VendorId.Value);

        return await query
            .Select(x => new PortalVendorUserDto
            {
                Id = x.Id,
                VendorId = x.VendorId,
                VendorName = x.Vendor != null ? x.Vendor.Name : null,
                Email = x.Email,
                Phone = x.Phone,
                Role = x.Role,
                Status = x.Status,
                LastLogin = x.LastLogin,
                DateAdd = x.DateAdd
            })
            .ToListAsync(ct);
    }
}

public class GetPortalVendorByIdHandler : IRequestHandler<GetPortalVendorByIdQry, PortalVendorUserDto>
{
    private readonly FinanceDbContext _context;

    public GetPortalVendorByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalVendorUserDto> Handle(GetPortalVendorByIdQry request, CancellationToken ct)
    {
        var user = await _context.VendorPortalUsers
            .Include(x => x.Vendor)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (user == null)
            throw new InvalidOperationException($"Portal vendor user with ID '{request.Id}' not found");

        return new PortalVendorUserDto
        {
            Id = user.Id,
            VendorId = user.VendorId,
             VendorName = user.Vendor != null ? user.Vendor.Name : null,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            LastLogin = user.LastLogin,
            DateAdd = user.DateAdd
        };
    }
}

public class GetAllPortalInvoicesHandler : IRequestHandler<GetAllPortalInvoicesQry, List<PortalInvoiceDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllPortalInvoicesHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PortalInvoiceDto>> Handle(GetAllPortalInvoicesQry request, CancellationToken ct)
    {
        var query = _context.PortalInvoices
            .Include(x => x.Vendor)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Status))
            query = query.Where(x => x.Status == request.Status);

        if (request.VendorId.HasValue)
            query = query.Where(x => x.VendorId == request.VendorId.Value);

        if (request.FromDate.HasValue)
            query = query.Where(x => x.InvoiceDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.InvoiceDate <= request.ToDate.Value);

        return await query
            .Select(x => new PortalInvoiceDto
            {
                Id = x.Id,
                InvoiceNumber = x.InvoiceNumber,
                VendorId = x.VendorId,

                 VendorName = x.Vendor != null ? x.Vendor.Name : null,
                Amount = x.Amount,
                InvoiceDate = x.InvoiceDate,
                DueDate = x.DueDate,
                Status = x.Status,
                SubmittedBy = x.SubmittedBy,
                SubmittedAt = x.SubmittedAt,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt,
                PaymentDate = x.PaymentDate,
                PaymentReference = x.PaymentReference,
                Notes = x.Notes,
                DateAdd = x.DateAdd
            })
            .ToListAsync(ct);
    }
}

public class GetPortalInvoiceByIdHandler : IRequestHandler<GetPortalInvoiceByIdQry, PortalInvoiceDto>
{
    private readonly FinanceDbContext _context;

    public GetPortalInvoiceByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalInvoiceDto> Handle(GetPortalInvoiceByIdQry request, CancellationToken ct)
    {
        var invoice = await _context.PortalInvoices
            .Include(x => x.Vendor)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (invoice == null)
            throw new InvalidOperationException($"Portal invoice with ID '{request.Id}' not found");

        return new PortalInvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            VendorId = invoice.VendorId,

            VendorName = invoice.Vendor != null ?invoice.Vendor.Name : null,
            Amount = invoice.Amount,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            SubmittedBy = invoice.SubmittedBy,
            SubmittedAt = invoice.SubmittedAt,
            ApprovedBy = invoice.ApprovedBy,
            ApprovedAt = invoice.ApprovedAt,
            PaymentDate = invoice.PaymentDate,
            PaymentReference = invoice.PaymentReference,
            Notes = invoice.Notes,
            DateAdd = invoice.DateAdd
        };
    }
}

public class GetPortalInvoiceTrackingHandler : IRequestHandler<GetPortalInvoiceTrackingQry, PortalInvoiceTrackingDto>
{
    private readonly FinanceDbContext _context;

    public GetPortalInvoiceTrackingHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalInvoiceTrackingDto> Handle(GetPortalInvoiceTrackingQry request, CancellationToken ct)
    {
        var invoice = await _context.PortalInvoices
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (invoice == null)
            throw new InvalidOperationException($"Portal invoice with ID '{request.Id}' not found");

        var events = new List<TrackingEventDto>();

        // Build tracking events based on invoice status
        if (invoice.SubmittedAt.HasValue)
        {
            events.Add(new TrackingEventDto
            {
                Timestamp = invoice.SubmittedAt.Value,
                Status = "Submitted",
                Description = "Invoice submitted by vendor",
                PerformedBy = invoice.SubmittedBy
            });
        }

        if (invoice.ApprovedAt.HasValue)
        {
            events.Add(new TrackingEventDto
            {
                Timestamp = invoice.ApprovedAt.Value,
                Status = "Approved",
                Description = "Invoice approved",
                PerformedBy = invoice.ApprovedBy
            });
        }

        if (invoice.PaymentDate.HasValue)
        {
            events.Add(new TrackingEventDto
            {
                Timestamp = invoice.PaymentDate.Value,
                Status = "Paid",
                Description = "Payment processed",
                PerformedBy = "System"
            });
        }

        return new PortalInvoiceTrackingDto
        {
            InvoiceId = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            Status = invoice.Status ?? "Submitted",
            Events = events.OrderBy(e => e.Timestamp).ToList()
        };
    }
}

public class GetAllPortalNotificationsHandler : IRequestHandler<GetAllPortalNotificationsQry, List<PortalNotificationDto>>
{
    private readonly FinanceDbContext _context;

    public GetAllPortalNotificationsHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<PortalNotificationDto>> Handle(GetAllPortalNotificationsQry request, CancellationToken ct)
    {
        var query = _context.PortalNotifications
            .Include(x => x.Vendor)
            .AsQueryable();

        if (request.VendorId.HasValue)
            query = query.Where(x => x.VendorId == request.VendorId.Value);

        if (request.IsRead.HasValue)
            query = query.Where(x => x.IsRead == request.IsRead.Value);

        return await query
            .OrderByDescending(x => x.DateAdd)
            .Select(x => new PortalNotificationDto
            {
                Id = x.Id,
                VendorId = x.VendorId,
               VendorName = x.Vendor != null ? x.Vendor.Name : string.Empty,
                Type = x.Type,
                Title = x.Title,
                Message = x.Message,
                IsRead = x.IsRead,
                Link = x.Link,
                DateAdd = x.DateAdd
            })
            .ToListAsync(ct);
    }
}

public class GetPortalNotificationByIdHandler : IRequestHandler<GetPortalNotificationByIdQry, PortalNotificationDto>
{
    private readonly FinanceDbContext _context;

    public GetPortalNotificationByIdHandler(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task<PortalNotificationDto> Handle(GetPortalNotificationByIdQry request, CancellationToken ct)
    {
        var notification = await _context.PortalNotifications
            .Include(x => x.Vendor)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (notification == null)
            throw new InvalidOperationException($"Portal notification with ID '{request.Id}' not found");

        return new PortalNotificationDto
        {
            Id = notification.Id,
            VendorId = notification.VendorId,
             VendorName =notification.Vendor != null ? notification.Vendor.Name : string.Empty,

            Type = notification.Type,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            Link = notification.Link,
            DateAdd = notification.DateAdd
        };
    }
}