using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Permissions;

public static class CrmPermissionsSeeder
{
    public static IEnumerable<PerAccessSeedDto> GetPermissions()
    {
        return new List<PerAccessSeedDto>
        {
            // ============================================================
            // DASHBOARD
            // ============================================================
            new() { MenuKey = "crm.db", Key = "crm.db.view", Desc = "View CRM Dashboard" },
            new() { MenuKey = "crm.db", Key = "crm.db.export", Desc = "Export Dashboard Data" },

            // ============================================================
            // LEAD MANAGEMENT - Parent Menu
            // ============================================================
            new() { MenuKey = "crm.leads", Key = "crm.leads.view", Desc = "Access Lead Management" },
            new() { MenuKey = "crm.leads", Key = "crm.leads.manage", Desc = "Manage Leads" },

            // Lead Generation (All Leads)
            new() { MenuKey = "crm.leads.list", Key = "crm.leads.list.view", Desc = "View All Leads" },
            new() { MenuKey = "crm.leads.list", Key = "crm.leads.list.add", Desc = "Create Lead" },
            new() { MenuKey = "crm.leads.list", Key = "crm.leads.list.mod", Desc = "Edit Lead" },
            new() { MenuKey = "crm.leads.list", Key = "crm.leads.list.del", Desc = "Delete Lead" },
            new() { MenuKey = "crm.leads.list", Key = "crm.leads.list.export", Desc = "Export Leads" },
            new() { MenuKey = "crm.leads.list", Key = "crm.leads.list.import", Desc = "Import Leads" },
            new() { MenuKey = "crm.leads.list", Key = "crm.leads.list.assign", Desc = "Assign Lead" },

            // Lead Generation
            new() { MenuKey = "crm.leads.generation", Key = "crm.leads.generation.view", Desc = "View Lead Generation" },
            new() { MenuKey = "crm.leads.generation", Key = "crm.leads.generation.add", Desc = "Create Lead" },
            new() { MenuKey = "crm.leads.generation", Key = "crm.leads.generation.source", Desc = "Manage Lead Sources" },
            new() { MenuKey = "crm.leads.generation", Key = "crm.leads.generation.import", Desc = "Import Leads" },

            // Lead Qualification
            new() { MenuKey = "crm.leads.qualification", Key = "crm.leads.qualification.view", Desc = "View Qualification" },
            new() { MenuKey = "crm.leads.qualification", Key = "crm.leads.qualification.qualify", Desc = "Qualify Lead" },
            new() { MenuKey = "crm.leads.qualification", Key = "crm.leads.qualification.disqualify", Desc = "Disqualify Lead" },
            new() { MenuKey = "crm.leads.qualification", Key = "crm.leads.qualification.score", Desc = "Score Lead" },
            new() { MenuKey = "crm.leads.qualification", Key = "crm.leads.qualification.criteria", Desc = "Set Qualification Criteria" },

            // Lead Conversion
            new() { MenuKey = "crm.leads.conversion", Key = "crm.leads.conversion.view", Desc = "View Conversion" },
            new() { MenuKey = "crm.leads.conversion", Key = "crm.leads.conversion.convert", Desc = "Convert Lead to Customer" },
            new() { MenuKey = "crm.leads.conversion", Key = "crm.leads.conversion.opportunity", Desc = "Convert to Opportunity" },
            new() { MenuKey = "crm.leads.conversion", Key = "crm.leads.conversion.history", Desc = "View Conversion History" },

            // Assigned Leads
            new() { MenuKey = "crm.leads.assigned", Key = "crm.leads.assigned.view", Desc = "View Assigned Leads" },
            new() { MenuKey = "crm.leads.assigned", Key = "crm.leads.assigned.update", Desc = "Update Lead Status" },
            new() { MenuKey = "crm.leads.assigned", Key = "crm.leads.assigned.reassign", Desc = "Reassign Lead" },

            // Lead Routing
            new() { MenuKey = "crm.leads.routing", Key = "crm.leads.routing.view", Desc = "View Routing Rules" },
            new() { MenuKey = "crm.leads.routing", Key = "crm.leads.routing.add", Desc = "Create Routing Rule" },
            new() { MenuKey = "crm.leads.routing", Key = "crm.leads.routing.mod", Desc = "Edit Routing Rule" },
            new() { MenuKey = "crm.leads.routing", Key = "crm.leads.routing.del", Desc = "Delete Routing Rule" },
            new() { MenuKey = "crm.leads.routing", Key = "crm.leads.routing.activate", Desc = "Activate/Deactivate Rule" },

            // Bulk Actions
            new() { MenuKey = "crm.leads.bulk", Key = "crm.leads.bulk.view", Desc = "View Bulk Actions" },
            new() { MenuKey = "crm.leads.bulk", Key = "crm.leads.bulk.update", Desc = "Bulk Update Leads" },
            new() { MenuKey = "crm.leads.bulk", Key = "crm.leads.bulk.delete", Desc = "Bulk Delete Leads" },
            new() { MenuKey = "crm.leads.bulk", Key = "crm.leads.bulk.assign", Desc = "Bulk Assign Leads" },
            new() { MenuKey = "crm.leads.bulk", Key = "crm.leads.bulk.export", Desc = "Bulk Export Leads" },

            // Import Leads
            new() { MenuKey = "crm.leads.import", Key = "crm.leads.import.view", Desc = "View Import" },
            new() { MenuKey = "crm.leads.import", Key = "crm.leads.import.upload", Desc = "Upload Leads" },
            new() { MenuKey = "crm.leads.import", Key = "crm.leads.import.template", Desc = "Download Template" },
            new() { MenuKey = "crm.leads.import", Key = "crm.leads.import.history", Desc = "View Import History" },

            // Lead Grouping
            new() { MenuKey = "crm.leads.grouping", Key = "crm.leads.grouping.view", Desc = "View Groups" },
            new() { MenuKey = "crm.leads.grouping", Key = "crm.leads.grouping.add", Desc = "Create Group" },
            new() { MenuKey = "crm.leads.grouping", Key = "crm.leads.grouping.mod", Desc = "Edit Group" },
            new() { MenuKey = "crm.leads.grouping", Key = "crm.leads.grouping.del", Desc = "Delete Group" },
            new() { MenuKey = "crm.leads.grouping", Key = "crm.leads.grouping.assign", Desc = "Assign Leads to Group" },

            // ============================================================
            // CONTACT MANAGEMENT - Parent Menu
            // ============================================================
            new() { MenuKey = "crm.contacts", Key = "crm.contacts.view", Desc = "Access Contact Management" },
            new() { MenuKey = "crm.contacts", Key = "crm.contacts.manage", Desc = "Manage Contacts" },

            // Contacts List
            new() { MenuKey = "crm.contacts.list", Key = "crm.contacts.list.view", Desc = "View Contacts" },
            new() { MenuKey = "crm.contacts.list", Key = "crm.contacts.list.add", Desc = "Add Contact" },
            new() { MenuKey = "crm.contacts.list", Key = "crm.contacts.list.mod", Desc = "Edit Contact" },
            new() { MenuKey = "crm.contacts.list", Key = "crm.contacts.list.del", Desc = "Delete Contact" },
            new() { MenuKey = "crm.contacts.list", Key = "crm.contacts.list.export", Desc = "Export Contacts" },
            new() { MenuKey = "crm.contacts.list", Key = "crm.contacts.list.import", Desc = "Import Contacts" },
            new() { MenuKey = "crm.contacts.list", Key = "crm.contacts.list.merge", Desc = "Merge Contacts" },

            // Assigned Contacts
            new() { MenuKey = "crm.contacts.assigned", Key = "crm.contacts.assigned.view", Desc = "View Assigned Contacts" },
            new() { MenuKey = "crm.contacts.assigned", Key = "crm.contacts.assigned.update", Desc = "Update Contact" },
            new() { MenuKey = "crm.contacts.assigned", Key = "crm.contacts.assigned.reassign", Desc = "Reassign Contact" },

            // Contact Grouping
            new() { MenuKey = "crm.contacts.grouping", Key = "crm.contacts.grouping.view", Desc = "View Contact Groups" },
            new() { MenuKey = "crm.contacts.grouping", Key = "crm.contacts.grouping.add", Desc = "Create Group" },
            new() { MenuKey = "crm.contacts.grouping", Key = "crm.contacts.grouping.mod", Desc = "Edit Group" },
            new() { MenuKey = "crm.contacts.grouping", Key = "crm.contacts.grouping.del", Desc = "Delete Group" },
            new() { MenuKey = "crm.contacts.grouping", Key = "crm.contacts.grouping.assign", Desc = "Assign Contacts to Group" },

            // Companies
            new() { MenuKey = "crm.contacts.companies", Key = "crm.contacts.companies.view", Desc = "View Companies" },
            new() { MenuKey = "crm.contacts.companies", Key = "crm.contacts.companies.add", Desc = "Add Company" },
            new() { MenuKey = "crm.contacts.companies", Key = "crm.contacts.companies.mod", Desc = "Edit Company" },
            new() { MenuKey = "crm.contacts.companies", Key = "crm.contacts.companies.del", Desc = "Delete Company" },
            new() { MenuKey = "crm.contacts.companies", Key = "crm.contacts.companies.export", Desc = "Export Companies" },

            // Interactions
            new() { MenuKey = "crm.contacts.interactions", Key = "crm.contacts.interactions.view", Desc = "View Interactions" },
            new() { MenuKey = "crm.contacts.interactions", Key = "crm.contacts.interactions.add", Desc = "Log Interaction" },
            new() { MenuKey = "crm.contacts.interactions", Key = "crm.contacts.interactions.mod", Desc = "Edit Interaction" },
            new() { MenuKey = "crm.contacts.interactions", Key = "crm.contacts.interactions.del", Desc = "Delete Interaction" },
            new() { MenuKey = "crm.contacts.interactions", Key = "crm.contacts.interactions.export", Desc = "Export Interactions" },
            new() { MenuKey = "crm.contacts.interactions", Key = "crm.contacts.interactions.template", Desc = "Interaction Templates" },

            // ============================================================
            // SALES MANAGEMENT - Parent Menu
            // ============================================================
            new() { MenuKey = "crm.sales", Key = "crm.sales.view", Desc = "Access Sales Management" },
            new() { MenuKey = "crm.sales", Key = "crm.sales.manage", Desc = "Manage Sales" },

            // Sales Dashboard
            new() { MenuKey = "crm.sales.dashboard", Key = "crm.sales.dashboard.view", Desc = "View Sales Dashboard" },
            new() { MenuKey = "crm.sales.dashboard", Key = "crm.sales.dashboard.export", Desc = "Export Dashboard" },

            // Opportunities
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.view", Desc = "View Opportunities" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.add", Desc = "Create Opportunity" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.mod", Desc = "Edit Opportunity" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.del", Desc = "Delete Opportunity" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.won", Desc = "Mark as Won" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.lost", Desc = "Mark as Lost" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.stage", Desc = "Update Stage" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.export", Desc = "Export Opportunities" },
            new() { MenuKey = "crm.sales.opportunities", Key = "crm.sales.opportunities.pipeline", Desc = "View Pipeline" },

            // Quotes
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.view", Desc = "View Quotes" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.add", Desc = "Create Quote" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.mod", Desc = "Edit Quote" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.del", Desc = "Delete Quote" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.approve", Desc = "Approve Quote" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.reject", Desc = "Reject Quote" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.send", Desc = "Send Quote" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.print", Desc = "Print Quote" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.convert", Desc = "Convert to Order" },
            new() { MenuKey = "crm.sales.quotes", Key = "crm.sales.quotes.template", Desc = "Quote Templates" },

            // Sales Orders
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.view", Desc = "View Sales Orders" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.add", Desc = "Create Sales Order" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.mod", Desc = "Edit Sales Order" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.del", Desc = "Delete Sales Order" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.approve", Desc = "Approve Sales Order" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.fulfill", Desc = "Fulfill Order" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.ship", Desc = "Ship Order" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.export", Desc = "Export Orders" },
            new() { MenuKey = "crm.sales.orders", Key = "crm.sales.orders.status", Desc = "Update Status" },

