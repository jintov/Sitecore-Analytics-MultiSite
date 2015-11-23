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
    public class ProcessGoalsRules : IAnalyticsPipelineProcessor
    {
        public void Process(AnalyticsPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.ArgumentNotNull(args.RulesFolderID, "args.RulesFolderID");
            Assert.IsFalse(args.RulesFolderID.IsNull, "args.RulesFolderID.IsNull");

            GoalsPipelineArgs goalsArgs = args as GoalsPipelineArgs;

            goalsArgs.PageEvents = Tracker.DefinitionItems.AllPageEvents.Where(e => e.IsDeployed && !e.IsSystem && e.IsGoal);

            if (args.ContextItem == null)
                return;

            //Execute rules here to get the below goalsFolders
            AnalyticsRuleContext ruleContext = new AnalyticsRuleContext() { Item = args.ContextItem, RulesFolderID = args.RulesFolderID };
            RulesManager.RunRules(ruleContext);
            if (ruleContext.AnalyticsFolders != null && ruleContext.AnalyticsFolders.Count > 0)
            {
                var analyticsFolders = ruleContext.AnalyticsFolders.Select(e => e.Paths.FullPath).ToList();
                goalsArgs.PageEvents = goalsArgs.PageEvents.Where(e => analyticsFolders.Exists(gf => e.InnerItem.Paths.FullPath.StartsWith(gf)));
            }
        }
    }
}
