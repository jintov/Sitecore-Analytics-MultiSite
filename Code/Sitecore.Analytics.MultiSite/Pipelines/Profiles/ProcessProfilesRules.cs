using Sitecore.Diagnostics;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Analytics.MultiSite.Rules;
using Sitecore.Analytics.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Analytics.MultiSite.Pipelines
{
    public class ProcessProfilesRules : IAnalyticsPipelineProcessor
    {
        public void Process(AnalyticsPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.RulesFolderID, "args.RulesFolderID");
            Assert.IsFalse(args.RulesFolderID.IsNull, "args.RulesFolderID.IsNull");

            ProfilesPipelineArgs profilesArgs = args as ProfilesPipelineArgs;

            if (args.ContextItem == null)
                return;

            //Execute rules here to get the below goalsFolders
            AnalyticsRuleContext ruleContext = new AnalyticsRuleContext() { Item = args.ContextItem, RulesFolderID = args.RulesFolderID };
            RulesManager.RunRules(ruleContext);
            if (ruleContext.AnalyticsFolders == null || ruleContext.AnalyticsFolders.Count == 0)
                ruleContext.AnalyticsFolders.Add(GetDefaultProfilesRoot(profilesArgs.ContextItem));

            List<ProfileItem> profiles = new List<ProfileItem>();
            foreach (Item profilesRoot in ruleContext.AnalyticsFolders)
            {
                profiles.AddRange(profilesRoot.Database.Analytics(profilesRoot.Language).Profiles
                                        .Where(p => ((Item) p).TemplateID == AnalyticsIds.Profile)
                                        .Where(p => ((Item)p).Paths.FullPath.StartsWith(profilesRoot.Paths.FullPath, StringComparison.OrdinalIgnoreCase)));
            }
            profilesArgs.Profiles = profiles;
        }

        private Item GetDefaultProfilesRoot(Item contextItem)
        {
            Assert.ArgumentNotNull((object)contextItem, "contextItem");

            Database database = contextItem.Database;
            Assert.IsNotNull((object)database, "database");
            
            return database.GetItem(Sitecore.ItemIDs.Analytics.Profiles);
        }
    }
}
