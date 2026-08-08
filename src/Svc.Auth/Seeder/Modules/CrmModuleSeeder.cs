using Svc.Auth.Models.Dtos;

namespace Svc.Auth.Seeder.Modules;

public static class CrmModuleSeeder
{
    public static IEnumerable<PerMenuSeedDto> GetMenus()
    {
        return new List<PerMenuSeedDto>
        {
            // ============================================================
            // DASHBOARD
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.db",
                Label = "Dashboard",
                Path = "/crm",
                Icon = "LayoutDashboard",
                ParKey = "",
                IsChild = false,
                Order = 1
            },

            // ============================================================
            // LEAD MANAGEMENT
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads",
                Label = "Lead Management",
                Path = "",
                Icon = "Users",
                ParKey = "",
                IsChild = false,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.list",
                Label = "All Leads",
                Path = "/crm/leads",
                Icon = "Users",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.generation",
                Label = "Lead Generation",
                Path = "/crm/leads/generation",
                Icon = "UserPlus",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.qualification",
                Label = "Lead Qualification",
                Path = "/crm/leads/qualification",
                Icon = "CheckCircle",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.conversion",
                Label = "Lead Conversion",
                Path = "/crm/leads/conversion",
                Icon = "UserPlus",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.assigned",
                Label = "Assigned Leads",
                Path = "/crm/leads/assigned",
                Icon = "UserCheck",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 5
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.routing",
                Label = "Lead Routing",
                Path = "/crm/leads/routing",
                Icon = "GitBranch",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 6
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.bulk",
                Label = "Bulk Actions",
                Path = "/crm/leads/bulk-action",
                Icon = "Layers",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 7
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.import",
                Label = "Import Leads",
                Path = "/crm/leads/import",
                Icon = "Upload",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 8
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.leads.grouping",
                Label = "Lead Grouping",
                Path = "/crm/leads/grouping",
                Icon = "Layers",
                ParKey = "crm.leads",
                IsChild = true,
                Order = 9
            },

