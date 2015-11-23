using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.SecurityModel;

namespace Sitecore.Analytics.MultiSite.Rules
{
    public class RulesManager
    {
        public static void RunRules(AnalyticsRuleContext ruleContext)
        {
            Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
            Assert.ArgumentNotNull((object)ruleContext.Item, "ruleContext.Item");
            Assert.ArgumentNotNull((object)ruleContext.RulesFolder, "ruleContext.RulesFolder");

            if (!Settings.Rules.ItemEventHandlers.RulesSupported(ruleContext.Item.Database))
                return;

            RuleList<AnalyticsRuleContext> rules = RuleFactory.GetRules<AnalyticsRuleContext>(ruleContext.RulesFolder, "Rule");
            if (rules == null)
                return;

            int count = 0;
            rules.Run(ruleContext, out count);
        }
    }
}