            // Contracts
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.view", Desc = "View Contracts" },
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.add", Desc = "Create Contract" },
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.mod", Desc = "Edit Contract" },
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.del", Desc = "Delete Contract" },
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.renew", Desc = "Renew Contract" },
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.terminate", Desc = "Terminate Contract" },
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.sign", Desc = "Digital Sign Contract" },
            new() { MenuKey = "crm.sales.contracts", Key = "crm.sales.contracts.template", Desc = "Contract Templates" },

            // Sales Forecast
            new() { MenuKey = "crm.sales.forecast", Key = "crm.sales.forecast.view", Desc = "View Sales Forecast" },
            new() { MenuKey = "crm.sales.forecast", Key = "crm.sales.forecast.generate", Desc = "Generate Forecast" },
            new() { MenuKey = "crm.sales.forecast", Key = "crm.sales.forecast.export", Desc = "Export Forecast" },
            new() { MenuKey = "crm.sales.forecast", Key = "crm.sales.forecast.compare", Desc = "Compare with Actuals" },

            // ============================================================
            // MARKETING - Parent Menu
            // ============================================================
            new() { MenuKey = "crm.marketing", Key = "crm.marketing.view", Desc = "Access Marketing" },
            new() { MenuKey = "crm.marketing", Key = "crm.marketing.manage", Desc = "Manage Marketing" },