            // ============================================================
            // CONTACT MANAGEMENT
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.contacts",
                Label = "Contact Management",
                Path = "",
                Icon = "Mail",
                ParKey = "",
                IsChild = false,
                Order = 3
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.contacts.list",
                Label = "All Contacts",
                Path = "/crm/contacts",
                Icon = "Users",
                ParKey = "crm.contacts",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.contacts.assigned",
                Label = "Assigned Contacts",
                Path = "/crm/contacts/assigned",
                Icon = "UserCheck",
                ParKey = "crm.contacts",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.contacts.grouping",
                Label = "Contact Grouping",
                Path = "/crm/contacts/grouping",
                Icon = "Layers",
                ParKey = "crm.contacts",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.contacts.companies",
                Label = "Companies",
                Path = "/crm/companies",
                Icon = "Building",
                ParKey = "crm.contacts",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.contacts.interactions",
                Label = "Interactions",
                Path = "/crm/interactions",
                Icon = "MessageSquare",
                ParKey = "crm.contacts",
                IsChild = true,
                Order = 5
            },

            // ============================================================
            // SALES MANAGEMENT
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.sales",
                Label = "Sales Management",
                Path = "",
                Icon = "BarChart3",
                ParKey = "",
                IsChild = false,
                Order = 4
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.sales.dashboard",
                Label = "Sales Dashboard",
                Path = "/crm/sales",
                Icon = "BarChart3",
                ParKey = "crm.sales",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.sales.opportunities",
                Label = "Opportunities",
                Path = "/crm/sales/opportunities",
                Icon = "Target",
                ParKey = "crm.sales",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.sales.quotes",
                Label = "Quotes",
                Path = "/crm/sales/quotes",
                Icon = "FileText",
                ParKey = "crm.sales",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.sales.orders",
                Label = "Sales Orders",
                Path = "/crm/sales/orders",
                Icon = "ShoppingCart",
                ParKey = "crm.sales",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.sales.contracts",
                Label = "Contracts",
                Path = "/crm/sales/contracts",
                Icon = "FileCheck",
                ParKey = "crm.sales",
                IsChild = true,
                Order = 5
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.sales.forecast",
                Label = "Sales Forecast",
                Path = "/crm/sales/forecast",
                Icon = "TrendingUp",
                ParKey = "crm.sales",
                IsChild = true,
                Order = 6
            },

            // ============================================================
            // REAL ESTATE - NEW SECTION
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.realestate",
                Label = "Real Estate",
                Path = "",
                Icon = "Building2",
                ParKey = "",
                IsChild = false,
                Order = 5
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.realestate.properties",
                Label = "Properties",
                Path = "/crm/realestate/properties",
                Icon = "Home",
                ParKey = "crm.realestate",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.realestate.transactions",
                Label = "Transactions",
                Path = "/crm/realestate/transactions",
                Icon = "FileText",
                ParKey = "crm.realestate",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.realestate.commissions",
                Label = "Commissions",
                Path = "/crm/realestate/commissions",
                Icon = "DollarSign",
                ParKey = "crm.realestate",
                IsChild = true,
                Order = 3
            },

            // ============================================================
            // MARKETING - Updated Order (moved to 6)
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing",
                Label = "Marketing",
                Path = "",
                Icon = "MessageSquare",
                ParKey = "",
                IsChild = false,
                Order = 6
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing.dashboard",
                Label = "Marketing Dashboard",
                Path = "/crm/marketing",
                Icon = "MessageSquare",
                ParKey = "crm.marketing",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing.campaigns",
                Label = "Campaigns",
                Path = "",
                Icon = "Megaphone",
                ParKey = "crm.marketing",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing.campaigns.all",
                Label = "All Campaigns",
                Path = "/crm/campaigns",
                Icon = "Megaphone",
                ParKey = "crm.marketing.campaigns",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing.email",
                Label = "Email Marketing",
                Path = "/crm/campaigns/email",
                Icon = "Mail",
                ParKey = "crm.marketing.campaigns",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing.sms",
                Label = "SMS Campaigns",
                Path = "/crm/campaigns/sms",
                Icon = "MessageSquare",
                ParKey = "crm.marketing.campaigns",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing.social",
                Label = "Social Media",
                Path = "/crm/social",
                Icon = "Share2",
                ParKey = "crm.marketing.campaigns",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.marketing.campaigns.add",
                Label = "Add Campaign",
                Path = "/crm/campaigns/add",
                Icon = "Plus",
                ParKey = "crm.marketing.campaigns",
                IsChild = true,
                Order = 99
            },

            // ============================================================
            // CUSTOMER SUPPORT - Updated Order (moved to 7)
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.support",
                Label = "Customer Support",
                Path = "",
                Icon = "Phone",
                ParKey = "",
                IsChild = false,
                Order = 7
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.support.dashboard",
                Label = "Support Dashboard",
                Path = "/crm/support",
                Icon = "Phone",
                ParKey = "crm.support",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.support.tickets",
                Label = "Support Tickets",
                Path = "/crm/support/tickets",
                Icon = "Ticket",
                ParKey = "crm.support",
                IsChild = true,
                Order = 2
            },

            // ============================================================
            // ACTIVITIES - Updated Order (moved to 8)
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.activities",
                Label = "Activities",
                Path = "",
                Icon = "Calendar",
                ParKey = "",
                IsChild = false,
                Order = 8
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.activities.list",
                Label = "Activities",
                Path = "/crm/activities",
                Icon = "Calendar",
                ParKey = "crm.activities",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.activities.calendar",
                Label = "Calendar",
                Path = "/crm/activities/calendar",
                Icon = "Calendar",
                ParKey = "crm.activities",
                IsChild = true,
                Order = 2
            },

            // ============================================================
            // ANALYTICS - Updated Order (moved to 9)
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.analytics",
                Label = "Analytics",
                Path = "",
                Icon = "BarChart3",
                ParKey = "",
                IsChild = false,
                Order = 9
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.analytics.dashboard",
                Label = "Analytics Dashboard",
                Path = "/crm/analytics",
                Icon = "BarChart3",
                ParKey = "crm.analytics",
                IsChild = true,
                Order = 1
            },

            // ============================================================
            // SETTINGS - Updated Order (moved to 10)
            // ============================================================
            new() {
                ModKey = "mod.crm",
                Key = "crm.settings",
                Label = "Settings",
                Path = "",
                Icon = "Settings",
                ParKey = "",
                IsChild = false,
                Order = 10
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.settings.crm",
                Label = "CRM Settings",
                Path = "/settings/crm",
                Icon = "Settings",
                ParKey = "crm.settings",
                IsChild = true,
                Order = 1
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.settings.sources",
                Label = "Lead Sources",
                Path = "/settings/crm/lead-sources",
                Icon = "List",
                ParKey = "crm.settings",
                IsChild = true,
                Order = 2
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.settings.statuses",
                Label = "Lead Statuses",
                Path = "/settings/crm/lead-statuses",
                Icon = "List",
                ParKey = "crm.settings",
                IsChild = true,
                Order = 3
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.settings.routing",
                Label = "Routing Rules",
                Path = "/settings/crm/routing-rules",
                Icon = "GitBranch",
                ParKey = "crm.settings",
                IsChild = true,
                Order = 4
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.settings.scoring",
                Label = "Lead Scoring",
                Path = "/settings/crm/lead-scoring",
                Icon = "Star",
                ParKey = "crm.settings",
                IsChild = true,
                Order = 5
            },
            new() {
                ModKey = "mod.crm",
                Key = "crm.settings.templates",
                Label = "Email Templates",
                Path = "/settings/crm/email-templates",
                Icon = "Mail",
                ParKey = "crm.settings",
                IsChild = true,
                Order = 6
            },
        };
    }
}