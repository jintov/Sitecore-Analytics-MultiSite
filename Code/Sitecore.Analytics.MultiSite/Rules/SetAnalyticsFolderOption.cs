using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Actions;

namespace Sitecore.Analytics.MultiSite.Rules
{
    public class SetAnalyticsFolderOption<T> : RuleAction<T> where T : AnalyticsRuleContext
    {
        private ID analyticsFolderOptionID;

        public ID AnalyticsFolderOptionID
        {
            get
            {
                return this.analyticsFolderOptionID ?? ID.Null;
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.analyticsFolderOptionID = value;
            }
        }

        public override void Apply(T ruleContext)
        {
            Log.Error("Inside Apply", this);
            Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
            
            Item item = ruleContext.Item;
            if (item == null)
                return;
            
            Item analyticsFolder = Sitecore.Context.ContentDatabase.GetItem(this.analyticsFolderOptionID);
            if (analyticsFolder == null)
                return;
            
            ruleContext.AnalyticsFolders.Add(analyticsFolder);
        }
    }
}