            // Marketing Dashboard
            new() { MenuKey = "crm.marketing.dashboard", Key = "crm.marketing.dashboard.view", Desc = "View Marketing Dashboard" },
            new() { MenuKey = "crm.marketing.dashboard", Key = "crm.marketing.dashboard.export", Desc = "Export Marketing Dashboard" },

            // Campaigns
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.view", Desc = "View Campaigns" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.add", Desc = "Create Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.mod", Desc = "Edit Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.del", Desc = "Delete Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.start", Desc = "Start Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.pause", Desc = "Pause Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.resume", Desc = "Resume Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.cancel", Desc = "Cancel Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.archive", Desc = "Archive Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.duplicate", Desc = "Duplicate Campaign" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.analytics", Desc = "Campaign Analytics" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.roi", Desc = "ROI Analysis" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.budget", Desc = "Manage Budget" },
            new() { MenuKey = "crm.marketing.campaigns", Key = "crm.marketing.campaigns.export", Desc = "Export Campaigns" },

            // Email Campaigns
            new() { MenuKey = "crm.marketing.email", Key = "crm.marketing.email.view", Desc = "View Email Campaigns" },
            new() { MenuKey = "crm.marketing.email", Key = "crm.marketing.email.create", Desc = "Create Email Campaign" },
            new() { MenuKey = "crm.marketing.email", Key = "crm.marketing.email.send", Desc = "Send Emails" },
            new() { MenuKey = "crm.marketing.email", Key = "crm.marketing.email.template", Desc = "Email Templates" },
            new() { MenuKey = "crm.marketing.email", Key = "crm.marketing.email.track", Desc = "Track Opens/Clicks" },
            new() { MenuKey = "crm.marketing.email", Key = "crm.marketing.email.analytics", Desc = "Email Analytics" },
            new() { MenuKey = "crm.marketing.email", Key = "crm.marketing.email.list", Desc = "Manage Email Lists" },

