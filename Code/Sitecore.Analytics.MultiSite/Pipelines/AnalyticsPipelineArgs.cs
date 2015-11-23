using Sitecore.Diagnostics;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Analytics.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Analytics.MultiSite.Pipelines
{
    public class AnalyticsPipelineArgs : PipelineArgs
    {
        public Item ContextItem { get; set; }

        public ID RulesFolderID { get; set; }
    }
}
