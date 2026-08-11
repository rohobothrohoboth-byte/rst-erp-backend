using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Svc.Auth.Interfaces;
using Svc.Auth.Models.Dtos;
using Svc.Auth.Models.Entities;
using Svc.Auth.Seeder;
using Dapper;
using Helpers;

namespace Svc.Auth.Commands.Setup;

public class CompleteSetupCmd : IRequest<SetupResultDto>
{
    public SetupCompanyDto Company { get; set; } = new();
    public SetupBranchDto Branch { get; set; } = new();
    public SetupDepartmentDto Department { get; set; } = new();
    public SetupPositionDto Position { get; set; } = new();
    public SetupUserDto AdminUser { get; set; } = new();
}

public class SetupCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}

public class SetupBranchDto
{
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string BranchType { get; set; } = "Main";
}

public class SetupDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
}

public class SetupPositionDto
{
    public string Name { get; set; } = string.Empty;
    public string NameAm { get; set; } = string.Empty;
    public string JobGradeName { get; set; } = "Senior";
    public int NoOfPosition { get; set; } = 1;
}

public class SetupUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string FirstNameAm { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string MiddleNameAm { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string LastNameAm { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = "Male";
    public string Nationality { get; set; } = string.Empty;
}

public class SetupResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? CompanyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? JobGradeId { get; set; }
    public Guid? EmployeeId { get; set; }
    public Guid? AdminUserId { get; set; }
    public List<string> Details { get; set; } = new();
}

public class CompleteSetupHandler : IRequestHandler<CompleteSetupCmd, SetupResultDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _configuration;

