using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace Sitecore.Analytics.MultiSite.Rules
{
    public class AnalyticsRuleContext : RuleContext
    {
        public AnalyticsRuleContext()
        {
            this.AnalyticsFolders = new List<Item>();
        }

        private Item rulesFolder;

        public ID RulesFolderID { get; set; }

        public Item RulesFolder
        { 
            get
            {
                if (rulesFolder != null)
                    return rulesFolder;

                if (!RulesFolderID.IsNull && Item != null)
                {
                    rulesFolder = Sitecore.Context.ContentDatabase.GetItem(RulesFolderID);
                }

                return rulesFolder;
            }
        }

        public List<Item> AnalyticsFolders { get; set; }
    }
}
