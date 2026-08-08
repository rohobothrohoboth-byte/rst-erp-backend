using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class RecruitmentPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // ===== DASHBOARD & ANALYTICS =====
            new() { MenuKey = "hr.recruit.dashboard", Key = "hr.recruit.dashboard.view", Desc = "View Recruitment Dashboard" },
            new() { MenuKey = "hr.recruit.dashboard", Key = "hr.recruit.dashboard.export", Desc = "Export Dashboard Data" },

            new() { MenuKey = "hr.recruit.analytics", Key = "hr.recruit.analytics.view", Desc = "View Recruitment Analytics" },
            new() { MenuKey = "hr.recruit.analytics", Key = "hr.recruit.analytics.export", Desc = "Export Analytics Data" },

            // ===== WORKFORCE PLANNING =====
            new() { MenuKey = "hr.recruit.workforce", Key = "hr.recruit.workforce.view", Desc = "Access Workforce Planning" },
            new() { MenuKey = "hr.recruit.workforce", Key = "hr.recruit.workforce.manage", Desc = "Manage Workforce Planning" },

            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.view", Desc = "View Workforce Plans" },
            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.add", Desc = "Create Workforce Plan" },
            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.mod", Desc = "Edit Workforce Plan" },
            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.del", Desc = "Delete Workforce Plan" },
            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.review", Desc = "Review Workforce Plan" },
            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.approve", Desc = "Approve Workforce Plan" },
            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.reject", Desc = "Reject Workforce Plan" },
            new() { MenuKey = "hr.recruit.workforce.plans", Key = "hr.recruit.workforce.plans.export", Desc = "Export Workforce Plans" },

            // ===== JOB REQUISITIONS =====
            new() { MenuKey = "hr.recruit.requisition", Key = "hr.recruit.requisition.view", Desc = "Access Job Requisitions" },
            new() { MenuKey = "hr.recruit.requisition", Key = "hr.recruit.requisition.manage", Desc = "Manage Job Requisitions" },

            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.view", Desc = "View Requisitions" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.add", Desc = "Create Requisition" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.mod", Desc = "Edit Requisition" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.del", Desc = "Delete Requisition" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.copy", Desc = "Copy Requisition" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.submit", Desc = "Submit for Approval" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.approve", Desc = "Approve Requisition" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.reject", Desc = "Reject Requisition" },
            new() { MenuKey = "hr.recruit.requisition.list", Key = "hr.recruit.requisition.list.export", Desc = "Export Requisitions" },

            new() { MenuKey = "hr.recruit.requisition.approved", Key = "hr.recruit.requisition.approved.view", Desc = "View Approved Requisitions" },
            new() { MenuKey = "hr.recruit.requisition.approved", Key = "hr.recruit.requisition.approved.post", Desc = "Create Posting from Requisition" },

            // ===== JOB POSTINGS =====
            new() { MenuKey = "hr.recruit.posting", Key = "hr.recruit.posting.view", Desc = "Access Job Postings" },
            new() { MenuKey = "hr.recruit.posting", Key = "hr.recruit.posting.manage", Desc = "Manage Job Postings" },

            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.view", Desc = "View Postings" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.add", Desc = "Create Posting" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.mod", Desc = "Edit Posting" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.del", Desc = "Delete Posting" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.publish", Desc = "Publish Posting" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.unpublish", Desc = "Unpublish Posting" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.close", Desc = "Close Posting" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.repost", Desc = "Repost Posting" },
            new() { MenuKey = "hr.recruit.posting.list", Key = "hr.recruit.posting.list.export", Desc = "Export Postings" },

            new() { MenuKey = "hr.recruit.posting.published", Key = "hr.recruit.posting.published.view", Desc = "View Published Postings" },
            new() { MenuKey = "hr.recruit.posting.published", Key = "hr.recruit.posting.published.apply", Desc = "Apply to Posting" },

            // ===== APPLICANTS =====
            new() { MenuKey = "hr.recruit.applicant", Key = "hr.recruit.applicant.view", Desc = "Access Applicants" },
            new() { MenuKey = "hr.recruit.applicant", Key = "hr.recruit.applicant.manage", Desc = "Manage Applicants" },

            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.view", Desc = "View Applicants" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.review", Desc = "Review Applicant" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.shortlist", Desc = "Shortlist Applicant" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.reject", Desc = "Reject Applicant" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.hire", Desc = "Hire Applicant" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.export", Desc = "Export Applicants" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.import", Desc = "Import Applicants" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.assign", Desc = "Assign Applicant to Posting" },
            new() { MenuKey = "hr.recruit.applicant.list", Key = "hr.recruit.applicant.list.move", Desc = "Move Applicant Stage" },

            new() { MenuKey = "hr.recruit.applicant.evaluate", Key = "hr.recruit.applicant.evaluate.view", Desc = "View Evaluation" },
            new() { MenuKey = "hr.recruit.applicant.evaluate", Key = "hr.recruit.applicant.evaluate.score", Desc = "Score Applicant" },
            new() { MenuKey = "hr.recruit.applicant.evaluate", Key = "hr.recruit.applicant.evaluate.feedback", Desc = "Provide Feedback" },

            // ===== INTERVIEWS =====
            new() { MenuKey = "hr.recruit.interview", Key = "hr.recruit.interview.view", Desc = "Access Interviews" },
            new() { MenuKey = "hr.recruit.interview", Key = "hr.recruit.interview.manage", Desc = "Manage Interviews" },

            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.view", Desc = "View Interviews" },
            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.schedule", Desc = "Schedule Interview" },
            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.mod", Desc = "Edit Interview" },
            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.cancel", Desc = "Cancel Interview" },
            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.reschedule", Desc = "Reschedule Interview" },
            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.feedback", Desc = "Submit Interview Feedback" },
            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.score", Desc = "Score Interview" },
            new() { MenuKey = "hr.recruit.interview.list", Key = "hr.recruit.interview.list.export", Desc = "Export Interviews" },

            new() { MenuKey = "hr.recruit.interview.schedule", Key = "hr.recruit.interview.schedule.do", Desc = "Schedule New Interview" },
            new() { MenuKey = "hr.recruit.interview.schedule", Key = "hr.recruit.interview.schedule.template", Desc = "Interview Templates" },

            // ===== OFFERS =====
            new() { MenuKey = "hr.recruit.offer", Key = "hr.recruit.offer.view", Desc = "Access Offers" },
            new() { MenuKey = "hr.recruit.offer", Key = "hr.recruit.offer.manage", Desc = "Manage Offers" },

            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.view", Desc = "View Offers" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.create", Desc = "Create Offer" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.mod", Desc = "Edit Offer" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.del", Desc = "Delete Offer" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.send", Desc = "Send Offer" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.accept", Desc = "Accept Offer" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.reject", Desc = "Reject Offer" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.withdraw", Desc = "Withdraw Offer" },
            new() { MenuKey = "hr.recruit.offer.list", Key = "hr.recruit.offer.list.export", Desc = "Export Offers" },

            new() { MenuKey = "hr.recruit.offer.create", Key = "hr.recruit.offer.create.do", Desc = "Create New Offer" },
            new() { MenuKey = "hr.recruit.offer.create", Key = "hr.recruit.offer.create.template", Desc = "Offer Templates" },

            // ===== ONBOARDING =====
            new() { MenuKey = "hr.recruit.onboard", Key = "hr.recruit.onboard.view", Desc = "Access Onboarding" },
            new() { MenuKey = "hr.recruit.onboard", Key = "hr.recruit.onboard.manage", Desc = "Manage Onboarding" },

            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.view", Desc = "View Onboarding Tasks" },
            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.create", Desc = "Create Onboarding Task" },
            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.mod", Desc = "Edit Onboarding Task" },
            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.del", Desc = "Delete Onboarding Task" },
            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.assign", Desc = "Assign Task to Employee" },
            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.complete", Desc = "Complete Task" },
            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.verify", Desc = "Verify Task" },
            new() { MenuKey = "hr.recruit.onboard.tasks", Key = "hr.recruit.onboard.tasks.export", Desc = "Export Tasks" },

            new() { MenuKey = "hr.recruit.onboard.assignments", Key = "hr.recruit.onboard.assignments.view", Desc = "View Assignments" },
            new() { MenuKey = "hr.recruit.onboard.assignments", Key = "hr.recruit.onboard.assignments.create", Desc = "Create Assignment" },
            new() { MenuKey = "hr.recruit.onboard.assignments", Key = "hr.recruit.onboard.assignments.mod", Desc = "Edit Assignment" },
            new() { MenuKey = "hr.recruit.onboard.assignments", Key = "hr.recruit.onboard.assignments.del", Desc = "Delete Assignment" },
            new() { MenuKey = "hr.recruit.onboard.assignments", Key = "hr.recruit.onboard.assignments.complete", Desc = "Complete Assignment" },

            // ===== EVALUATION =====
            new() { MenuKey = "hr.recruit.evaluation", Key = "hr.recruit.evaluation.view", Desc = "Access Evaluation" },
            new() { MenuKey = "hr.recruit.evaluation", Key = "hr.recruit.evaluation.manage", Desc = "Manage Evaluation" },

            new() { MenuKey = "hr.recruit.evaluation.flow", Key = "hr.recruit.evaluation.flow.view", Desc = "View Evaluation Flows" },
            new() { MenuKey = "hr.recruit.evaluation.flow", Key = "hr.recruit.evaluation.flow.assign", Desc = "Assign Evaluation Flow" },
            new() { MenuKey = "hr.recruit.evaluation.flow", Key = "hr.recruit.evaluation.flow.start", Desc = "Start Evaluation" },
            new() { MenuKey = "hr.recruit.evaluation.flow", Key = "hr.recruit.evaluation.flow.submit", Desc = "Submit Evaluation" },
            new() { MenuKey = "hr.recruit.evaluation.flow", Key = "hr.recruit.evaluation.flow.export", Desc = "Export Evaluations" },
        };
    }
}