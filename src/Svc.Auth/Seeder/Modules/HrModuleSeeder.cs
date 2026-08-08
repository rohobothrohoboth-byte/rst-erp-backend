using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class HrModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ===== DASHBOARD =====
            new() { ModKey = "mod.hrm", Key = "hr.db", Label = "Dashboard", Path = "/hr", Icon = "LayoutDashboard", ParKey = "", IsChild = false, Order = 1 },

            // ===== EMPLOYEE MANAGEMENT =====
            new() { ModKey = "mod.hrm", Key = "hr.emp", Label = "Employee Management", Path = "", Icon = "Users", ParKey = "", IsChild = false, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.list", Label = "Employee List", Path = "/hr/employees/record", Icon = "List", ParKey = "hr.emp", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.add", Label = "Add New Employee", Path = "/hr/employees/record", Icon = "UserPlus", ParKey = "hr.emp", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.profile", Label = "Employee Profile", Path = "/hr/employees/profile", Icon = "User", ParKey = "hr.emp", IsChild = true, Order = 3 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.document", Label = "Employee Documents", Path = "/hr/employees/documents", Icon = "FileText", ParKey = "hr.emp", IsChild = true, Order = 4 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.contract", Label = "Contracts", Path = "/hr/employees/contracts", Icon = "FileCheck", ParKey = "hr.emp", IsChild = true, Order = 5 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.performance", Label = "Performance Reviews", Path = "/hr/employees/performance", Icon = "TrendingUp", ParKey = "hr.emp", IsChild = true, Order = 6 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.promotion", Label = "Promotions", Path = "/hr/employees/promotions", Icon = "ArrowUp", ParKey = "hr.emp", IsChild = true, Order = 7 },
            new() { ModKey = "mod.hrm", Key = "hr.emp.termination", Label = "Terminations", Path = "/hr/employees/terminations", Icon = "UserX", ParKey = "hr.emp", IsChild = true, Order = 8 },

            // ===== RECRUITMENT =====
            new() { ModKey = "mod.hrm", Key = "hr.recruit", Label = "Recruitment", Path = "", Icon = "ClipboardCheck", ParKey = "", IsChild = false, Order = 3 },

            // Dashboard & Analytics
            new() { ModKey = "mod.hrm", Key = "hr.recruit.dashboard", Label = "Recruitment Dashboard", Path = "/hr/recruitment/dashboard", Icon = "LayoutDashboard", ParKey = "hr.recruit", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.analytics", Label = "Analytics", Path = "/hr/recruitment/analytics", Icon = "BarChart", ParKey = "hr.recruit", IsChild = true, Order = 2 },

            // Workforce Planning
            new() { ModKey = "mod.hrm", Key = "hr.recruit.workforce", Label = "Workforce Planning", Path = "", Icon = "Building2", ParKey = "hr.recruit", IsChild = true, Order = 3 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.workforce.plans", Label = "Workforce Plans", Path = "/hr/recruitment/workforce-plans", Icon = "ClipboardList", ParKey = "hr.recruit.workforce", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.workforce.create", Label = "Create Plan", Path = "/hr/recruitment/workforce-plan/new", Icon = "Plus", ParKey = "hr.recruit.workforce", IsChild = true, Order = 2 },

            // Job Requisition
            new() { ModKey = "mod.hrm", Key = "hr.recruit.requisition", Label = "Job Requisitions", Path = "", Icon = "FileText", ParKey = "hr.recruit", IsChild = true, Order = 4 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.requisition.list", Label = "All Requisitions", Path = "/hr/recruitment/requisitions", Icon = "List", ParKey = "hr.recruit.requisition", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.requisition.create", Label = "Create Requisition", Path = "/hr/recruitment/requisition/new", Icon = "Plus", ParKey = "hr.recruit.requisition", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.requisition.approved", Label = "Approved Requisitions", Path = "/hr/recruitment/approved-requisitions", Icon = "CheckCircle", ParKey = "hr.recruit.requisition", IsChild = true, Order = 3 },

            // Job Posting
            new() { ModKey = "mod.hrm", Key = "hr.recruit.posting", Label = "Job Postings", Path = "", Icon = "Megaphone", ParKey = "hr.recruit", IsChild = true, Order = 5 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.posting.list", Label = "All Postings", Path = "/hr/recruitment/postings", Icon = "List", ParKey = "hr.recruit.posting", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.posting.create", Label = "Create Posting", Path = "/hr/recruitment/posting/new", Icon = "Plus", ParKey = "hr.recruit.posting", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.posting.published", Label = "Published Postings", Path = "/hr/recruitment/jobs", Icon = "CheckCircle", ParKey = "hr.recruit.posting", IsChild = true, Order = 3 },

            // Applicant Management
            new() { ModKey = "mod.hrm", Key = "hr.recruit.applicant", Label = "Applicants", Path = "", Icon = "Users", ParKey = "hr.recruit", IsChild = true, Order = 6 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.applicant.list", Label = "All Applicants", Path = "/hr/recruitment/applicants", Icon = "Users", ParKey = "hr.recruit.applicant", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.applicant.evaluate", Label = "Evaluate Applicant", Path = "/hr/recruitment/applicant/evaluate", Icon = "ClipboardCheck", ParKey = "hr.recruit.applicant", IsChild = true, Order = 2 },

            // Interview Management
            new() { ModKey = "mod.hrm", Key = "hr.recruit.interview", Label = "Interviews", Path = "", Icon = "Calendar", ParKey = "hr.recruit", IsChild = true, Order = 7 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.interview.list", Label = "All Interviews", Path = "/hr/recruitment/interviews", Icon = "List", ParKey = "hr.recruit.interview", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.interview.schedule", Label = "Schedule Interview", Path = "/hr/recruitment/interview/schedule", Icon = "CalendarPlus", ParKey = "hr.recruit.interview", IsChild = true, Order = 2 },

            // Offer Management
            new() { ModKey = "mod.hrm", Key = "hr.recruit.offer", Label = "Offers", Path = "", Icon = "FileCheck", ParKey = "hr.recruit", IsChild = true, Order = 8 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.offer.list", Label = "All Offers", Path = "/hr/recruitment/offers", Icon = "List", ParKey = "hr.recruit.offer", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.offer.create", Label = "Create Offer", Path = "/hr/recruitment/offer/new", Icon = "Plus", ParKey = "hr.recruit.offer", IsChild = true, Order = 2 },

            // Onboarding
            new() { ModKey = "mod.hrm", Key = "hr.recruit.onboard", Label = "Onboarding", Path = "", Icon = "UserPlus", ParKey = "hr.recruit", IsChild = true, Order = 9 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.onboard.tasks", Label = "Onboarding Tasks", Path = "/hr/recruitment/onboarding/tasks", Icon = "ClipboardList", ParKey = "hr.recruit.onboard", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.onboard.assignments", Label = "Assignments", Path = "/hr/recruitment/onboarding/assignments", Icon = "UserCheck", ParKey = "hr.recruit.onboard", IsChild = true, Order = 2 },

            // Evaluation
            new() { ModKey = "mod.hrm", Key = "hr.recruit.evaluation", Label = "Evaluation", Path = "", Icon = "Star", ParKey = "hr.recruit", IsChild = true, Order = 10 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.evaluation.flow", Label = "Evaluation Flows", Path = "/hr/recruitment/posting/:postId/eval-flow", Icon = "GitBranch", ParKey = "hr.recruit.evaluation", IsChild = true, Order = 1 },

            // ===== LEAVE MANAGEMENT =====
            new() { ModKey = "mod.hrm", Key = "hr.leave", Label = "Leave Management", Path = "", Icon = "CalendarDays", ParKey = "", IsChild = false, Order = 4 },
            new() { ModKey = "mod.hrm", Key = "my.leave", Label = "My Leave", Path = "/hr/leave/list", Icon = "Calendar", ParKey = "hr.leave", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "leave.balance", Label = "Leave Balance", Path = "/hr/leave/balance", Icon = "BarChart", ParKey = "hr.leave", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "leave.approve", Label = "Leave Approval", Path = "/hr/leave/approval", Icon = "ClipboardCheck", ParKey = "hr.leave", IsChild = true, Order = 3 },
            new() { ModKey = "mod.hrm", Key = "leave.types", Label = "Leave Types", Path = "/hr/leave/types", Icon = "Tag", ParKey = "hr.leave", IsChild = true, Order = 4 },
            new() { ModKey = "mod.hrm", Key = "leave.policies", Label = "Leave Policies", Path = "/hr/leave/policies", Icon = "FileText", ParKey = "hr.leave", IsChild = true, Order = 5 },

            // ===== ATTENDANCE =====
            new() { ModKey = "mod.hrm", Key = "hr.attend", Label = "Attendance", Path = "", Icon = "Clock", ParKey = "", IsChild = false, Order = 5 },
            new() { ModKey = "mod.hrm", Key = "hr.attend.list", Label = "Attendance List", Path = "/hr/attendance/list", Icon = "List", ParKey = "hr.attend", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.attend.shift", Label = "Shift Schedule", Path = "/hr/shift-scheduler", Icon = "Calendar", ParKey = "hr.attend", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.attend.checkin", Label = "Check In/Out", Path = "/hr/attendance/checkin", Icon = "Clock", ParKey = "hr.attend", IsChild = true, Order = 3 },
            new() { ModKey = "mod.hrm", Key = "hr.attend.report", Label = "Attendance Report", Path = "/hr/attendance/report", Icon = "FileSpreadsheet", ParKey = "hr.attend", IsChild = true, Order = 4 },

            // ===== PAYROLL =====
            new() { ModKey = "mod.hrm", Key = "hr.payroll", Label = "Payroll", Path = "", Icon = "DollarSign", ParKey = "", IsChild = false, Order = 6 },
            new() { ModKey = "mod.hrm", Key = "hr.payroll.run", Label = "Run Payroll", Path = "/hr/payroll/run", Icon = "Play", ParKey = "hr.payroll", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.payroll.history", Label = "Payroll History", Path = "/hr/payroll/history", Icon = "History", ParKey = "hr.payroll", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.payroll.salary", Label = "Salary Structure", Path = "/hr/payroll/salary-structure", Icon = "FileText", ParKey = "hr.payroll", IsChild = true, Order = 3 },
            new() { ModKey = "mod.hrm", Key = "hr.payroll.tax", Label = "Tax Configurations", Path = "/hr/payroll/tax", Icon = "Calculator", ParKey = "hr.payroll", IsChild = true, Order = 4 },

            // ===== TRAINING =====
            new() { ModKey = "mod.hrm", Key = "hr.training", Label = "Training & Development", Path = "", Icon = "GraduationCap", ParKey = "", IsChild = false, Order = 7 },
            new() { ModKey = "mod.hrm", Key = "hr.training.list", Label = "Training Programs", Path = "/hr/training/programs", Icon = "List", ParKey = "hr.training", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.training.calendar", Label = "Training Calendar", Path = "/hr/training/calendar", Icon = "Calendar", ParKey = "hr.training", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.training.feedback", Label = "Feedback", Path = "/hr/training/feedback", Icon = "MessageSquare", ParKey = "hr.training", IsChild = true, Order = 3 },
            new() { ModKey = "mod.hrm", Key = "hr.training.certificate", Label = "Certifications", Path = "/hr/training/certificates", Icon = "Award", ParKey = "hr.training", IsChild = true, Order = 4 },

            // ===== HR REPORTS =====
            new() { ModKey = "mod.hrm", Key = "hr.reports", Label = "HR Reports", Path = "", Icon = "FileSpreadsheet", ParKey = "", IsChild = false, Order = 8 },
            new() { ModKey = "mod.hrm", Key = "hr.reports.employee", Label = "Employee Reports", Path = "/hr/reports/employees", Icon = "Users", ParKey = "hr.reports", IsChild = true, Order = 1 },
            new() { ModKey = "mod.hrm", Key = "hr.reports.attendance", Label = "Attendance Reports", Path = "/hr/reports/attendance", Icon = "Clock", ParKey = "hr.reports", IsChild = true, Order = 2 },
            new() { ModKey = "mod.hrm", Key = "hr.reports.leave", Label = "Leave Reports", Path = "/hr/reports/leave", Icon = "CalendarDays", ParKey = "hr.reports", IsChild = true, Order = 3 },
            new() { ModKey = "mod.hrm", Key = "hr.reports.payroll", Label = "Payroll Reports", Path = "/hr/reports/payroll", Icon = "DollarSign", ParKey = "hr.reports", IsChild = true, Order = 4 },
            new() { ModKey = "mod.hrm", Key = "hr.reports.recruitment", Label = "Recruitment Reports", Path = "/hr/reports/recruitment", Icon = "ClipboardCheck", ParKey = "hr.reports", IsChild = true, Order = 5 },
            new() { ModKey = "mod.hrm", Key = "hr.recruit.workforce", Label = "Workforce Planning", Path = "", Icon = "Building2", ParKey = "hr.recruit", IsChild = true, Order = 3 },
new() { ModKey = "mod.hrm", Key = "hr.leave", Label = "Leave Management", Path = "", Icon = "CalendarDays", ParKey = "", IsChild = false, Order = 4 },
new() { ModKey = "mod.hrm", Key = "my.leave", Label = "My Leave", Path = "/hr/leave/list", Icon = "Calendar", ParKey = "hr.leave", IsChild = true, Order = 1 },
new() { ModKey = "mod.hrm", Key = "leave.balance", Label = "Leave Balance", Path = "/hr/leave/balance", Icon = "BarChart", ParKey = "hr.leave", IsChild = true, Order = 2 },
new() { ModKey = "mod.hrm", Key = "leave.approve", Label = "Leave Approval", Path = "/hr/leave/approval", Icon = "ClipboardCheck", ParKey = "hr.leave", IsChild = true, Order = 3 },
new() { ModKey = "mod.hrm", Key = "leave.types", Label = "Leave Types", Path = "/hr/leave/types", Icon = "Tag", ParKey = "hr.leave", IsChild = true, Order = 4 },
new() { ModKey = "mod.hrm", Key = "leave.policies", Label = "Leave Policies", Path = "/hr/leave/policies", Icon = "FileText", ParKey = "hr.leave", IsChild = true, Order = 5 },
        };
    }
}