            // SMS Campaigns
            new() { MenuKey = "crm.marketing.sms", Key = "crm.marketing.sms.view", Desc = "View SMS Campaigns" },
            new() { MenuKey = "crm.marketing.sms", Key = "crm.marketing.sms.create", Desc = "Create SMS Campaign" },
            new() { MenuKey = "crm.marketing.sms", Key = "crm.marketing.sms.send", Desc = "Send SMS" },
            new() { MenuKey = "crm.marketing.sms", Key = "crm.marketing.sms.template", Desc = "SMS Templates" },
            new() { MenuKey = "crm.marketing.sms", Key = "crm.marketing.sms.analytics", Desc = "SMS Analytics" },
            new() { MenuKey = "crm.marketing.sms", Key = "crm.marketing.sms.list", Desc = "Manage SMS Lists" },

            // Add Campaign (child of Marketing)
            new() { MenuKey = "crm.marketing.add", Key = "crm.marketing.add.view", Desc = "View Add Campaign" },
            new() { MenuKey = "crm.marketing.add", Key = "crm.marketing.add.create", Desc = "Create New Campaign" },

            // ============================================================
            // CUSTOMER SUPPORT - Parent Menu
            // ============================================================
            new() { MenuKey = "crm.support", Key = "crm.support.view", Desc = "Access Customer Support" },
            new() { MenuKey = "crm.support", Key = "crm.support.manage", Desc = "Manage Support" },

