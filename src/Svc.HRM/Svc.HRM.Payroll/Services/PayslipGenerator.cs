using Microsoft.EntityFrameworkCore;
using Svc.HRM.Payroll.Models.DTOs;
using Svc.HRM.Payroll.Models.Entities;
using Svc.HRM.Payroll.Persistence;
using System.Text;

namespace Svc.HRM.Payroll.Services;

public class PayslipGenerator : IPayslipGenerator
{
    private readonly PayrollDbContext _context;
    private readonly ILogger<PayslipGenerator> _logger;

    public PayslipGenerator(PayrollDbContext context, ILogger<PayslipGenerator> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PayslipDto> GeneratePayslipAsync(Guid payrollEmployeeId, CancellationToken ct = default)
    {
        var payrollEmployee = await _context.PayrollEmployees
            .Include(x => x.PayrollRun)
            .FirstOrDefaultAsync(x => x.Id == payrollEmployeeId && !x.IsDeleted, ct);
        if (payrollEmployee == null)
            throw new KeyNotFoundException($"Payroll employee {payrollEmployeeId} not found");

        // Check if payslip already exists
        var existingPayslip = await _context.Payslips
            .FirstOrDefaultAsync(x => x.PayrollEmployeeId == payrollEmployeeId && !x.IsDeleted, ct);

        if (existingPayslip != null)
            return MapToPayslipDto(existingPayslip);

        var payslip = new LocalPayslip
        {
            Id = Guid.CreateVersion7(),
            PayrollEmployeeId = payrollEmployeeId,
            EmployeeId = payrollEmployee.EmployeeId,
            PayslipNumber = $"PS-{DateTime.UtcNow:yyyyMMdd}-{payrollEmployeeId.ToString().Substring(0, 8)}",
            PeriodStart = payrollEmployee.PayrollRun.PayPeriodStart,
            PeriodEnd = payrollEmployee.PayrollRun.PayPeriodEnd,
            PaymentDate = payrollEmployee.PayrollRun.PaymentDate,
            GrossPay = payrollEmployee.GrossPay,
            TotalAllowances = payrollEmployee.HousingAllowance + payrollEmployee.TransportAllowance +
                              payrollEmployee.MealAllowance + payrollEmployee.MedicalAllowance +
                              payrollEmployee.OtherAllowances,
            TotalDeductions = payrollEmployee.PensionContribution + payrollEmployee.OtherDeductions,
            TaxAmount = payrollEmployee.TaxAmount,
            NetPay = payrollEmployee.NetPay,
            IsGenerated = true,
            GeneratedAt = DateTime.UtcNow,
            GeneratedBy = "System",
            Notes = payrollEmployee.Notes
        };

        await _context.Payslips.AddAsync(payslip, ct);
        await _context.SaveChangesAsync(ct);

        return MapToPayslipDto(payslip);
    }

    public async Task<byte[]> GeneratePayslipPdfAsync(Guid payslipId, CancellationToken ct = default)
    {
        var payslip = await _context.Payslips
            .Include(x => x.PayrollEmployee)
            .FirstOrDefaultAsync(x => x.Id == payslipId && !x.IsDeleted, ct);
        if (payslip == null)
            throw new KeyNotFoundException($"Payslip {payslipId} not found");

        var html = GenerateHtmlPayslip(payslip);
        return Encoding.UTF8.GetBytes(html);
    }

   private string GenerateHtmlPayslip(LocalPayslip payslip)
   {
       var employee = payslip.PayrollEmployee;
       var sb = new StringBuilder();

       sb.AppendLine("<!DOCTYPE html>");
       sb.AppendLine("<html>");
       sb.AppendLine("<head>");
       sb.AppendLine("<meta charset='UTF-8'>");
       sb.AppendLine("<title>Payslip</title>");
       sb.AppendLine(@"<style>
           * { margin: 0; padding: 0; box-sizing: border-box; }
           body {
               font-family: 'Segoe UI', Arial, sans-serif;
               background: #f0f2f5;
               padding: 40px;
               display: flex;
               justify-content: center;
           }
           .payslip-container {
               max-width: 900px;
               width: 100%;
               background: white;
               border-radius: 16px;
               box-shadow: 0 8px 40px rgba(0,0,0,0.12);
               padding: 50px 60px;
           }
           .header {
               text-align: center;
               border-bottom: 4px solid #1a237e;
               padding-bottom: 25px;
               margin-bottom: 30px;
           }
           .company-name {
               font-size: 32px;
               font-weight: 700;
               color: #1a237e;
               letter-spacing: 1px;
           }
           .company-sub {
               font-size: 14px;
               color: #666;
               margin-top: 5px;
           }
           .payslip-title {
               font-size: 24px;
               font-weight: 700;
               color: #1a237e;
               margin-top: 12px;
               letter-spacing: 2px;
           }
           .payslip-number {
               font-size: 14px;
               color: #888;
               margin-top: 5px;
               font-weight: 500;
           }
           .payslip-date {
               font-size: 13px;
               color: #999;
               margin-top: 3px;
           }
           .section {
               margin-top: 25px;
           }
           .section-title {
               font-size: 17px;
               font-weight: 600;
               color: #1a237e;
               border-bottom: 2px solid #e8eaf6;
               padding-bottom: 8px;
               margin-bottom: 15px;
               text-transform: uppercase;
               letter-spacing: 0.5px;
           }
           .info-grid {
               display: grid;
               grid-template-columns: 1fr 1fr;
               gap: 6px 30px;
           }
           .info-item {
               display: flex;
               justify-content: space-between;
               padding: 8px 0;
               border-bottom: 1px solid #f5f5f5;
           }
           .info-label {
               color: #666;
               font-weight: 500;
               font-size: 14px;
           }
           .info-value {
               font-weight: 600;
               font-size: 14px;
               color: #1a1a1a;
           }
           .table {
               width: 100%;
               border-collapse: collapse;
           }
           .table th {
               text-align: left;
               padding: 12px 10px;
               border-bottom: 2px solid #e0e0e0;
               color: #1a237e;
               font-weight: 600;
               font-size: 14px;
               text-transform: uppercase;
               letter-spacing: 0.3px;
           }
           .table td {
               padding: 10px 10px;
               border-bottom: 1px solid #f0f0f0;
               font-size: 14px;
           }
           .table .amount {
               text-align: right;
               font-weight: 500;
           }
           .table .total-row td {
               font-weight: 700;
               border-top: 2px solid #1a237e;
               padding-top: 14px;
               font-size: 15px;
           }
           .table .total-row .amount {
               font-size: 16px;
           }
           .net-pay {
               background: linear-gradient(135deg, #1a237e, #283593);
               color: white;
               padding: 18px 25px;
               border-radius: 10px;
               margin-top: 25px;
               display: flex;
               justify-content: space-between;
               align-items: center;
               box-shadow: 0 4px 15px rgba(26, 35, 126, 0.3);
           }
           .net-pay .label {
               font-size: 20px;
               font-weight: 600;
               letter-spacing: 1px;
           }
           .net-pay .amount {
               font-size: 28px;
               font-weight: 700;
           }
           .notes-box {
               margin-top: 20px;
               padding: 15px 20px;
               background: #f8f9fa;
               border-radius: 8px;
               border-left: 4px solid #1a237e;
               font-size: 14px;
               color: #555;
           }
           .notes-box strong {
               color: #1a237e;
           }
           .footer {
               margin-top: 35px;
               text-align: center;
               font-size: 13px;
               color: #aaa;
               border-top: 1px solid #e8eaf6;
               padding-top: 20px;
           }
           .footer .highlight {
               color: #1a237e;
               font-weight: 500;
           }
           @media print {
               body { background: white; padding: 20px; }
               .payslip-container { box-shadow: none; padding: 30px; }
               .net-pay { background: #1a237e !important; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
           }
           @media (max-width: 600px) {
               .payslip-container { padding: 20px; }
               .info-grid { grid-template-columns: 1fr; }
               .net-pay { flex-direction: column; gap: 10px; text-align: center; }
               .net-pay .amount { font-size: 24px; }
           }
       </style>");
       sb.AppendLine("</head>");
       sb.AppendLine("<body>");
       sb.AppendLine("<div class='payslip-container'>");

       // Header
       sb.AppendLine("<div class='header'>");
       sb.AppendLine("<div class='company-name'>🏢 RST ERP</div>");
       sb.AppendLine("<div class='company-sub'>Human Resource Management System</div>");
       sb.AppendLine("<div class='payslip-title'>PAYSLIP</div>");
       sb.AppendLine($"<div class='payslip-number'>📄 #{payslip.PayslipNumber}</div>");
       sb.AppendLine($"<div class='payslip-date'>📅 Generated: {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC</div>");
       sb.AppendLine("</div>");

       // Employee Information
       sb.AppendLine("<div class='section'>");
       sb.AppendLine("<div class='section-title'>👤 Employee Information</div>");
       sb.AppendLine("<div class='info-grid'>");
       sb.AppendLine($"<div class='info-item'><span class='info-label'>Employee Name</span><span class='info-value'>{employee?.EmployeeName ?? "N/A"}</span></div>");
       sb.AppendLine($"<div class='info-item'><span class='info-label'>Employee Code</span><span class='info-value'>{employee?.EmployeeCode ?? "N/A"}</span></div>");
       sb.AppendLine($"<div class='info-item'><span class='info-label'>Department</span><span class='info-value'>{employee?.Department ?? "N/A"}</span></div>");
       sb.AppendLine($"<div class='info-item'><span class='info-label'>Position</span><span class='info-value'>{employee?.Position ?? "N/A"}</span></div>");
       sb.AppendLine($"<div class='info-item'><span class='info-label'>Pay Period</span><span class='info-value'>{payslip.PeriodStart:dd/MM/yyyy} - {payslip.PeriodEnd:dd/MM/yyyy}</span></div>");
       sb.AppendLine($"<div class='info-item'><span class='info-label'>Payment Date</span><span class='info-value'>{payslip.PaymentDate:dd/MM/yyyy}</span></div>");
       sb.AppendLine("</div>");
       sb.AppendLine("</div>");

       // Earnings
       sb.AppendLine("<div class='section'>");
       sb.AppendLine("<div class='section-title'>💰 Earnings</div>");
       sb.AppendLine("<table class='table'>");
       sb.AppendLine("<tr><th>Description</th><th class='amount'>Amount (ETB)</th></tr>");
       sb.AppendLine($"<tr><td>Base Salary</td><td class='amount'>{employee?.BaseSalary:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Housing Allowance</td><td class='amount'>{employee?.HousingAllowance:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Transport Allowance</td><td class='amount'>{employee?.TransportAllowance:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Meal Allowance</td><td class='amount'>{employee?.MealAllowance:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Medical Allowance</td><td class='amount'>{employee?.MedicalAllowance:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Other Allowances</td><td class='amount'>{employee?.OtherAllowances:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Overtime Pay</td><td class='amount'>{employee?.OvertimePay:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Bonus Pay</td><td class='amount'>{employee?.BonusPay:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Commission Pay</td><td class='amount'>{employee?.CommissionPay:N2}</td></tr>");
       sb.AppendLine($"<tr class='total-row'><td><strong>TOTAL GROSS PAY</strong></td><td class='amount'><strong>{payslip.GrossPay:N2}</strong></td></tr>");
       sb.AppendLine("</table>");
       sb.AppendLine("</div>");

       // Deductions
       sb.AppendLine("<div class='section'>");
       sb.AppendLine("<div class='section-title'>📉 Deductions</div>");
       sb.AppendLine("<table class='table'>");
       sb.AppendLine("<tr><th>Description</th><th class='amount'>Amount (ETB)</th></tr>");
       sb.AppendLine($"<tr><td>Income Tax</td><td class='amount'>{payslip.TaxAmount:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Pension Contribution</td><td class='amount'>{employee?.PensionContribution:N2}</td></tr>");
       sb.AppendLine($"<tr><td>Other Deductions</td><td class='amount'>{employee?.OtherDeductions:N2}</td></tr>");
       var totalDeductions = payslip.TaxAmount + (employee?.PensionContribution ?? 0) + (employee?.OtherDeductions ?? 0);
       sb.AppendLine($"<tr class='total-row'><td><strong>TOTAL DEDUCTIONS</strong></td><td class='amount'><strong>{totalDeductions:N2}</strong></td></tr>");
       sb.AppendLine("</table>");
       sb.AppendLine("</div>");

       // Net Pay
       sb.AppendLine("<div class='net-pay'>");
       sb.AppendLine("<span class='label'>💵 NET PAY</span>");
       sb.AppendLine($"<span class='amount'>ETB {payslip.NetPay:N2}</span>");
       sb.AppendLine("</div>");

       // Notes
       if (!string.IsNullOrEmpty(payslip.Notes))
       {
           sb.AppendLine($"<div class='notes-box'>");
           sb.AppendLine($"<strong>📝 Notes:</strong> {payslip.Notes}");
           sb.AppendLine("</div>");
       }

       // Summary
       sb.AppendLine("<div style='margin-top: 20px; display: grid; grid-template-columns: repeat(3, 1fr); gap: 15px;'>");
       sb.AppendLine($"<div style='background: #e8f5e9; padding: 12px; border-radius: 8px; text-align: center;'><div style='font-size: 12px; color: #666;'>Days Worked</div><div style='font-size: 18px; font-weight: bold; color: #2e7d32;'>{employee?.DaysWorked ?? 0}</div></div>");
       sb.AppendLine($"<div style='background: #fff3e0; padding: 12px; border-radius: 8px; text-align: center;'><div style='font-size: 12px; color: #666;'>Days Absent</div><div style='font-size: 18px; font-weight: bold; color: #e65100;'>{employee?.DaysAbsent ?? 0}</div></div>");
       sb.AppendLine($"<div style='background: #e3f2fd; padding: 12px; border-radius: 8px; text-align: center;'><div style='font-size: 12px; color: #666;'>Overtime Hours</div><div style='font-size: 18px; font-weight: bold; color: #0d47a1;'>{employee?.OvertimeHours ?? 0}</div></div>");
       sb.AppendLine("</div>");

       // Footer
       sb.AppendLine("<div class='footer'>");
       sb.AppendLine("This is a computer-generated payslip. No signature is required.");
       sb.AppendLine("<br>");
       sb.AppendLine($"<span class='highlight'>RST ERP</span> © {DateTime.UtcNow.Year} - All rights reserved");
       sb.AppendLine("</div>");

       sb.AppendLine("</div>");
       sb.AppendLine("</body>");
       sb.AppendLine("</html>");

       return sb.ToString();
   }

    private static PayslipDto MapToPayslipDto(LocalPayslip entity)
    {
        return new PayslipDto
        {
            Id = entity.Id,
            PayslipNumber = entity.PayslipNumber,



            EmployeeName = entity.PayrollEmployee?.EmployeeName ?? string.Empty,
            EmployeeCode = entity.PayrollEmployee?.EmployeeCode ?? string.Empty,
            Department = entity.PayrollEmployee?.Department ?? string.Empty,
            Position = entity.PayrollEmployee?.Position ?? string.Empty,


            PeriodStart = entity.PeriodStart,
            PeriodEnd = entity.PeriodEnd,
            PaymentDate = entity.PaymentDate,
            GrossPay = entity.GrossPay,
            HousingAllowance = entity.PayrollEmployee?.HousingAllowance ?? 0,
            TransportAllowance = entity.PayrollEmployee?.TransportAllowance ?? 0,
            MealAllowance = entity.PayrollEmployee?.MealAllowance ?? 0,
            MedicalAllowance = entity.PayrollEmployee?.MedicalAllowance ?? 0,
            OtherAllowances = entity.PayrollEmployee?.OtherAllowances ?? 0,
            TotalAllowances = entity.TotalAllowances,
            OvertimePay = entity.PayrollEmployee?.OvertimePay ?? 0,
            BonusPay = entity.PayrollEmployee?.BonusPay ?? 0,
            CommissionPay = entity.PayrollEmployee?.CommissionPay ?? 0,
            TaxAmount = entity.TaxAmount,
            PensionContribution = entity.PayrollEmployee?.PensionContribution ?? 0,
            OtherDeductions = entity.PayrollEmployee?.OtherDeductions ?? 0,
            TotalDeductions = entity.TotalDeductions,
            NetPay = entity.NetPay,
            PaymentMethod = entity.PaymentMethod,
            BankAccount = entity.BankAccount,
            DaysWorked = entity.PayrollEmployee?.DaysWorked ?? 0,
            DaysAbsent = entity.PayrollEmployee?.DaysAbsent ?? 0,
            OvertimeHours = entity.PayrollEmployee?.OvertimeHours ?? 0,
            Notes = entity.Notes,
            IsGenerated = entity.IsGenerated,
            GeneratedAt = entity.GeneratedAt
        };
    }
}