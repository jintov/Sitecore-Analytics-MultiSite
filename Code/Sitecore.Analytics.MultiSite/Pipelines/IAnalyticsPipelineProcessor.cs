using Sitecore.Diagnostics;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Analytics.MultiSite.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Analytics.MultiSite.Pipelines
{
    public interface IAnalyticsPipelineProcessor
    {
        void Process(AnalyticsPipelineArgs args);
    }
}
