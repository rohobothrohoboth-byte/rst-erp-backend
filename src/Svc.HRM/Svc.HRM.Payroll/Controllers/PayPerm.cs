namespace Svc.HRM.Payroll.Controllers;

// The Payroll service backs BOTH the HR Payroll menu (hr.payroll.*) and the
// Finance Payroll menu (fnm.payroll.*). A user may be granted either family, so
// these pipe-joined sets use PerAuth's OR semantics: the action is allowed if
// the user holds ANY listed permission. (Unknown keys are ignored at policy
// build time, so listing both families is safe.)
public static class PayPerm
{
    public const string SalaryView =
        "fnm.payroll.salaries.view|hr.payroll.salary.view|fnm.payroll.view|hr.payroll.view";
    public const string SalaryManage =
        "fnm.payroll.salaries.add|fnm.payroll.salaries.mod|hr.payroll.salary.add|hr.payroll.salary.mod|hr.payroll.salary.del|fnm.payroll.manage|hr.payroll.manage";

    public const string RunView =
        "fnm.payroll.run.view|fnm.payroll.view|hr.payroll.view";
    public const string RunCreate =
        "fnm.payroll.run.create|fnm.payroll.manage|hr.payroll.manage";
    public const string RunProcess =
        "fnm.payroll.run.process|hr.payroll.run.process|fnm.payroll.manage|hr.payroll.manage";
    public const string RunApprove =
        "fnm.payroll.run.approve|hr.payroll.run.approve|fnm.payroll.manage|hr.payroll.manage";

    public const string PayslipView =
        "fnm.payroll.payslips.view|hr.payroll.history.view|fnm.payroll.view|hr.payroll.view";
    public const string PayslipGenerate =
        "fnm.payroll.payslips.generate|fnm.payroll.manage|hr.payroll.manage";
    public const string PayslipDownload =
        "fnm.payroll.payslips.download|fnm.payroll.payslips.view|hr.payroll.history.view|fnm.payroll.view|hr.payroll.view";

    public const string ReportView =
        "fnm.payroll.reports.view|hr.reports.payroll.view|fnm.payroll.view|hr.payroll.view";

    public const string StructureView =
        "fnm.payroll.structure.view|hr.payroll.salary.view|fnm.payroll.view|hr.payroll.view";
    public const string StructureManage =
        "fnm.payroll.structure.add|fnm.payroll.structure.mod|fnm.payroll.structure.del|hr.payroll.salary.add|hr.payroll.salary.mod|hr.payroll.salary.del|fnm.payroll.manage|hr.payroll.manage";

    public const string TaxView =
        "fnm.payroll.tax.view|hr.payroll.tax.view|fnm.payroll.view|hr.payroll.view";
    public const string TaxCalculate =
        "fnm.payroll.tax.calculate|hr.payroll.tax.calculate|fnm.payroll.manage|hr.payroll.manage";
    public const string TaxManage =
        "fnm.payroll.tax.add|hr.payroll.tax.mod|fnm.payroll.manage|hr.payroll.manage";
}
