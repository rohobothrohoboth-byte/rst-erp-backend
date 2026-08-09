// DbPerformanceTestSeeder.cs
using Cor.Finance.Models.Entities;
using Cor.Finance.Models.Entities.Local;
using Cor.Finance.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Cor.Finance.Models.Enums;

namespace Cor.Finance.Seeding;

public class DbPerformanceTestSeeder
{
    private readonly FinanceDbContext _context;
    private readonly ILogger<DbPerformanceTestSeeder> _logger;
    private readonly Random _random = new();
    private readonly string[] _firstNames = { "John", "Jane", "Michael", "Sarah", "David", "Emma", "James", "Lisa", "Robert", "Maria" };
    private readonly string[] _lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };
    private readonly string[] _companyNames = { "Acme Corp", "Globex Inc", "Initech", "Hooli", "Stark Industries", "Wayne Enterprises", "Cyberdyne", "Umbrella Corp", "Weyland-Yutani", "Tyrell Corp" };
    private readonly string[] _branchNames = { "Main Office", "North Branch", "South Branch", "East Branch", "West Branch", "Downtown", "Uptown", "Metro", "Plaza", "Tower" };
    private readonly string[] _departmentNames = { "Sales", "Marketing", "Engineering", "Finance", "HR", "Operations", "IT", "Legal", "R&D", "Customer Service" };
    private readonly string[] _descriptions = {
        "Monthly service fee", "Product purchase", "Consulting service", "Maintenance contract",
        "Software license", "Hardware upgrade", "Training session", "Support package",
        "Professional services", "Annual subscription", "One-time fee", "Installation service"
    };

    public DbPerformanceTestSeeder(FinanceDbContext context, ILogger<DbPerformanceTestSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }
public async Task<long> GetTableCountAsync(string tableName)
{
#pragma warning disable EF1002 // Table name is an identifier; cannot be parameterized via FormattableString
    return await _context.Set<object>().FromSqlRaw($"SELECT COUNT(*) FROM \"{tableName}\"").CountAsync();
#pragma warning restore EF1002
}
 private async Task ClearTestDataAsync()
    {
        _logger.LogInformation("🗑️ Clearing existing test data...");

        // ✅ Option 1: Using TRUNCATE (Fastest)
        try
        {
            // Disable foreign key checks temporarily (PostgreSQL doesn't support this directly)
            // So we use TRUNCATE with CASCADE

         var sql = @"
                TRUNCATE TABLE ""JournalLines"" CASCADE;
                TRUNCATE TABLE ""JournalEntries"" CASCADE;
                TRUNCATE TABLE ""BudgetLines"" CASCADE;
                TRUNCATE TABLE ""Budgets"" CASCADE;
                TRUNCATE TABLE ""Payments"" CASCADE;
                TRUNCATE TABLE ""InvoiceLines"" CASCADE;
                TRUNCATE TABLE ""Invoices"" CASCADE;
                TRUNCATE TABLE ""Expenses"" CASCADE;
                TRUNCATE TABLE ""ExpenseCategories"" CASCADE;
                TRUNCATE TABLE ""LocalEmployees"" CASCADE;
                TRUNCATE TABLE ""LocalPositions"" CASCADE;
                TRUNCATE TABLE ""LocalJobGrades"" CASCADE;
                TRUNCATE TABLE ""LocalDepartments"" CASCADE;
                TRUNCATE TABLE ""LocalBranches"" CASCADE;
                TRUNCATE TABLE ""LocalCompanies"" CASCADE;
                TRUNCATE TABLE ""AuditLogs"" CASCADE;
                TRUNCATE TABLE ""FinancialPeriods"" CASCADE;
                TRUNCATE TABLE ""ChartOfAccounts"" CASCADE;
                TRUNCATE TABLE ""Vendors"" CASCADE;
                TRUNCATE TABLE ""Customers"" CASCADE;
                TRUNCATE TABLE ""BankAccounts"" CASCADE;
                TRUNCATE TABLE ""Assets"" CASCADE;
                TRUNCATE TABLE ""TaxReturns"" CASCADE;
            ";

            await _context.Database.ExecuteSqlRawAsync(sql);
            _logger.LogInformation("✅ All test data cleared successfully using TRUNCATE");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "TRUNCATE failed, falling back to DELETE method...");

            // ✅ Option 2: Using DELETE (Fallback)
            // Delete in correct order (child tables first, then parent tables)
            _context.JournalLines.RemoveRange(_context.JournalLines);
            _context.JournalEntries.RemoveRange(_context.JournalEntries);
            _context.BudgetLines.RemoveRange(_context.BudgetLines);
            _context.Budgets.RemoveRange(_context.Budgets);
            _context.Payments.RemoveRange(_context.Payments);
            _context.InvoiceLines.RemoveRange(_context.InvoiceLines);
            _context.Invoices.RemoveRange(_context.Invoices);
            _context.Expenses.RemoveRange(_context.Expenses);
            _context.ExpenseCategories.RemoveRange(_context.ExpenseCategories);
            _context.LocalEmployees.RemoveRange(_context.LocalEmployees);
            _context.LocalDepartments.RemoveRange(_context.LocalDepartments);
            _context.LocalBranches.RemoveRange(_context.LocalBranches);
            _context.LocalCompanies.RemoveRange(_context.LocalCompanies);
            _context.AuditLogs.RemoveRange(_context.AuditLogs);
            _context.FinancialPeriods.RemoveRange(_context.FinancialPeriods);
            _context.ChartOfAccounts.RemoveRange(_context.ChartOfAccounts);
            _context.LocalJobGrades.RemoveRange(_context.LocalJobGrades);
            _context.LocalPositions.RemoveRange(_context.LocalPositions);
            _context.Vendors.RemoveRange(_context.Vendors);
            _context.Customers.RemoveRange(_context.Customers);
            _context.BankAccounts.RemoveRange(_context.BankAccounts);
            _context.Assets.RemoveRange(_context.Assets);
            _context.TaxReturns.RemoveRange(_context.TaxReturns);

            await _context.SaveChangesAsync();
            _logger.LogInformation("✅ All test data cleared successfully using DELETE");
        }
    }