            // Support Dashboard
            new() { MenuKey = "crm.support.dashboard", Key = "crm.support.dashboard.view", Desc = "View Support Dashboard" },
            new() { MenuKey = "crm.support.dashboard", Key = "crm.support.dashboard.export", Desc = "Export Support Dashboard" },

            // Tickets
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.view", Desc = "View Tickets" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.add", Desc = "Create Ticket" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.mod", Desc = "Edit Ticket" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.del", Desc = "Delete Ticket" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.assign", Desc = "Assign Ticket" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.resolve", Desc = "Resolve Ticket" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.escalate", Desc = "Escalate Ticket" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.reopen", Desc = "Reopen Ticket" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.export", Desc = "Export Tickets" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.priority", Desc = "Set Priority" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.category", Desc = "Manage Categories" },
            new() { MenuKey = "crm.support.tickets", Key = "crm.support.tickets.satisfaction", Desc = "Satisfaction Survey" },

            // Knowledge Base
            new() { MenuKey = "crm.support.knowledge", Key = "crm.support.knowledge.view", Desc = "View Knowledge Base" },
            new() { MenuKey = "crm.support.knowledge", Key = "crm.support.knowledge.add", Desc = "Add Article" },
            new() { MenuKey = "crm.support.knowledge", Key = "crm.support.knowledge.mod", Desc = "Edit Article" },
            new() { MenuKey = "crm.support.knowledge", Key = "crm.support.knowledge.del", Desc = "Delete Article" },
            new() { MenuKey = "crm.support.knowledge", Key = "crm.support.knowledge.publish", Desc = "Publish Article" },
            new() { MenuKey = "crm.support.knowledge", Key = "crm.support.knowledge.category", Desc = "Manage Categories" },
            new() { MenuKey = "crm.support.knowledge", Key = "crm.support.knowledge.search", Desc = "Search Articles" },

            // Customer Feedback
            new() { MenuKey = "crm.support.feedback", Key = "crm.support.feedback.view", Desc = "View Feedback" },
            new() { MenuKey = "crm.support.feedback", Key = "crm.support.feedback.respond", Desc = "Respond to Feedback" },
            new() { MenuKey = "crm.support.feedback", Key = "crm.support.feedback.export", Desc = "Export Feedback" },
            new() { MenuKey = "crm.support.feedback", Key = "crm.support.feedback.analytics", Desc = "Feedback Analytics" },

            // ============================================================
            // ACTIVITIES
            // ============================================================
            new() { MenuKey = "crm.activities", Key = "crm.activities.view", Desc = "Access Activities" },
            new() { MenuKey = "crm.activities", Key = "crm.activities.manage", Desc = "Manage Activities" },

            // Activities List
            new() { MenuKey = "crm.activities.list", Key = "crm.activities.list.view", Desc = "View Activities" },
            new() { MenuKey = "crm.activities.list", Key = "crm.activities.list.add", Desc = "Add Activity" },
            new() { MenuKey = "crm.activities.list", Key = "crm.activities.list.mod", Desc = "Edit Activity" },
            new() { MenuKey = "crm.activities.list", Key = "crm.activities.list.del", Desc = "Delete Activity" },
            new() { MenuKey = "crm.activities.list", Key = "crm.activities.list.complete", Desc = "Complete Activity" },
            new() { MenuKey = "crm.activities.list", Key = "crm.activities.list.export", Desc = "Export Activities" },

