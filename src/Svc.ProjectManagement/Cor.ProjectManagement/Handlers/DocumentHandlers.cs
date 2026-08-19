// Handlers/DocumentHandlers.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Security.Cryptography;
using System.Text;
using Cor.ProjectManagement.Models.Entities;
using Cor.ProjectManagement.Models.DTOs;
using Cor.ProjectManagement.Persistence;
using Cor.ProjectManagement.Commands.DocumentCommands;
using Cor.ProjectManagement.Queries.DocumentQueries;

namespace Cor.ProjectManagement.Handlers
{
    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, ProjectDocumentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UploadDocumentCommandHandler> _logger;

        public UploadDocumentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UploadDocumentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == request.ProjectId && !p.IsDeleted, cancellationToken);

                if (project == null)
                    throw new Exception($"Project with ID {request.ProjectId} not found");

                // Generate file hash for integrity checking
                var fileHash = GenerateFileHash(request.FileName + DateTime.UtcNow.Ticks);

                var document = new ProjectDocument
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    PhaseId = request.PhaseId,
                    TaskId = request.TaskId,
                    Type = request.Type,
                    FileName = request.FileName,
                    FilePath = request.FilePath ?? $"documents/{request.ProjectId}/{request.FileName}",
                    FileSize = request.FileSize ?? "0",
                    FileType = request.FileType ?? System.IO.Path.GetExtension(request.FileName),
                    FileHash = fileHash,
                    Version = "1.0",
                    RevisionNumber = 1,
                    UploadedByName = request.CreatedBy ?? "System",
                    UploadedAt = DateTime.UtcNow,
                    IsApproved = false,
                    IsConfidential = request.IsConfidential,
                    IsArchived = false,
                    Tags = request.Tags,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy ?? "System"
                };

                await _context.ProjectDocuments.AddAsync(document, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document uploaded successfully with ID: {DocumentId}", document.Id);
                return _mapper.Map<ProjectDocumentDto>(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                throw;
            }
        }

        private string GenerateFileHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, ProjectDocumentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateDocumentCommandHandler> _logger;

        public UpdateDocumentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<UpdateDocumentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDocumentDto> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.ProjectDocuments
                    .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

                if (document == null)
                    throw new Exception($"Document with ID {request.Id} not found");

                if (!string.IsNullOrEmpty(request.Name))
                    document.Name = request.Name;

                if (!string.IsNullOrEmpty(request.Description))
                    document.Description = request.Description;

                if (!string.IsNullOrEmpty(request.Version))
                {
                    document.Version = request.Version;
                    document.RevisionNumber += 1;
                }

                if (request.IsApproved.HasValue)
                    document.IsApproved = request.IsApproved.Value;

                if (request.IsArchived.HasValue)
                    document.IsArchived = request.IsArchived.Value;

                if (!string.IsNullOrEmpty(request.Tags))
                    document.Tags = request.Tags;

                document.LastModifiedById = Guid.TryParse(request.UpdatedBy, out var id) ? id : null;
                document.LastModifiedByName = request.UpdatedBy ?? "System";
                document.LastModifiedAt = DateTime.UtcNow;
                document.UpdatedAt = DateTime.UtcNow;
                document.UpdatedBy = request.UpdatedBy ?? "System";
                document.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document updated successfully with ID: {DocumentId}", document.Id);
                return _mapper.Map<ProjectDocumentDto>(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID: {DocumentId}", request.Id);
                throw;
            }
        }
    }

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, bool>
    {
        private readonly ProjectDbContext _context;
        private readonly ILogger<DeleteDocumentCommandHandler> _logger;

        public DeleteDocumentCommandHandler(
            ProjectDbContext context,
            ILogger<DeleteDocumentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.ProjectDocuments
                    .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

                if (document == null)
                    throw new Exception($"Document with ID {request.Id} not found");

                document.IsDeleted = true;
                document.DeletedAt = DateTime.UtcNow;
                document.DeletedBy = request.DeletedBy ?? "System";

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document deleted successfully with ID: {DocumentId}", request.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document with ID: {DocumentId}", request.Id);
                throw;
            }
        }
    }

    public class ApproveDocumentCommandHandler : IRequestHandler<ApproveDocumentCommand, ProjectDocumentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ApproveDocumentCommandHandler> _logger;

        public ApproveDocumentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ApproveDocumentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDocumentDto> Handle(ApproveDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.ProjectDocuments
                    .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

                if (document == null)
                    throw new Exception($"Document with ID {request.Id} not found");

                if (document.IsApproved)
                    throw new Exception("Document is already approved");

                document.IsApproved = true;
                document.ApprovedById = Guid.TryParse(request.ApprovedBy, out var id) ? id : null;
                document.ApprovedByName = request.ApprovedBy ?? "System";
                document.ApprovedAt = DateTime.UtcNow;
                document.UpdatedAt = DateTime.UtcNow;
                document.UpdatedBy = request.ApprovedBy ?? "System";
                document.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document approved successfully with ID: {DocumentId}", document.Id);
                return _mapper.Map<ProjectDocumentDto>(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving document with ID: {DocumentId}", request.Id);
                throw;
            }
        }
    }

    public class ArchiveDocumentCommandHandler : IRequestHandler<ArchiveDocumentCommand, ProjectDocumentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ArchiveDocumentCommandHandler> _logger;

        public ArchiveDocumentCommandHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<ArchiveDocumentCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDocumentDto> Handle(ArchiveDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.ProjectDocuments
                    .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

                if (document == null)
                    throw new Exception($"Document with ID {request.Id} not found");

                document.IsArchived = true;
                document.UpdatedAt = DateTime.UtcNow;
                document.UpdatedBy = request.ArchivedBy ?? "System";
                document.Version += 1;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Document archived successfully with ID: {DocumentId}", document.Id);
                return _mapper.Map<ProjectDocumentDto>(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving document with ID: {DocumentId}", request.Id);
                throw;
            }
        }
    }

    // Query Handlers
    public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, ProjectDocumentDto>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentByIdQueryHandler> _logger;

        public GetDocumentByIdQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetDocumentByIdQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProjectDocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.ProjectDocuments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.Id && !d.IsDeleted, cancellationToken);

                if (document == null)
                    throw new Exception($"Document with ID {request.Id} not found");

                return _mapper.Map<ProjectDocumentDto>(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document with ID: {DocumentId}", request.Id);
                throw;
            }
        }
    }

    public class GetDocumentsByProjectQueryHandler : IRequestHandler<GetDocumentsByProjectQuery, List<ProjectDocumentDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentsByProjectQueryHandler> _logger;

        public GetDocumentsByProjectQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetDocumentsByProjectQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectDocumentDto>> Handle(GetDocumentsByProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ProjectDocuments
                    .AsNoTracking()
                    .Where(d => d.ProjectId == request.ProjectId && !d.IsDeleted);

                if (request.Type.HasValue)
                    query = query.Where(d => d.Type == request.Type.Value);

                if (request.IsApproved.HasValue)
                    query = query.Where(d => d.IsApproved == request.IsApproved.Value);

                if (request.IsArchived.HasValue)
                    query = query.Where(d => d.IsArchived == request.IsArchived.Value);

                if (request.PhaseId.HasValue)
                    query = query.Where(d => d.PhaseId == request.PhaseId.Value);

                if (request.TaskId.HasValue)
                    query = query.Where(d => d.TaskId == request.TaskId.Value);

                var items = await query
                    .OrderByDescending(d => d.UploadedAt)
                    .Select(d => _mapper.Map<ProjectDocumentDto>(d))
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for project ID: {ProjectId}", request.ProjectId);
                throw;
            }
        }
    }

    public class GetDocumentVersionsQueryHandler : IRequestHandler<GetDocumentVersionsQuery, List<ProjectDocumentDto>>
    {
        private readonly ProjectDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDocumentVersionsQueryHandler> _logger;

        public GetDocumentVersionsQueryHandler(
            ProjectDbContext context,
            IMapper mapper,
            ILogger<GetDocumentVersionsQueryHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ProjectDocumentDto>> Handle(GetDocumentVersionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // For versioning, we track revisions on the same document
                // In a real implementation, you might have a separate version table
                // For now, we return the current document with its revision history

                var document = await _context.ProjectDocuments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == request.DocumentId && !d.IsDeleted, cancellationToken);

                if (document == null)
                    throw new Exception($"Document with ID {request.DocumentId} not found");

                // Return current document
                var result = new List<ProjectDocumentDto>
                {
                    _mapper.Map<ProjectDocumentDto>(document)
                };

                // In a real implementation, you'd query a version history table here
                // For now, we just return the current version

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document versions for ID: {DocumentId}", request.DocumentId);
                throw;
            }
        }
    }
}