private async Task<List<LocalJobGrade>> SeedJobGradesAsync(int count)
{
    _logger.LogInformation($"📊 Seeding {count} job grades...");
    var jobGrades = new List<LocalJobGrade>();

    var jobGradeNames = new[] {
        "Entry Level", "Junior", "Mid-Level", "Senior", "Lead",
        "Manager", "Senior Manager", "Director", "Senior Director",
        "VP", "SVP", "EVP", "C-Level", "Executive", "Fellow"
    };

    for (int i = 0; i < count; i++)
    {
        var name = jobGradeNames[_random.Next(jobGradeNames.Length)] + $" {i + 1}";
        jobGrades.Add(new LocalJobGrade
        {
            Id = Guid.NewGuid(),
            Name = name,
            StartSalary = _random.Next(30000, 80000),
            MaxSalary = _random.Next(80000, 200000),
            DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
            IsDeleted = false,
            SyncedAt = DateTime.UtcNow
        });
    }

    await _context.LocalJobGrades.AddRangeAsync(jobGrades);
    await _context.SaveChangesAsync();
    _logger.LogInformation($"✅ Seeded {jobGrades.Count} job grades");
    return jobGrades;
}

private async Task<List<LocalPosition>> SeedPositionsAsync(int count, List<LocalDepartment> departments, List<LocalJobGrade> jobGrades)
{
    _logger.LogInformation($"💼 Seeding {count} positions...");
    var positions = new List<LocalPosition>();

    var positionNames = new[] {
        "Software Engineer", "Accountant", "HR Manager", "Sales Representative",
        "Marketing Specialist", "Financial Analyst", "Operations Manager",
        "Product Manager", "Project Manager", "Business Analyst",
        "Data Scientist", "UX Designer", "DevOps Engineer", "QA Engineer",
        "Technical Lead", "Architect", "Consultant", "Administrator"
    };
    var isVacant = new[] { "Yes", "No" };

    for (int i = 0; i < count; i++)
    {
        var name = positionNames[_random.Next(positionNames.Length)] + $" {i + 1}";
        positions.Add(new LocalPosition
        {
            Id = Guid.NewGuid(),
            Name = name,
            NameAm = name + " (Amharic)",
            NoOfPosition = _random.Next(1, 10),
            IsVacant = isVacant[_random.Next(isVacant.Length)],
            DepartmentId = departments[_random.Next(departments.Count)].Id,
            JobGradeId = jobGrades[_random.Next(jobGrades.Count)].Id,
            DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
            IsDeleted = false,
            SyncedAt = DateTime.UtcNow
        });
    }

    await _context.LocalPositions.AddRangeAsync(positions);
    await _context.SaveChangesAsync();
    _logger.LogInformation($"✅ Seeded {positions.Count} positions");
    return positions;
}
  public async Task SeedDataAsync(int count = 1000)
  {
      _logger.LogInformation("========================================");
      _logger.LogInformation("🚀 STARTING DATABASE PERFORMANCE TEST");
      _logger.LogInformation($"📊 Target: {count} records per table");
      _logger.LogInformation("========================================");

      // Clear existing test data
      await ClearTestDataAsync();

      // Create base entities first
      var companies = await SeedCompaniesAsync(count);
      var branches = await SeedBranchesAsync(count, companies);
      var departments = await SeedDepartmentsAsync(count, branches);
      var jobGrades = await SeedJobGradesAsync(count);
      var positions = await SeedPositionsAsync(count, departments, jobGrades);
      var employees = await SeedEmployeesAsync(count, departments, positions, jobGrades);

      // ✅ Create accounts before using them in budgets
      var accounts = await SeedChartOfAccountsAsync(count);
      var periods = await SeedFinancialPeriodsAsync(count);

      // Seed transactional data
      await SeedInvoicesAsync(count, companies, branches, departments, employees, periods);
      await SeedPaymentsAsync(count, branches, departments, employees, periods);
      await SeedExpensesAsync(count, branches, departments, employees, periods);
      await SeedJournalEntriesAsync(count, accounts, periods, employees);

      // ✅ Pass accounts to SeedBudgetsAsync
      await SeedBudgetsAsync(count, departments, periods, accounts);

      await _context.SaveChangesAsync();

      _logger.LogInformation("========================================");
      _logger.LogInformation("✅ SEEDING COMPLETED SUCCESSFULLY");
      _logger.LogInformation($"📊 Total records inserted: ~{count * 12}");
      _logger.LogInformation("========================================");
  }


    private async Task<List<LocalCompany>> SeedCompaniesAsync(int count)
    {
        _logger.LogInformation($"🏢 Seeding {count} companies...");
        var companies = new List<LocalCompany>();

        for (int i = 0; i < count; i++)
        {
            var name = _companyNames[_random.Next(_companyNames.Length)] + $" {i + 1}";
            companies.Add(new LocalCompany
            {
                Id = Guid.NewGuid(),
                Name = name,
                NameAm = name + " (Amharic)",
                TaxId = $"TAX-{_random.Next(100000, 999999)}",
                Phone = $"+1-{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
                Email = $"info@{name.Replace(" ", "").ToLower()}.com",
                Address = $"{_random.Next(1, 999)} Main St, City {_random.Next(1, 100)}",
                DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                IsDeleted = false
            });
        }

        await _context.LocalCompanies.AddRangeAsync(companies);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Seeded {companies.Count} companies");
        return companies;
    }

    private async Task<List<LocalBranch>> SeedBranchesAsync(int count, List<LocalCompany> companies)
    {
        _logger.LogInformation($"🏪 Seeding {count} branches...");
        var branches = new List<LocalBranch>();

        for (int i = 0; i < count; i++)
        {
            var name = _branchNames[_random.Next(_branchNames.Length)] + $" {i + 1}";
            branches.Add(new LocalBranch
            {
                Id = Guid.NewGuid(),
                Name = name,
                NameAm = name + " (Amharic)",
                Code = $"BR-{_random.Next(1000, 9999)}",
                Location = $"Location {_random.Next(1, 100)}",
                CompId = companies[_random.Next(companies.Count)].Id,
                DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                IsDeleted = false
            });
        }

        await _context.LocalBranches.AddRangeAsync(branches);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Seeded {branches.Count} branches");
        return branches;
    }

    private async Task<List<LocalDepartment>> SeedDepartmentsAsync(int count, List<LocalBranch> branches)
    {
        _logger.LogInformation($"🏢 Seeding {count} departments...");
        var departments = new List<LocalDepartment>();

        for (int i = 0; i < count; i++)
        {
            var name = _departmentNames[_random.Next(_departmentNames.Length)] + $" {i + 1}";
            departments.Add(new LocalDepartment
            {
                Id = Guid.NewGuid(),
                Name = name,
                NameAm = name + " (Amharic)",
                BranchId = branches[_random.Next(branches.Count)].Id,
                DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                IsDeleted = false
            });
        }

        await _context.LocalDepartments.AddRangeAsync(departments);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Seeded {departments.Count} departments");
        return departments;
    }

   private async Task<List<LocalEmployee>> SeedEmployeesAsync(int count, List<LocalDepartment> departments, List<LocalPosition> positions, List<LocalJobGrade> jobGrades)
   {
       _logger.LogInformation($"👤 Seeding {count} employees...");
       var employees = new List<LocalEmployee>();

       var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emma", "James", "Lisa", "Robert", "Maria", "William", "Patricia", "Richard", "Jennifer", "Charles" };
       var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez", "Hernandez", "Lopez", "Wilson", "Anderson", "Thomas" };
       var middleNames = new[] { "A.", "J.", "K.", "M.", "R.", "L.", "S.", "D.", "E.", "W." };
       var employmentTypes = new[] { "Full-Time", "Part-Time", "Contract", "Intern" };
       var employmentNatures = new[] { "Permanent", "Temporary", "Probation", "Contractual" };
       var workArrangements = new[] { "On-site", "Remote", "Hybrid", "Field" };
       var empStates = new[] { "Active", "Inactive", "Terminated", "On Leave" };
       var genders = new[] { "Male", "Female", "Other" };
       var nationalities = new[] { "American", "Canadian", "British", "Australian", "Indian", "Ethiopian", "German", "French", "Japanese", "Brazilian" };

       for (int i = 0; i < count; i++)
       {
           var firstName = firstNames[_random.Next(firstNames.Length)];
           var lastName = lastNames[_random.Next(lastNames.Length)];
           var middleName = middleNames[_random.Next(middleNames.Length)];

           employees.Add(new LocalEmployee
           {
               Id = Guid.NewGuid(),
               // Required string properties
               FirstName = firstName,
               FirstNameAm = firstName + " (Amharic)",
               MiddleName = middleName,
               MiddleNameAm = middleName + " (Amharic)",
               LastName = lastName,
               LastNameAm = lastName + " (Amharic)",
               Code = $"EMP-{_random.Next(10000, 99999)}",
               EmploymentType = employmentTypes[_random.Next(employmentTypes.Length)],
               EmploymentNature = employmentNatures[_random.Next(employmentNatures.Length)],
               WorkArrangement = workArrangements[_random.Next(workArrangements.Length)],
               EmpState = empStates[_random.Next(empStates.Length)],
               Gender = genders[_random.Next(genders.Length)],
               Nationality = nationalities[_random.Next(nationalities.Length)],

               // Required Guid properties
               PersonId = Guid.NewGuid(),
               JobGradeId = jobGrades[_random.Next(jobGrades.Count)].Id,
               PositionId = positions[_random.Next(positions.Count)].Id,
               DepartmentId = departments[_random.Next(departments.Count)].Id,

               // ✅ FIX: Set BranchId to null (it's nullable)
               BranchId = null,  // ← This is the fix

               // AppUserId is nullable
               AppUserId = _random.Next(0, 2) == 0 ? Guid.NewGuid() : (Guid?)null,

               // Optional properties
               Email = $"{firstName.ToLower()}.{lastName.ToLower()}{_random.Next(1, 100)}@company.com",
               Phone = $"+1-{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",

               // Dates
               EmploymentDate = DateTime.UtcNow.AddDays(-_random.Next(1, 1095)),
               DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
               IsActive = _random.Next(0, 2) == 0,
               IsDeleted = false,
               SyncedAt = DateTime.UtcNow
           });
       }

       // Batch insert for better performance
       const int batchSize = 100;
       for (int i = 0; i < employees.Count; i += batchSize)
       {
           var batch = employees.Skip(i).Take(batchSize);
           await _context.LocalEmployees.AddRangeAsync(batch);
           await _context.SaveChangesAsync();
           _logger.LogDebug($"   - Seeded batch {i / batchSize + 1} ({batch.Count()} employees)");
       }

       _logger.LogInformation($"✅ Seeded {employees.Count} employees");
       return employees;
   }

    private async Task<List<ChartOfAccounts>> SeedChartOfAccountsAsync(int count)
    {
        _logger.LogInformation($"📊 Seeding {count} chart of accounts...");
        var accounts = new List<ChartOfAccounts>();

        var accountTypes = new[] { "Asset", "Liability", "Equity", "Revenue", "Expense" };
        var accountSubTypes = new[] { "Current", "Non-Current", "Operating", "Non-Operating", "Direct", "Indirect" };
        var accountNames = new[] {
            "Cash", "Accounts Receivable", "Inventory", "Fixed Assets", "Accounts Payable",
            "Accrued Expenses", "Share Capital", "Retained Earnings", "Sales Revenue",
            "Service Revenue", "Cost of Goods Sold", "Rent Expense", "Salaries Expense",
            "Utilities Expense", "Depreciation", "Interest Expense", "Tax Expense",
            "Prepaid Expenses", "Equipment", "Vehicles", "Buildings", "Land"
        };

        // ✅ Use a HashSet to track used codes
        var usedCodes = new HashSet<string>();

        for (int i = 0; i < count; i++)
        {
            string code;
            // ✅ Generate unique code
            do
            {
                code = $"ACC-{_random.Next(10000, 99999)}";
            } while (usedCodes.Contains(code));
            usedCodes.Add(code);

            var nameIndex = _random.Next(accountNames.Length);
            var name = accountNames[nameIndex];

            // ✅ Add number suffix to make names unique
            var uniqueName = $"{name} {i + 1}";

            accounts.Add(new ChartOfAccounts
            {
                Id = Guid.NewGuid(),
                Code = code,  // ✅ Now unique
                Name = uniqueName,
                NameAm = uniqueName + " (Amharic)",
                Description = _descriptions[_random.Next(_descriptions.Length)],
                AccountType = accountTypes[_random.Next(accountTypes.Length)],
                AccountSubType = accountSubTypes[_random.Next(accountSubTypes.Length)],
                IsActive = true,
                Level = _random.Next(1, 5),
                DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                IsDeleted = false
            });
        }

        // Batch insert for better performance
        const int batchSize = 100;
        for (int i = 0; i < accounts.Count; i += batchSize)
        {
            var batch = accounts.Skip(i).Take(batchSize);
            await _context.ChartOfAccounts.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"   - Seeded batch {i / batchSize + 1} ({batch.Count()} accounts)");
        }

        _logger.LogInformation($"✅ Seeded {accounts.Count} chart of accounts");
        return accounts;
    }

   private async Task<List<FinancialPeriod>> SeedFinancialPeriodsAsync(int count)
   {
       _logger.LogInformation($"📅 Seeding {count} financial periods...");
       var periods = new List<FinancialPeriod>();

       var startDate = new DateTime(2020, 1, 1);
       var usedNames = new HashSet<string>();

       for (int i = 0; i < count; i++)
       {
           var start = startDate.AddMonths(i);
           var name = $"Period {start:yyyy-MM}";

           // ✅ Ensure unique name
           if (usedNames.Contains(name))
           {
               name = $"Period {start:yyyy-MM}-{i}";
           }
           usedNames.Add(name);

           periods.Add(new FinancialPeriod
           {
               Id = Guid.NewGuid(),
               Name = name,
               StartDate = start,
               EndDate = start.AddMonths(1).AddDays(-1),
               IsClosed = false,
              Status = PeriodStatus.DRAFT,
               DateAdd = DateTime.UtcNow,
               IsDeleted = false
           });
       }

       await _context.FinancialPeriods.AddRangeAsync(periods);
       await _context.SaveChangesAsync();
       _logger.LogInformation($"✅ Seeded {periods.Count} financial periods");
       return periods;
   }
