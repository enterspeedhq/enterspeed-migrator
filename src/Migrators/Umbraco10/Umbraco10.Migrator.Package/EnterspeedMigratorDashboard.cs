using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Dashboards;

namespace Umbraco10.Migrator.Package
{
    public class EnterspeedMigratorDashboard : IDashboard
    {
        public string Alias => "EnterspeedMigrator";
        public string View => "/App_Plugins/Umbraco10.Migrator.Package/dashboard/dashboard.html";

        public string[] Sections => new[]
        {
            Constants.Applications.Settings
        };

        public IAccessRule[] AccessRules
        {
            get
            {
                var rules = new IAccessRule[]
                {
                    new AccessRule {Type = AccessRuleType.Grant, Value = Constants.Security.AdminGroupAlias}
                };
                return rules;
            }
        }
    }
}