    public CompleteSetupHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IUnitOfWork uow,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _uow = uow;
        _configuration = configuration;
    }

    public async Task<SetupResultDto> Handle(CompleteSetupCmd request, CancellationToken ct)
    {
        var result = new SetupResultDto();
        var details = new List<string>();

        // Foreign-database connections/transactions. Setup writes to four separate
        // databases (Auth via the unit of work, plus Core Module, HRMM and Profile).
        // We keep one transaction per foreign DB so all writes commit together — or
        // roll back together — instead of auto-committing statement by statement.
        NpgsqlConnection? coreConn = null, hrmmConn = null, profConn = null;
        NpgsqlTransaction? coreTx = null, hrmmTx = null, profTx = null;

        try
        {
            // ✅ Validate connection strings
            ValidateConnectionStrings();
            details.Add("✅ Connection strings validated");

            // ✅ Start transactions (Auth + one per foreign database)
            await _uow.Begin(ct);

            coreConn = new NpgsqlConnection(_configuration.GetConnectionString("CorModuleDbCon"));
            hrmmConn = new NpgsqlConnection(_configuration.GetConnectionString("coreHRMMDbCon"));
            profConn = new NpgsqlConnection(_configuration.GetConnectionString("HRMProDbCon"));
            await coreConn.OpenAsync(ct);
            await hrmmConn.OpenAsync(ct);
            await profConn.OpenAsync(ct);
            coreTx = await coreConn.BeginTransactionAsync(ct);
            hrmmTx = await hrmmConn.BeginTransactionAsync(ct);
            profTx = await profConn.BeginTransactionAsync(ct);
            details.Add("✅ Transactions started");

            // ============ CLEAN EXISTING DATA ============
            details.Add("🗑️ Cleaning existing data...");
            await CleanExistingData(coreConn, coreTx, hrmmConn, hrmmTx, profConn, profTx, ct);
            details.Add("✅ Existing data cleaned");

            // ============ 1. SEED ROLES ============
            details.Add("📦 Seeding roles...");
            await SeedRoles(ct);
            details.Add("✅ Roles seeded");

            // ============ 2. CREATE COMPANY ============
            details.Add("🏢 Creating company...");
            var companyId = await CreateCompany(request.Company, coreConn, coreTx, ct);
            result.CompanyId = companyId;
            details.Add($"✅ Company created: {companyId}");

            // ============ 3. CREATE BRANCH ============
            details.Add("🏪 Creating branch...");
            var branchId = await CreateBranch(request.Branch, companyId, coreConn, coreTx, ct);
            result.BranchId = branchId;
            details.Add($"✅ Branch created: {branchId}");

            // ============ 4. CREATE DEPARTMENT ============
            details.Add("📁 Creating department...");
            var departmentId = await CreateDepartment(request.Department, branchId, coreConn, coreTx, ct);
            result.DepartmentId = departmentId;
            details.Add($"✅ Department created: {departmentId}");

            // ============ 5. CREATE POSITION ============
            details.Add("💼 Creating position...");
            var positionId = await CreatePosition(request.Position, departmentId, hrmmConn, hrmmTx, ct);
            result.PositionId = positionId;
            details.Add($"✅ Position created: {positionId}");

            // ============ 6. CREATE JOB GRADE ============
            details.Add("📊 Creating job grade...");
            var jobGradeId = await CreateJobGrade(request.Position.JobGradeName, hrmmConn, hrmmTx, ct);
            result.JobGradeId = jobGradeId;
            details.Add($"✅ Job grade created: {jobGradeId}");

            // ============ 7. CREATE PERSON ============
            details.Add("👤 Creating person...");
            var personId = await CreatePerson(request.AdminUser, profConn, profTx, ct);
            details.Add($"✅ Person created: {personId}");

            // ============ 8. CREATE EMPLOYEE ============
            details.Add("👔 Creating employee...");
            var employeeId = await CreateEmployee(
                request.AdminUser,
                personId,
                positionId,
                departmentId,
                jobGradeId,
                profConn,
                profTx,
                ct);
            result.EmployeeId = employeeId;
            details.Add($"✅ Employee created: {employeeId}");

            // ============ 9. CREATE ADMIN USER ============
            details.Add("🔐 Creating admin user...");
            var userId = await CreateAdminUser(
                request.AdminUser,
                employeeId,
                branchId,
                departmentId,
                positionId,
                ct);
            result.AdminUserId = userId;
            details.Add($"✅ Admin user created: {userId}");

            // ============ 10. SEED PERMISSIONS ============
            details.Add("🔑 Seeding permissions...");
            await SeedPermissions(ct);
            details.Add("✅ Permissions seeded");

            // ============ 11. ASSIGN ALL PERMISSIONS TO ADMIN ============
            details.Add("📋 Assigning permissions to admin...");
            await AssignAllPermissionsToAdmin(userId, ct);
            details.Add("✅ Permissions assigned to admin");

            // ✅ Commit all changes — foreign databases first, then the Auth unit of
            // work last (the admin user only persists once the Auth transaction commits).
            await coreTx.CommitAsync(ct);
            await hrmmTx.CommitAsync(ct);
            await profTx.CommitAsync(ct);
            await _uow.Commit(ct);
            details.Add("✅ Transactions committed");

            result.Success = true;
            result.Message = "System setup completed successfully!";
            result.Details = details;
            return result;
        }
        catch (Exception ex)
        {
            // ✅ Roll back everything on any error
            try { await _uow.Rollback(ct); } catch { }
            try { if (coreTx != null) await coreTx.RollbackAsync(ct); } catch { }
            try { if (hrmmTx != null) await hrmmTx.RollbackAsync(ct); } catch { }
            try { if (profTx != null) await profTx.RollbackAsync(ct); } catch { }

            details.Add($"❌ ERROR: {ex.Message}");
            if (ex.InnerException != null)
            {
                details.Add($"Inner Error: {ex.InnerException.Message}");
            }

            result.Success = false;
            result.Message = $"Setup failed: {ex.Message}";
            result.Details = details;

            throw new DomainException($"Setup failed: {ex.Message}");
        }
        finally
        {
            if (coreTx != null) await coreTx.DisposeAsync();
            if (hrmmTx != null) await hrmmTx.DisposeAsync();
            if (profTx != null) await profTx.DisposeAsync();
            if (coreConn != null) await coreConn.DisposeAsync();
            if (hrmmConn != null) await hrmmConn.DisposeAsync();
            if (profConn != null) await profConn.DisposeAsync();
        }
    }

    // ============ VALIDATE CONNECTION STRINGS ============
    private void ValidateConnectionStrings()
    {
        var requiredConnections = new[]
        {
            ("authMgrCon", "Auth Database"),
            ("HRMProDbCon", "HRM Profile Database"),
            ("coreHRMMDbCon", "HRMM Database"),
            ("CorModuleDbCon", "Core Module Database")
        };

        foreach (var (key, name) in requiredConnections)
        {
            var connString = _configuration.GetConnectionString(key);
            if (string.IsNullOrEmpty(connString))
            {
                throw new DomainException($"Connection string '{key}' ({name}) is not configured. Please check appsettings.json.");
            }

            try
            {
                using var connection = new NpgsqlConnection(connString);
                connection.Open();
                connection.Close();
                Console.WriteLine($"✅ Connection '{key}' validated successfully.");
            }
            catch (Exception ex)
            {
                throw new DomainException($"Cannot connect to '{key}' ({name}): {ex.Message}. Please check your connection string.");
            }
        }
    }

    // ============ CLEAN DATA ============
    private async Task CleanExistingData(
        NpgsqlConnection coreConn, NpgsqlTransaction coreTx,
        NpgsqlConnection hrmmConn, NpgsqlTransaction hrmmTx,
        NpgsqlConnection profConn, NpgsqlTransaction profTx,
        CancellationToken ct)
    {
        // Auth DB: wiped on its own connection. This clean is intentional on (re)setup;
        // the identity/permission rows are re-created below within the Auth unit of work.
        var authConnectionString = _configuration.GetConnectionString("authMgrCon");
        if (string.IsNullOrEmpty(authConnectionString))
            throw new DomainException("authMgrCon connection string is not configured");

        await using (var authConnection = new NpgsqlConnection(authConnectionString))
        {
            await authConnection.OpenAsync(ct);
            await authConnection.ExecuteAsync(@"
                DELETE FROM ""UserPerApi"";
                DELETE FROM ""UserPerMenu"";
                DELETE FROM ""UserPerModule"";
                DELETE FROM ""UserRole"";
                DELETE FROM ""AppUser"";
                DELETE FROM ""AppRole"";
                DELETE FROM ""RefreshToken"";
            ");
        }

        // Profile DB (inside the shared transaction)
        await profConn.ExecuteAsync(@"
            DELETE FROM ""Employee"";
            DELETE FROM ""Person"";
        ", transaction: profTx);

        // HRMM DB (inside the shared transaction)
        await hrmmConn.ExecuteAsync(@"DELETE FROM ""JgStep"";", transaction: hrmmTx);
        await hrmmConn.ExecuteAsync(@"DELETE FROM ""Position"";", transaction: hrmmTx);
        await hrmmConn.ExecuteAsync(@"
            DELETE FROM ""PositionBenefit"";
            DELETE FROM ""PositionEducation"";
            DELETE FROM ""PositionExp"";
            DELETE FROM ""PositionReq"";
        ", transaction: hrmmTx);
        await hrmmConn.ExecuteAsync(@"DELETE FROM ""JobGrade"";", transaction: hrmmTx);

        // Core Module DB (inside the shared transaction)
        await coreConn.ExecuteAsync(@"DELETE FROM ""Department"";", transaction: coreTx);
        await coreConn.ExecuteAsync(@"DELETE FROM ""Branch"";", transaction: coreTx);
        await coreConn.ExecuteAsync(@"DELETE FROM ""Company"";", transaction: coreTx);
    }

    // ============ SEED ROLES ============
    private async Task SeedRoles(CancellationToken ct)
    {
        var roles = new[]
        {
            new { Name = "admin", Desc = "System Administrator" },
            new { Name = "pre", Desc = "President" },
            new { Name = "ceo", Desc = "Executive/CEO" },
            new { Name = "vice", Desc = "Vice" },
            new { Name = "dir", Desc = "Director" },
            new { Name = "mgr", Desc = "Manager" },
            new { Name = "emp", Desc = "Employee" },
            new { Name = "inte", Desc = "Intern" }
        };

        foreach (var roleData in roles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleData.Name);
            if (!roleExists)
            {
                var role = new AppRole
                {
                    Name = roleData.Name,
                    NormalizedName = roleData.Name.ToUpper(),
                    Desc = roleData.Desc,
                };
                await _roleManager.CreateAsync(role);
            }
        }
    }

    // ============ CREATE COMPANY ============
    private async Task<Guid> CreateCompany(SetupCompanyDto dto, NpgsqlConnection connection, NpgsqlTransaction tx, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO ""Company"" (""Id"", ""Name"", ""NameAm"", ""TaxId"", ""Phone"", ""Email"", ""Address"", ""DateAdd"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @TaxId, @Phone, @Email, @Address, NOW(), false)";

        var id = Guid.CreateVersion7();
        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            dto.Name,
            dto.NameAm,
            dto.TaxId,
            dto.Phone,
            dto.Email,
            dto.Address
        }, transaction: tx);

        return id;
    }

    // ============ CREATE BRANCH ============
    private async Task<Guid> CreateBranch(SetupBranchDto dto, Guid companyId, NpgsqlConnection connection, NpgsqlTransaction tx, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO ""Branch"" (""Id"", ""Name"", ""NameAm"", ""Code"", ""Location"", ""OpenDate"", ""BranchType"", ""BranchStat"", ""CompId"", ""DateAdd"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @Code, @Location, NOW(), @BranchType, 'Active', @CompId, NOW(), false)";

        var id = Guid.CreateVersion7();
        var code = $"BR-{new Random().Next(1, 9999):D4}";

        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            dto.Name,
            dto.NameAm,
            Code = code,
            dto.Location,
            dto.BranchType,
            CompId = companyId
        }, transaction: tx);

        return id;
    }

    // ============ CREATE DEPARTMENT ============
    private async Task<Guid> CreateDepartment(SetupDepartmentDto dto, Guid branchId, NpgsqlConnection connection, NpgsqlTransaction tx, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO ""Department"" (""Id"", ""Name"", ""NameAm"", ""DeptStat"", ""BranchId"", ""DateAdd"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, 'Active', @BranchId, NOW(), false)";

        var id = Guid.CreateVersion7();
        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            dto.Name,
            dto.NameAm,
            BranchId = branchId
        }, transaction: tx);

        return id;
    }

    // ============ CREATE POSITION ============
    private async Task<Guid> CreatePosition(SetupPositionDto dto, Guid departmentId, NpgsqlConnection connection, NpgsqlTransaction tx, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO ""Position"" (""Id"", ""Name"", ""NameAm"", ""NoOfPosition"", ""IsVacant"", ""DepartmentId"", ""DateAdd"", ""IsDeleted"")
            VALUES (@Id, @Name, @NameAm, @NoOfPosition, 'No', @DepartmentId, NOW(), false)";

        var id = Guid.CreateVersion7();
        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            dto.Name,
            dto.NameAm,
            dto.NoOfPosition,
            DepartmentId = departmentId
        }, transaction: tx);

        return id;
    }

    // ============ CREATE JOB GRADE ============
    private async Task<Guid> CreateJobGrade(string jobGradeName, NpgsqlConnection connection, NpgsqlTransaction tx, CancellationToken ct)
    {
        const string checkSql = @"SELECT ""Id"" FROM ""JobGrade"" WHERE ""Name"" = @Name AND ""IsDeleted"" = false";
        var existingId = await connection.QueryFirstOrDefaultAsync<Guid?>(checkSql, new { Name = jobGradeName }, transaction: tx);

        if (existingId.HasValue)
            return existingId.Value;

        const string sql = @"
            INSERT INTO ""JobGrade"" (""Id"", ""Name"", ""StartSalary"", ""MaxSalary"", ""DateAdd"", ""IsDeleted"")
            VALUES (@Id, @Name, 0, 0, NOW(), false)
            RETURNING ""Id""";

        var id = Guid.CreateVersion7();
        var result = await connection.ExecuteScalarAsync<Guid>(sql, new
        {
            Id = id,
            Name = jobGradeName
        }, transaction: tx);

        return result;
    }

    // ============ CREATE PERSON ============
    private async Task<Guid> CreatePerson(SetupUserDto dto, NpgsqlConnection connection, NpgsqlTransaction tx, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO ""Person"" (""Id"", ""FirstName"", ""FirstNameAm"", ""MiddleName"", ""MiddleNameAm"", ""LastName"", ""LastNameAm"", ""Gender"", ""Nationality"", ""DateAdd"", ""IsDeleted"")
            VALUES (@Id, @FirstName, @FirstNameAm, @MiddleName, @MiddleNameAm, @LastName, @LastNameAm, @Gender, @Nationality, NOW(), false)
            RETURNING ""Id""";

        var id = Guid.CreateVersion7();

        var firstName = string.IsNullOrWhiteSpace(dto.FirstName) ? "Admin" : dto.FirstName;
        var firstNameAm = string.IsNullOrWhiteSpace(dto.FirstNameAm) ? firstName : dto.FirstNameAm;
        var middleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? "" : dto.MiddleName;
        var middleNameAm = string.IsNullOrWhiteSpace(dto.MiddleNameAm) ? "" : dto.MiddleNameAm;
        var lastName = string.IsNullOrWhiteSpace(dto.LastName) ? "User" : dto.LastName;
        var lastNameAm = string.IsNullOrWhiteSpace(dto.LastNameAm) ? lastName : dto.LastNameAm;
        var gender = string.IsNullOrWhiteSpace(dto.Gender) ? "Male" : dto.Gender;
        var nationality = string.IsNullOrWhiteSpace(dto.Nationality) ? "Eth" : dto.Nationality;

        var result = await connection.ExecuteScalarAsync<Guid>(sql, new
        {
            Id = id,
            FirstName = firstName,
            FirstNameAm = firstNameAm,
            MiddleName = middleName,
            MiddleNameAm = middleNameAm,
            LastName = lastName,
            LastNameAm = lastNameAm,
            Gender = gender,
            Nationality = nationality
        }, transaction: tx);

        return result;
    }

    // ============ CREATE EMPLOYEE ============
    private async Task<Guid> CreateEmployee(
        SetupUserDto dto,
        Guid personId,
        Guid positionId,
        Guid departmentId,
        Guid jobGradeId,
        NpgsqlConnection connection,
        NpgsqlTransaction tx,
        CancellationToken ct)
    {
        var random = new Random();
        var code = $"EMP-{random.Next(1, 999):D3}";

        const string sql = @"
            INSERT INTO ""Employee"" (
                ""Id"", ""Code"", ""EmploymentType"", ""EmploymentNature"", ""WorkArrangement"",
                ""EmpState"", ""EmploymentDate"", ""PersonId"", ""JobGradeId"", ""PositionId"",
                ""DepartmentId"", ""DateAdd"", ""IsDeleted""
            )
            VALUES (
                @Id, @Code, 'Permanent', 'FullTime', 'OnSite',
                'Active', NOW(), @PersonId, @JobGradeId, @PositionId,
                @DepartmentId, NOW(), false
            )
            RETURNING ""Id""";

        var id = Guid.CreateVersion7();
        var result = await connection.ExecuteScalarAsync<Guid>(sql, new
        {
            Id = id,
            Code = code,
            PersonId = personId,
            JobGradeId = jobGradeId,
            PositionId = positionId,
            DepartmentId = departmentId
        }, transaction: tx);

        return result;
    }

    // ============ CREATE ADMIN USER ============
    private async Task<Guid> CreateAdminUser(
        SetupUserDto dto,
        Guid employeeId,
        Guid? branchId,
        Guid? departmentId,
        Guid? positionId,
        CancellationToken ct)
    {
        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            EmployeeId = employeeId,
            BranchId = branchId,
            DepartmentId = departmentId,
            PositionId = positionId
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new DomainException($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        await _userManager.AddToRoleAsync(user, "admin");

        return Guid.Parse(user.Id);
    }

    // ============ SEED PERMISSIONS (FIXED) ============
    private async Task SeedPermissions(CancellationToken ct)
    {
        try
        {
            // 1. Seed modules first
            var modules = SeedPerList.GetPerModule();
            foreach (var module in modules)
            {
                await _uow.Add(module, ct);
            }
            await _uow.SaveChangesAsync(ct);

            // 2. Get module dictionary
            var moduleDict = await _uow.Set<PerModule>().ToDictionaryAsync(m => m.Key, m => m.Id, ct);

            // 3. Get all menu DTOs
            var menuDtos = SeedPerList.GetPerMenu().ToList();

            // 4. Process in order: parents first, then children
            var menuKeyToId = new Dictionary<string, Guid>();

            // First pass: Add all parent menus (IsChild = false)
            var parentDtos = menuDtos.Where(m => !m.IsChild).OrderBy(m => m.Order);
            foreach (var menuDto in parentDtos)
            {
                if (!moduleDict.TryGetValue(menuDto.ModKey, out var moduleId))
                {
                    Console.WriteLine($"⚠️ Module '{menuDto.ModKey}' not found for parent menu '{menuDto.Key}'. Skipping...");
                    continue;
                }

                var menu = new PerMenu
                {
                    Id = Guid.CreateVersion7(),
                    Key = menuDto.Key,
                    Label = menuDto.Label,
                    Path = menuDto.Path,
                    Icon = menuDto.Icon,
                    IsChild = false,
                    Order = menuDto.Order,
                    PerModuleId = moduleId,
                    ParentId = null,
                    DateAdd = DateTime.UtcNow
                };
                await _uow.Add(menu, ct);
                menuKeyToId[menuDto.Key] = menu.Id;
            }
            await _uow.SaveChangesAsync(ct);

            // Refresh the menu dictionary with all menus (including parents)
            var allMenuDict = await _uow.Set<PerMenu>().ToDictionaryAsync(m => m.Key, m => m.Id, ct);

            // Second pass: Add all child menus (IsChild = true)
            var childDtos = menuDtos.Where(m => m.IsChild).OrderBy(m => m.Order);
            foreach (var menuDto in childDtos)
            {
                if (!moduleDict.TryGetValue(menuDto.ModKey, out var moduleId))
                {
                    Console.WriteLine($"⚠️ Module '{menuDto.ModKey}' not found for child menu '{menuDto.Key}'. Skipping...");
                    continue;
                }

                // Find parent ID
                Guid? parentId = null;
                if (!string.IsNullOrEmpty(menuDto.ParKey))
                {
                    if (menuKeyToId.TryGetValue(menuDto.ParKey, out var parentGuid))
                    {
                        parentId = parentGuid;
                    }
                    else if (allMenuDict.TryGetValue(menuDto.ParKey, out var dbParentId))
                    {
                        parentId = dbParentId;
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Parent menu '{menuDto.ParKey}' not found for child menu '{menuDto.Key}'. Skipping...");
                        continue;
                    }
                }

                var menu = new PerMenu
                {
                    Id = Guid.CreateVersion7(),
                    Key = menuDto.Key,
                    Label = menuDto.Label,
                    Path = menuDto.Path,
                    Icon = menuDto.Icon,
                    IsChild = true,
                    Order = menuDto.Order,
                    PerModuleId = moduleId,
                    ParentId = parentId,
                    DateAdd = DateTime.UtcNow
                };
                await _uow.Add(menu, ct);
            }
            await _uow.SaveChangesAsync(ct);

            // 5. Seed permissions (API actions)
            var permissions = SeedPerList.GetPerAccess();
            var menuDict = await _uow.Set<PerMenu>().ToDictionaryAsync(m => m.Key, m => m.Id, ct);

            var permissionCount = 0;
            foreach (var permDto in permissions)
            {
                if (!menuDict.TryGetValue(permDto.MenuKey, out var menuId))
                {
                    Console.WriteLine($"⚠️ Menu '{permDto.MenuKey}' not found for permission '{permDto.Key}'. Skipping...");
                    continue;
                }

                var perm = new PerApi
                {
                    Id = Guid.CreateVersion7(),
                    Key = permDto.Key,
                    Desc = permDto.Desc,
                    PerMenuId = menuId,
                    DateAdd = DateTime.UtcNow
                };
                await _uow.Add(perm, ct);
                permissionCount++;
            }
            await _uow.SaveChangesAsync(ct);

            Console.WriteLine($"✅ Seeded {modules.Count()} modules, {menuDtos.Count} menus, and {permissionCount} permissions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in SeedPermissions: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    // ============ ASSIGN ALL PERMISSIONS TO ADMIN ============
    private async Task AssignAllPermissionsToAdmin(Guid userId, CancellationToken ct)
    {
        var adminUserId = userId.ToString();

        var modules = await _uow.Set<PerModule>().ToListAsync(ct);
        foreach (var module in modules)
        {
            var userModule = new UserPerModule
            {
                Id = Guid.CreateVersion7(),
                UserId = adminUserId,
                PerModuleId = module.Id,
                DateAdd = DateTime.UtcNow
            };
            await _uow.Add(userModule, ct);
        }

        var menus = await _uow.Set<PerMenu>().ToListAsync(ct);
        foreach (var menu in menus)
        {
            var userMenu = new UserPerMenu
            {
                Id = Guid.CreateVersion7(),
                UserId = adminUserId,
                PerMenuId = menu.Id,
                DateAdd = DateTime.UtcNow
            };
            await _uow.Add(userMenu, ct);
        }

        var apis = await _uow.Set<PerApi>().ToListAsync(ct);
        foreach (var api in apis)
        {
            var userApi = new UserPerApi
            {
                Id = Guid.CreateVersion7(),
                UserId = adminUserId,
                PerApiId = api.Id,
                DateAdd = DateTime.UtcNow
            };
            await _uow.Add(userApi, ct);
        }

        await _uow.SaveChangesAsync(ct);
        Console.WriteLine($"✅ All permissions assigned to admin user: {userId}");
    }
}