private async Task<List<Vendor>> SeedVendorsAsync(int count)
{
    _logger.LogInformation($"🏢 Seeding {count} vendors...");
    var vendors = new List<Vendor>();

    var vendorNames = new[] {
        "ABC Supplies", "XYZ Distributors", "Global Trading Co", "Prime Logistics",
        "Elite Solutions", "Premium Services", "Reliable Partners", "Trusted Suppliers"
    };

    for (int i = 0; i < count; i++)
    {
        var name = vendorNames[_random.Next(vendorNames.Length)] + $" {i + 1}";
        vendors.Add(new Vendor
        {
            Id = Guid.NewGuid(),
            Code = $"VEN-{_random.Next(10000, 99999)}",
            Name = name,
            NameAm = name + " (Amharic)",
            Email = $"info@{name.Replace(" ", "").ToLower()}.com",
            Phone = $"+1-{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
            Address = $"{_random.Next(1, 999)} Business St, City {_random.Next(1, 100)}",
            Status = "Active",
            VendorType = new[] { "Individual", "Company", "Partnership" }[_random.Next(0, 3)],
            IsActive = true,
            DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
            IsDeleted = false
        });
    }

    await _context.Vendors.AddRangeAsync(vendors);
    await _context.SaveChangesAsync();
    _logger.LogInformation($"✅ Seeded {vendors.Count} vendors");
    return vendors;
}