            // Calendar
            new() { MenuKey = "crm.activities.calendar", Key = "crm.activities.calendar.view", Desc = "View Calendar" },
            new() { MenuKey = "crm.activities.calendar", Key = "crm.activities.calendar.add", Desc = "Add Event" },
            new() { MenuKey = "crm.activities.calendar", Key = "crm.activities.calendar.mod", Desc = "Edit Event" },
            new() { MenuKey = "crm.activities.calendar", Key = "crm.activities.calendar.del", Desc = "Delete Event" },
            new() { MenuKey = "crm.activities.calendar", Key = "crm.activities.calendar.sync", Desc = "Sync Calendar" },
            new() { MenuKey = "crm.activities.calendar", Key = "crm.activities.calendar.export", Desc = "Export Calendar" },

            // ============================================================
            // ANALYTICS - Parent Menu
            // ============================================================
            new() { MenuKey = "crm.analytics", Key = "crm.analytics.view", Desc = "Access CRM Analytics" },
            new() { MenuKey = "crm.analytics", Key = "crm.analytics.manage", Desc = "Manage Analytics" },

            // Analytics Dashboard
            new() { MenuKey = "crm.analytics.dashboard", Key = "crm.analytics.dashboard.view", Desc = "View Analytics Dashboard" },
            new() { MenuKey = "crm.analytics.dashboard", Key = "crm.analytics.dashboard.export", Desc = "Export Analytics Dashboard" },

            // Sales Analytics
            new() { MenuKey = "crm.analytics.sales", Key = "crm.analytics.sales.view", Desc = "View Sales Analytics" },
            new() { MenuKey = "crm.analytics.sales", Key = "crm.analytics.sales.export", Desc = "Export Sales Analytics" },
            new() { MenuKey = "crm.analytics.sales", Key = "crm.analytics.sales.revenue", Desc = "Revenue Analysis" },
            new() { MenuKey = "crm.analytics.sales", Key = "crm.analytics.sales.pipeline", Desc = "Pipeline Analysis" },
            new() { MenuKey = "crm.analytics.sales", Key = "crm.analytics.sales.forecast", Desc = "Forecast Analysis" },

            // Customer Analytics
            new() { MenuKey = "crm.analytics.customer", Key = "crm.analytics.customer.view", Desc = "View Customer Analytics" },
            new() { MenuKey = "crm.analytics.customer", Key = "crm.analytics.customer.export", Desc = "Export Customer Analytics" },
            new() { MenuKey = "crm.analytics.customer", Key = "crm.analytics.customer.lifetime", Desc = "Lifetime Value" },
            new() { MenuKey = "crm.analytics.customer", Key = "crm.analytics.customer.churn", Desc = "Churn Analysis" },
            new() { MenuKey = "crm.analytics.customer", Key = "crm.analytics.customer.segmentation", Desc = "Customer Segmentation" },
            new() { MenuKey = "crm.analytics.customer", Key = "crm.analytics.customer.satisfaction", Desc = "Customer Satisfaction" },

            // Marketing Analytics
            new() { MenuKey = "crm.analytics.marketing", Key = "crm.analytics.marketing.view", Desc = "View Marketing Analytics" },
            new() { MenuKey = "crm.analytics.marketing", Key = "crm.analytics.marketing.export", Desc = "Export Marketing Analytics" },
            new() { MenuKey = "crm.analytics.marketing", Key = "crm.analytics.marketing.roi", Desc = "Campaign ROI" },
            new() { MenuKey = "crm.analytics.marketing", Key = "crm.analytics.marketing.conversion", Desc = "Conversion Analysis" },
            new() { MenuKey = "crm.analytics.marketing", Key = "crm.analytics.marketing.engagement", Desc = "Engagement Analysis" },
            new() { MenuKey = "crm.analytics.marketing", Key = "crm.analytics.marketing.channel", Desc = "Channel Performance" },
        };
    }
}