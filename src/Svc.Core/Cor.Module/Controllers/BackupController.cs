using System.Diagnostics;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Cor.Module.Controllers;

[ApiController]
[Route("api/core/v{version:apiVersion}/Backup")]
[ApiVersion("1.0")]
[Authorize(Roles = "admin,System Administrator")]
public sealed class BackupController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<BackupController> _logger;
    public BackupController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<BackupController> logger) { _configuration = configuration; _environment = environment; _logger = logger; }
    private string RootPath => Path.GetFullPath(_configuration["Backup:RootPath"] ?? Path.Combine(_environment.ContentRootPath, "backups"));
    private int RetentionDays => Math.Max(1, int.TryParse(_configuration["Backup:RetentionDays"], out var days) ? days : 30);
    private string PgDump => _configuration["Backup:PgDumpPath"] ?? "pg_dump";
    private string PgRestore => _configuration["Backup:PgRestorePath"] ?? "pg_restore";

    [HttpGet("List")]
    public IActionResult List() { Directory.CreateDirectory(RootPath); var files = Directory.EnumerateFiles(RootPath, "*.dump", SearchOption.TopDirectoryOnly).Select(ToBackup).OrderByDescending(x => x.CreatedAt).ToList(); return Ok(new { items = files, total = files.Count, retentionDays = RetentionDays }); }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        Directory.CreateDirectory(RootPath); var connection = GetConnectionString(); var fileName = $"erp-core-full-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.dump"; var outputPath = Path.Combine(RootPath, fileName);
        await RunPgTool(PgDump, BuildDumpArguments(connection, outputPath), connection.Password, ct); ApplyRetention(); _logger.LogInformation("Created ERP core database backup {FileName}", fileName); var info = new FileInfo(outputPath);
        return Ok(new { id = fileName, name = fileName, status = "Completed", createdAt = info.CreationTimeUtc, sizeBytes = info.Length });
    }

    [HttpGet("Download/{fileName}")]
    public IActionResult Download(string fileName) { var path = SafePath(fileName); if (!System.IO.File.Exists(path)) return NotFound(); return PhysicalFile(path, "application/octet-stream", Path.GetFileName(path), enableRangeProcessing: true); }

    [HttpPost("Upload")]
    [RequestSizeLimit(10L * 1024 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0) return BadRequest("Backup file is required."); if (!Path.GetExtension(file.FileName).Equals(".dump", StringComparison.OrdinalIgnoreCase)) return BadRequest("Only PostgreSQL .dump files are supported.");
        Directory.CreateDirectory(RootPath); var safeName = $"uploaded-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.dump"; var path = Path.Combine(RootPath, safeName); await using var stream = System.IO.File.Create(path); await file.CopyToAsync(stream, ct);
        return Ok(new
        {
            id = safeName,
            name = safeName,
            status = "Uploaded",
            createdAt = System.IO.File.GetCreationTimeUtc(path),
            sizeBytes = file.Length
        });
    }

    [HttpPost("Restore/{fileName}")]
    public async Task<IActionResult> Restore(string fileName, [FromQuery] bool confirm = false, CancellationToken ct = default)
    {
        if (!confirm) return BadRequest("Restore requires confirm=true."); var path = SafePath(fileName); if (!System.IO.File.Exists(path)) return NotFound(); var connection = GetConnectionString();
        await RunPgTool(PgRestore, BuildRestoreArguments(connection, path), connection.Password, ct); _logger.LogWarning("ERP core database restored from backup {FileName}", fileName); return Ok(new { name = fileName, status = "Restored" });
    }

    [HttpDelete("{fileName}")]
    public IActionResult Delete(string fileName) { var path = SafePath(fileName); if (!System.IO.File.Exists(path)) return NotFound(); System.IO.File.Delete(path); return Ok(new { name = fileName, status = "Deleted" }); }

    private BackupItem ToBackup(string path) { var info = new FileInfo(path); return new BackupItem(info.Name, info.Name, info.Length, info.CreationTimeUtc, "Completed"); }
    private string SafePath(string fileName) { var name = Path.GetFileName(fileName); if (!name.Equals(fileName, StringComparison.Ordinal) || !name.EndsWith(".dump", StringComparison.OrdinalIgnoreCase)) throw new BadHttpRequestException("Invalid backup file name."); return Path.Combine(RootPath, name); }
    private NpgsqlConnectionStringBuilder GetConnectionString() { var value = _configuration["ConnectionStrings:CorModuleDbCon"]; if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException("ConnectionStrings:CorModuleDbCon is not configured."); return new NpgsqlConnectionStringBuilder(value); }
    private static string BuildDumpArguments(NpgsqlConnectionStringBuilder c, string output) => $"--format=custom --no-owner --no-privileges --host=\"{c.Host}\" --port={c.Port} --username=\"{c.Username}\" --dbname=\"{c.Database}\" --file=\"{output}\"";
    private static string BuildRestoreArguments(NpgsqlConnectionStringBuilder c, string input) => $"--clean --if-exists --no-owner --no-privileges --host=\"{c.Host}\" --port={c.Port} --username=\"{c.Username}\" --dbname=\"{c.Database}\" \"{input}\"";
    private static async Task RunPgTool(string executable, string arguments, string? password, CancellationToken ct) { var psi = new ProcessStartInfo { FileName = executable, Arguments = arguments, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true }; if (!string.IsNullOrEmpty(password)) psi.Environment["PGPASSWORD"] = password; using var process = new Process { StartInfo = psi }; process.Start(); var stdout = process.StandardOutput.ReadToEndAsync(ct); var stderr = process.StandardError.ReadToEndAsync(ct); await process.WaitForExitAsync(ct); var error = await stderr; if (process.ExitCode != 0) throw new InvalidOperationException($"PostgreSQL backup operation failed: {error.Trim()}"); await stdout; }
    private void ApplyRetention()
    {
        var cutoff = DateTime.UtcNow.AddDays(-RetentionDays);

        foreach (var file in Directory.EnumerateFiles(RootPath, "*.dump"))
            if (System.IO.File.GetCreationTimeUtc(file) < cutoff)
                try
                {
                    System.IO.File.Delete(file);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unable to remove expired backup {File}", file);
                }
    }
    private sealed record BackupItem(string Id, string Name, long SizeBytes, DateTime CreatedAt, string Status);
}