private async Task<List<Customer>> SeedCustomersAsync(int count)
{
    _logger.LogInformation($"👤 Seeding {count} customers...");
    var customers = new List<Customer>();

    var customerNames = new[] {
        "Tech Solutions Inc", "Cloud Services LLC", "Digital Systems", "Innovation Hub",
        "Smart Solutions", "Future Tech", "Vision Corp", "Pioneer Enterprises"
    };

    for (int i = 0; i < count; i++)
    {
        var name = customerNames[_random.Next(customerNames.Length)] + $" {i + 1}";
        customers.Add(new Customer
        {
            Id = Guid.NewGuid(),
            Code = $"CUS-{_random.Next(10000, 99999)}",
            Name = name,
            NameAm = name + " (Amharic)",
            Email = $"info@{name.Replace(" ", "").ToLower()}.com",
            Phone = $"+1-{_random.Next(100, 999)}-{_random.Next(100, 999)}-{_random.Next(1000, 9999)}",
            Address = $"{_random.Next(1, 999)} Customer St, City {_random.Next(1, 100)}",
            Status = "Active",
            CustomerType = new[] { "Individual", "Business", "Government" }[_random.Next(0, 3)],
            IsActive = true,
            DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
            IsDeleted = false
        });
    }

    await _context.Customers.AddRangeAsync(customers);
    await _context.SaveChangesAsync();
    _logger.LogInformation($"✅ Seeded {customers.Count} customers");
    return customers;
}
    private async Task SeedInvoicesAsync(int count, List<LocalCompany> companies, List<LocalBranch> branches,
        List<LocalDepartment> departments, List<LocalEmployee> employees, List<FinancialPeriod> periods)
    {
        _logger.LogInformation($"📄 Seeding {count} invoices...");
        var invoices = new List<Invoice>();

        // ✅ Track used invoice numbers
        var usedInvoiceNumbers = new HashSet<string>();

        // ✅ Create some vendors and customers (or use existing)
        var vendors = await SeedVendorsAsync(Math.Min(count / 10, 50));
        var customers = await SeedCustomersAsync(Math.Min(count / 10, 50));

        for (int i = 0; i < count; i++)
        {
            string invoiceNumber;
            do
            {
                invoiceNumber = $"INV-{_random.Next(100000, 999999)}";
            } while (usedInvoiceNumbers.Contains(invoiceNumber));
            usedInvoiceNumbers.Add(invoiceNumber);

            var total = (decimal)_random.Next(100, 10000);
            var tax = total * 0.15m;
            var isPurchase = _random.Next(0, 2) == 0;

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = invoiceNumber,
                InvoiceDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                DueDate = DateTime.UtcNow.AddDays(_random.Next(1, 30)),
                SubTotal = total - tax,
                TaxAmount = tax,
                TotalAmount = total,
                PaidAmount = _random.Next(0, 2) == 0 ? total : total * (decimal)_random.Next(0, 100) / 100,
                Status = "Paid",
                InvoiceType = isPurchase ? "Purchase" : "Sales",
                Notes = _descriptions[_random.Next(_descriptions.Length)],
                PeriodId = periods[_random.Next(periods.Count)].Id,
                BranchId = branches[_random.Next(branches.Count)].Id,
                DepartmentId = departments[_random.Next(departments.Count)].Id,
                EmployeeId = employees[_random.Next(employees.Count)].Id,
                DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                IsDeleted = false,

                // ✅ FIX: Set either VendorId OR CustomerId (not both, not neither)
                VendorId = isPurchase ? vendors[_random.Next(vendors.Count)].Id : (Guid?)null,
                CustomerId = !isPurchase ? customers[_random.Next(customers.Count)].Id : (Guid?)null
            };

            invoices.Add(invoice);
        }

        // Batch insert
        const int batchSize = 100;
        for (int i = 0; i < invoices.Count; i += batchSize)
        {
            var batch = invoices.Skip(i).Take(batchSize);
            await _context.Invoices.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"   - Seeded batch {i / batchSize + 1} ({batch.Count()} invoices)");
        }

        _logger.LogInformation($"✅ Seeded {invoices.Count} invoices");
    }

  private async Task SeedPaymentsAsync(int count, List<LocalBranch> branches, List<LocalDepartment> departments,
      List<LocalEmployee> employees, List<FinancialPeriod> periods)
  {
      _logger.LogInformation($"💰 Seeding {count} payments...");
      var payments = new List<Payment>();

      var usedPaymentNumbers = new HashSet<string>();

      // Get existing vendors and customers
      var vendors = await _context.Vendors.Take(50).ToListAsync();
      var customers = await _context.Customers.Take(50).ToListAsync();

      for (int i = 0; i < count; i++)
      {
          string paymentNumber;
          do
          {
              paymentNumber = $"PAY-{_random.Next(100000, 999999)}";
          } while (usedPaymentNumbers.Contains(paymentNumber));
          usedPaymentNumbers.Add(paymentNumber);

          var amount = (decimal)_random.Next(100, 5000);
          var isPurchase = _random.Next(0, 2) == 0;

          payments.Add(new Payment
          {
              Id = Guid.NewGuid(),
              PaymentNumber = paymentNumber,
              PaymentDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
              PaymentType = isPurchase ? "Purchase" : "Sales",
              PaymentMethod = new[] { "Bank Transfer", "Cash", "Check", "Credit Card" }[_random.Next(0, 4)],
              Amount = amount,
              Status = "Completed",
              Reference = $"REF-{_random.Next(100000, 999999)}",
              PeriodId = periods[_random.Next(periods.Count)].Id,
              BranchId = branches[_random.Next(branches.Count)].Id,
              EmployeeId = employees[_random.Next(employees.Count)].Id,
              DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
              IsDeleted = false,

              // ✅ Set either VendorId or CustomerId
              VendorId = isPurchase && vendors.Any() ? vendors[_random.Next(vendors.Count)].Id : (Guid?)null,
              CustomerId = !isPurchase && customers.Any() ? customers[_random.Next(customers.Count)].Id : (Guid?)null
          });
      }

      await _context.Payments.AddRangeAsync(payments);
      await _context.SaveChangesAsync();
      _logger.LogInformation($"✅ Seeded {payments.Count} payments");
  }

    private async Task SeedExpensesAsync(int count, List<LocalBranch> branches, List<LocalDepartment> departments,
        List<LocalEmployee> employees, List<FinancialPeriod> periods)
    {
        _logger.LogInformation($"💳 Seeding {count} expenses...");
        var expenses = new List<Expense>();

        // Create expense categories first
        var categories = new List<ExpenseCategory>();
        for (int i = 0; i < 5; i++)
        {
            categories.Add(new ExpenseCategory
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i + 1}",
                NameAm = $"Category {i + 1} (Amharic)",
                CategoryType = "Operating",
                IsActive = true,
                DateAdd = DateTime.UtcNow,
                IsDeleted = false
            });
        }
        await _context.ExpenseCategories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();

        for (int i = 0; i < count; i++)
        {
            expenses.Add(new Expense
            {
                Id = Guid.NewGuid(),
                Amount = (decimal)_random.Next(50, 2000),
                Description = _descriptions[_random.Next(_descriptions.Length)],
                ExpenseDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                PaymentMethod = new[] { "Bank Transfer", "Cash", "Check" }[_random.Next(0, 3)],
                Status = "Approved",
                PeriodId = periods[_random.Next(periods.Count)].Id,
                ExpenseCategoryId = categories[_random.Next(categories.Count)].Id,
                BranchId = branches[_random.Next(branches.Count)].Id,
                DepartmentId = departments[_random.Next(departments.Count)].Id,
                EmployeeId = employees[_random.Next(employees.Count)].Id,
                DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                IsDeleted = false
            });
        }

        await _context.Expenses.AddRangeAsync(expenses);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"✅ Seeded {expenses.Count} expenses");
    }

 private async Task SeedJournalEntriesAsync(int count, List<ChartOfAccounts> accounts,
     List<FinancialPeriod> periods, List<LocalEmployee> employees)
 {
     _logger.LogInformation($"📒 Seeding {count} journal entries...");
     var journalEntries = new List<JournalEntry>();
     var journalLines = new List<JournalLine>();

     // ✅ Define entry types
     var entryTypes = new[] { "Manual", "System", "Auto", "Reversal", "Adjustment" };
     var entryTypeDescriptions = new[] {
         "Manual Entry", "System Generated", "Auto Generated", "Reversal Entry", "Adjustment Entry"
     };

     for (int i = 0; i < count; i++)
     {
         var entryTypeIndex = _random.Next(entryTypes.Length);
         var entryType = entryTypes[entryTypeIndex];
         var entryTypeDesc = entryTypeDescriptions[entryTypeIndex];

         var entry = new JournalEntry
         {
             Id = Guid.NewGuid(),
             Reference = $"JE-{_random.Next(100000, 999999)}",
             EntryDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
             Description = _descriptions[_random.Next(_descriptions.Length)],
             EntryType = entryType,  // ✅ REQUIRED - NOT NULL

             IsPosted = true,
             PeriodId = periods[_random.Next(periods.Count)].Id,
             EmployeeId = employees[_random.Next(employees.Count)].Id,
             TotalDebit = 0,
             TotalCredit = 0,
             DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
             IsDeleted = false,
             IsApproved = _random.Next(0, 2) == 0,

             IsReversed = false
         };

         // Add 2-4 journal lines per entry
         int numLines = _random.Next(2, 5);
         decimal totalDebit = 0;
         decimal totalCredit = 0;

         for (int j = 0; j < numLines; j++)
         {
             var amount = (decimal)_random.Next(100, 5000);
             var isDebit = j == 0 || _random.Next(0, 2) == 0;

             var line = new JournalLine
             {
                 Id = Guid.NewGuid(),
                 JournalEntryId = entry.Id,
                 AccountId = accounts[_random.Next(accounts.Count)].Id,
                 Amount = amount,
                 Debit = isDebit ? amount : 0,
                 Credit = isDebit ? 0 : amount,
                 Description = _descriptions[_random.Next(_descriptions.Length)],
                 Direction = isDebit ? "Debit" : "Credit",
                 PeriodId = entry.PeriodId,
                 DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                 IsDeleted = false
             };

             if (isDebit)
                 totalDebit += amount;
             else
                 totalCredit += amount;

             journalLines.Add(line);
         }

         entry.TotalDebit = totalDebit;
         entry.TotalCredit = totalCredit;
         journalEntries.Add(entry);
     }

     // Batch insert JournalEntries
     const int batchSize = 50;
     for (int i = 0; i < journalEntries.Count; i += batchSize)
     {
         var batch = journalEntries.Skip(i).Take(batchSize);
         await _context.JournalEntries.AddRangeAsync(batch);
         await _context.SaveChangesAsync();
         _logger.LogDebug($"   - Seeded batch {i / batchSize + 1} ({batch.Count()} journal entries)");
     }

     // Batch insert JournalLines
     for (int i = 0; i < journalLines.Count; i += batchSize)
     {
         var batch = journalLines.Skip(i).Take(batchSize);
         await _context.JournalLines.AddRangeAsync(batch);
         await _context.SaveChangesAsync();
         _logger.LogDebug($"   - Seeded batch {i / batchSize + 1} ({batch.Count()} journal lines)");
     }

     _logger.LogInformation($"✅ Seeded {journalEntries.Count} journal entries with {journalLines.Count} lines");
 }

   private async Task SeedBudgetsAsync(int count, List<LocalDepartment> departments,
       List<FinancialPeriod> periods, List<ChartOfAccounts> accounts)  // ✅ Add accounts parameter
   {
       _logger.LogInformation($"📊 Seeding {count} budgets...");
       var budgets = new List<Budget>();
       var budgetLines = new List<BudgetLine>();

       for (int i = 0; i < count; i++)
       {
           var budget = new Budget
           {
               Id = Guid.NewGuid(),
               Name = $"Budget {i + 1}",
               Description = _descriptions[_random.Next(_descriptions.Length)],
               TotalAmount = (decimal)_random.Next(10000, 100000),
               StartDate = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
               EndDate = DateTime.UtcNow.AddDays(_random.Next(1, 365)),
               Status = "Active",
               DepartmentId = departments[_random.Next(departments.Count)].Id,
               PeriodId = periods[_random.Next(periods.Count)].Id,
               DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
               IsDeleted = false
           };

           budgets.Add(budget);

           // Add 3-6 budget lines per budget
           for (int j = 0; j < _random.Next(3, 7); j++)
           {
               budgetLines.Add(new BudgetLine
               {
                   Id = Guid.NewGuid(),
                   BudgetId = budget.Id,
                   AccountId = accounts[_random.Next(accounts.Count)].Id,  // ✅ Use valid account ID
                   AllocatedAmount = (decimal)_random.Next(1000, 10000),
                   SpentAmount = (decimal)_random.Next(0, 5000),
                   Description = _descriptions[_random.Next(_descriptions.Length)],
                   PeriodId = budget.PeriodId,
                   DateAdd = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                   IsDeleted = false
               });
           }
       }

       // Batch insert Budgets
       const int batchSize = 50;
       for (int i = 0; i < budgets.Count; i += batchSize)
       {
           var batch = budgets.Skip(i).Take(batchSize);
           await _context.Budgets.AddRangeAsync(batch);
           await _context.SaveChangesAsync();
           _logger.LogDebug($"   - Seeded batch {i / batchSize + 1} ({batch.Count()} budgets)");
       }

       // Batch insert BudgetLines
       for (int i = 0; i < budgetLines.Count; i += batchSize)
       {
           var batch = budgetLines.Skip(i).Take(batchSize);
           await _context.BudgetLines.AddRangeAsync(batch);
           await _context.SaveChangesAsync();
           _logger.LogDebug($"   - Seeded batch {i / batchSize + 1} ({batch.Count()} budget lines)");
       }

       _logger.LogInformation($"✅ Seeded {budgets.Count} budgets with {budgetLines.Count} lines");
   }
}