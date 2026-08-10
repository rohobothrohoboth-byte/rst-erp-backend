using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class HrPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // ===== DASHBOARD =====
            new() { MenuKey = "hr.db", Key = "hr.db.view", Desc = "View HR Dashboard" },
            new() { MenuKey = "hr.db", Key = "hr.db.export", Desc = "Export Dashboard Data" },

            // ===== EMPLOYEE MANAGEMENT =====
            new() { MenuKey = "hr.emp", Key = "hr.emp.view", Desc = "View Employee List" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.dtl", Desc = "View Employee Details" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.add", Desc = "Add New Employee" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.mod", Desc = "Edit Employee" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.del", Desc = "Delete Employee" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.export", Desc = "Export Employees" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.import", Desc = "Import Employees" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.bulk", Desc = "Bulk Operations" },
            new() { MenuKey = "hr.emp", Key = "hr.emp.print", Desc = "Print Employee Details" },

            // Employee Profile
            new() { MenuKey = "hr.emp.profile", Key = "hr.emp.profile.view", Desc = "View Employee Profile" },
            new() { MenuKey = "hr.emp.profile", Key = "hr.emp.profile.mod", Desc = "Edit Employee Profile" },
            new() { MenuKey = "hr.emp.profile", Key = "hr.emp.profile.image", Desc = "Update Profile Image" },
            new() { MenuKey = "hr.emp.profile", Key = "hr.emp.profile.history", Desc = "View Profile History" },

            // Employee Documents
            new() { MenuKey = "hr.emp.document", Key = "hr.emp.document.view", Desc = "View Documents" },
            new() { MenuKey = "hr.emp.document", Key = "hr.emp.document.add", Desc = "Upload Documents" },
            new() { MenuKey = "hr.emp.document", Key = "hr.emp.document.mod", Desc = "Edit Document" },
            new() { MenuKey = "hr.emp.document", Key = "hr.emp.document.del", Desc = "Delete Documents" },
            new() { MenuKey = "hr.emp.document", Key = "hr.emp.document.download", Desc = "Download Documents" },

            // Contracts
            new() { MenuKey = "hr.emp.contract", Key = "hr.emp.contract.view", Desc = "View Contracts" },
            new() { MenuKey = "hr.emp.contract", Key = "hr.emp.contract.add", Desc = "Create Contract" },
            new() { MenuKey = "hr.emp.contract", Key = "hr.emp.contract.mod", Desc = "Edit Contract" },
            new() { MenuKey = "hr.emp.contract", Key = "hr.emp.contract.del", Desc = "Delete Contract" },
            new() { MenuKey = "hr.emp.contract", Key = "hr.emp.contract.renew", Desc = "Renew Contract" },
            new() { MenuKey = "hr.emp.contract", Key = "hr.emp.contract.terminate", Desc = "Terminate Contract" },

            // Performance Reviews
            new() { MenuKey = "hr.emp.performance", Key = "hr.emp.performance.view", Desc = "View Performance Reviews" },
            new() { MenuKey = "hr.emp.performance", Key = "hr.emp.performance.add", Desc = "Create Review" },
            new() { MenuKey = "hr.emp.performance", Key = "hr.emp.performance.mod", Desc = "Edit Review" },
            new() { MenuKey = "hr.emp.performance", Key = "hr.emp.performance.del", Desc = "Delete Review" },
            new() { MenuKey = "hr.emp.performance", Key = "hr.emp.performance.submit", Desc = "Submit Review" },
            new() { MenuKey = "hr.emp.performance", Key = "hr.emp.performance.approve", Desc = "Approve Review" },

            // Promotions
            new() { MenuKey = "hr.emp.promotion", Key = "hr.emp.promotion.view", Desc = "View Promotions" },
            new() { MenuKey = "hr.emp.promotion", Key = "hr.emp.promotion.add", Desc = "Create Promotion" },
            new() { MenuKey = "hr.emp.promotion", Key = "hr.emp.promotion.mod", Desc = "Edit Promotion" },
            new() { MenuKey = "hr.emp.promotion", Key = "hr.emp.promotion.del", Desc = "Delete Promotion" },
            new() { MenuKey = "hr.emp.promotion", Key = "hr.emp.promotion.approve", Desc = "Approve Promotion" },

            // Terminations
            new() { MenuKey = "hr.emp.termination", Key = "hr.emp.termination.view", Desc = "View Terminations" },
            new() { MenuKey = "hr.emp.termination", Key = "hr.emp.termination.add", Desc = "Process Termination" },
            new() { MenuKey = "hr.emp.termination", Key = "hr.emp.termination.mod", Desc = "Edit Termination" },
            new() { MenuKey = "hr.emp.termination", Key = "hr.emp.termination.approve", Desc = "Approve Termination" },

            // ===== LEAVE MANAGEMENT =====
            new() { MenuKey = "hr.leave", Key = "hr.leave.view", Desc = "View Leave Management" },
            new() { MenuKey = "hr.leave", Key = "hr.leave.manage", Desc = "Manage Leave" },
            new() { MenuKey = "hr.leave", Key = "hr.leave.config", Desc = "Configure Leave Settings" },

            new() { MenuKey = "my.leave", Key = "my.leave.view", Desc = "View My Leave" },
            new() { MenuKey = "my.leave", Key = "my.leave.add", Desc = "Request Leave" },
            new() { MenuKey = "my.leave", Key = "my.leave.mod", Desc = "Edit Request" },
            new() { MenuKey = "my.leave", Key = "my.leave.del", Desc = "Cancel Request" },
            new() { MenuKey = "my.leave", Key = "my.leave.withdraw", Desc = "Withdraw Request" },

            new() { MenuKey = "leave.balance", Key = "leave.balance.view", Desc = "View Leave Balance" },
            new() { MenuKey = "leave.balance", Key = "leave.balance.export", Desc = "Export Leave Balance" },

            new() { MenuKey = "leave.approve", Key = "leave.approve.view", Desc = "View Leave Requests" },
            new() { MenuKey = "leave.approve", Key = "leave.approve.process", Desc = "Approve/Reject Leave" },
            new() { MenuKey = "leave.approve", Key = "leave.approve.bulk", Desc = "Bulk Approval" },
            new() { MenuKey = "leave.approve", Key = "leave.approve.comment", Desc = "Add Comment" },

            new() { MenuKey = "leave.types", Key = "leave.types.view", Desc = "View Leave Types" },
            new() { MenuKey = "leave.types", Key = "leave.types.add", Desc = "Add Leave Type" },
            new() { MenuKey = "leave.types", Key = "leave.types.mod", Desc = "Edit Leave Type" },
            new() { MenuKey = "leave.types", Key = "leave.types.del", Desc = "Delete Leave Type" },
            new() { MenuKey = "leave.types", Key = "leave.types.quota", Desc = "Set Leave Quota" },

            new() { MenuKey = "leave.policies", Key = "leave.policies.view", Desc = "View Policies" },
            new() { MenuKey = "leave.policies", Key = "leave.policies.mod", Desc = "Edit Policies" },
            new() { MenuKey = "leave.policies", Key = "leave.policies.assign", Desc = "Assign Policy to Department" },

            // ===== ATTENDANCE =====
            new() { MenuKey = "hr.attend", Key = "hr.attend.view", Desc = "View Attendance" },
            new() { MenuKey = "hr.attend", Key = "hr.attend.manage", Desc = "Manage Attendance" },

            new() { MenuKey = "hr.attend.list", Key = "hr.attend.list.view", Desc = "View Attendance List" },
            new() { MenuKey = "hr.attend.list", Key = "hr.attend.list.export", Desc = "Export Attendance" },
            new() { MenuKey = "hr.attend.list", Key = "hr.attend.list.filter", Desc = "Filter Attendance" },

            new() { MenuKey = "hr.attend.shift", Key = "hr.attend.shift.view", Desc = "View Shifts" },
            new() { MenuKey = "hr.attend.shift", Key = "hr.attend.shift.add", Desc = "Create Shift" },
            new() { MenuKey = "hr.attend.shift", Key = "hr.attend.shift.mod", Desc = "Edit Shift" },
            new() { MenuKey = "hr.attend.shift", Key = "hr.attend.shift.del", Desc = "Delete Shift" },
            new() { MenuKey = "hr.attend.shift", Key = "hr.attend.shift.assign", Desc = "Assign Shift" },
            new() { MenuKey = "hr.attend.shift", Key = "hr.attend.shift.template", Desc = "Shift Templates" },

            new() { MenuKey = "hr.attend.checkin", Key = "hr.attend.checkin.do", Desc = "Check In/Out" },
            new() { MenuKey = "hr.attend.checkin", Key = "hr.attend.checkin.manual", Desc = "Manual Entry" },
            new() { MenuKey = "hr.attend.checkin", Key = "hr.attend.checkin.correction", Desc = "Request Correction" },

            new() { MenuKey = "hr.attend.report", Key = "hr.attend.report.view", Desc = "View Attendance Report" },
            new() { MenuKey = "hr.attend.report", Key = "hr.attend.report.export", Desc = "Export Report" },
            new() { MenuKey = "hr.attend.report", Key = "hr.attend.report.summary", Desc = "Summary Report" },

            // ===== PAYROLL =====
            new() { MenuKey = "hr.payroll", Key = "hr.payroll.view", Desc = "View Payroll" },
            new() { MenuKey = "hr.payroll", Key = "hr.payroll.manage", Desc = "Manage Payroll" },

            new() { MenuKey = "hr.payroll.run", Key = "hr.payroll.run.process", Desc = "Run Payroll" },
            new() { MenuKey = "hr.payroll.run", Key = "hr.payroll.run.preview", Desc = "Preview Payroll" },
            new() { MenuKey = "hr.payroll.run", Key = "hr.payroll.run.approve", Desc = "Approve Payroll" },
            new() { MenuKey = "hr.payroll.run", Key = "hr.payroll.run.post", Desc = "Post Payroll" },

            new() { MenuKey = "hr.payroll.history", Key = "hr.payroll.history.view", Desc = "View Payroll History" },
            new() { MenuKey = "hr.payroll.history", Key = "hr.payroll.history.export", Desc = "Export History" },
            new() { MenuKey = "hr.payroll.history", Key = "hr.payroll.history.reprint", Desc = "Reprint Payslip" },

            new() { MenuKey = "hr.payroll.salary", Key = "hr.payroll.salary.view", Desc = "View Salary Structure" },
            new() { MenuKey = "hr.payroll.salary", Key = "hr.payroll.salary.add", Desc = "Create Salary Structure" },
            new() { MenuKey = "hr.payroll.salary", Key = "hr.payroll.salary.mod", Desc = "Edit Salary Structure" },
            new() { MenuKey = "hr.payroll.salary", Key = "hr.payroll.salary.del", Desc = "Delete Salary Structure" },
            new() { MenuKey = "hr.payroll.salary", Key = "hr.payroll.salary.assign", Desc = "Assign to Employee" },
            new() { MenuKey = "hr.payroll.salary", Key = "hr.payroll.salary.increment", Desc = "Process Increment" },

            new() { MenuKey = "hr.payroll.tax", Key = "hr.payroll.tax.view", Desc = "View Tax Config" },
            new() { MenuKey = "hr.payroll.tax", Key = "hr.payroll.tax.mod", Desc = "Edit Tax Config" },
            new() { MenuKey = "hr.payroll.tax", Key = "hr.payroll.tax.calculate", Desc = "Calculate Tax" },
            new() { MenuKey = "hr.payroll.tax", Key = "hr.payroll.tax.report", Desc = "Tax Report" },

            // ===== TRAINING =====
            new() { MenuKey = "hr.training", Key = "hr.training.view", Desc = "View Training" },
            new() { MenuKey = "hr.training", Key = "hr.training.manage", Desc = "Manage Training" },

            new() { MenuKey = "hr.training.list", Key = "hr.training.list.view", Desc = "View Programs" },
            new() { MenuKey = "hr.training.list", Key = "hr.training.list.add", Desc = "Create Program" },
            new() { MenuKey = "hr.training.list", Key = "hr.training.list.mod", Desc = "Edit Program" },
            new() { MenuKey = "hr.training.list", Key = "hr.training.list.del", Desc = "Delete Program" },
            new() { MenuKey = "hr.training.list", Key = "hr.training.list.enroll", Desc = "Enroll Employees" },
            new() { MenuKey = "hr.training.list", Key = "hr.training.list.cancel", Desc = "Cancel Program" },

            new() { MenuKey = "hr.training.calendar", Key = "hr.training.calendar.view", Desc = "View Calendar" },
            new() { MenuKey = "hr.training.calendar", Key = "hr.training.calendar.add", Desc = "Add Event" },
            new() { MenuKey = "hr.training.calendar", Key = "hr.training.calendar.export", Desc = "Export Calendar" },

            new() { MenuKey = "hr.training.feedback", Key = "hr.training.feedback.view", Desc = "View Feedback" },
            new() { MenuKey = "hr.training.feedback", Key = "hr.training.feedback.add", Desc = "Submit Feedback" },
            new() { MenuKey = "hr.training.feedback", Key = "hr.training.feedback.export", Desc = "Export Feedback" },

            new() { MenuKey = "hr.training.certificate", Key = "hr.training.certificate.view", Desc = "View Certificates" },
            new() { MenuKey = "hr.training.certificate", Key = "hr.training.certificate.issue", Desc = "Issue Certificate" },
            new() { MenuKey = "hr.training.certificate", Key = "hr.training.certificate.print", Desc = "Print Certificate" },
            new() { MenuKey = "hr.training.certificate", Key = "hr.training.certificate.verify", Desc = "Verify Certificate" },

            // ===== HR REPORTS =====
            new() { MenuKey = "hr.reports", Key = "hr.reports.view", Desc = "View HR Reports" },
            new() { MenuKey = "hr.reports", Key = "hr.reports.export", Desc = "Export Reports" },
            new() { MenuKey = "hr.reports", Key = "hr.reports.schedule", Desc = "Schedule Reports" },

            new() { MenuKey = "hr.reports.employee", Key = "hr.reports.employee.view", Desc = "Employee Reports" },
            new() { MenuKey = "hr.reports.employee", Key = "hr.reports.employee.export", Desc = "Export Employee Reports" },

            new() { MenuKey = "hr.reports.attendance", Key = "hr.reports.attendance.view", Desc = "Attendance Reports" },
            new() { MenuKey = "hr.reports.attendance", Key = "hr.reports.attendance.export", Desc = "Export Attendance Reports" },

            new() { MenuKey = "hr.reports.leave", Key = "hr.reports.leave.view", Desc = "Leave Reports" },
            new() { MenuKey = "hr.reports.leave", Key = "hr.reports.leave.export", Desc = "Export Leave Reports" },

            new() { MenuKey = "hr.reports.payroll", Key = "hr.reports.payroll.view", Desc = "Payroll Reports" },
            new() { MenuKey = "hr.reports.payroll", Key = "hr.reports.payroll.export", Desc = "Export Payroll Reports" },

            new() { MenuKey = "hr.reports.recruitment", Key = "hr.reports.recruitment.view", Desc = "Recruitment Reports" },
            new() { MenuKey = "hr.reports.recruitment", Key = "hr.reports.recruitment.export", Desc = "Export Recruitment Reports" },

            // ============================================================
            // BASELINE PERMISSIONS (menus that previously had no actions)
            // ============================================================
            new() { MenuKey = "hr.emp.list", Key = "hr.emp.list.view", Desc = "View Employee List" },
            new() { MenuKey = "hr.emp.list", Key = "hr.emp.list.export", Desc = "Export Employee List" },
            new() { MenuKey = "hr.emp.add", Key = "hr.emp.add.view", Desc = "Access Add Employee" },
            new() { MenuKey = "hr.emp.add", Key = "hr.emp.add.add", Desc = "Create Employee" },
            new() { MenuKey = "hr.recruit", Key = "hr.recruit.view", Desc = "Access Recruitment" },
            new() { MenuKey = "hr.recruit.requisition.create", Key = "hr.recruit.requisition.create.view", Desc = "Access Create Requisition" },
            new() { MenuKey = "hr.recruit.requisition.create", Key = "hr.recruit.requisition.create.add", Desc = "Create Requisition" },
            new() { MenuKey = "hr.recruit.posting.create", Key = "hr.recruit.posting.create.view", Desc = "Access Create Posting" },
            new() { MenuKey = "hr.recruit.posting.create", Key = "hr.recruit.posting.create.add", Desc = "Create Posting" },
            new() { MenuKey = "hr.recruit.workforce.create", Key = "hr.recruit.workforce.create.view", Desc = "Access Create Plan" },
            new() { MenuKey = "hr.recruit.workforce.create", Key = "hr.recruit.workforce.create.add", Desc = "Create Workforce Plan" },
        };
    }
}