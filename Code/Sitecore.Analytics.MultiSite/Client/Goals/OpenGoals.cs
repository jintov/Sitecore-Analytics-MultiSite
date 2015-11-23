using Sitecore;
using Sitecore.Shell.Applications.Analytics.TrackingField;
using System;

namespace Sitecore.Analytics.MultiSite.Client
{
    [Serializable, UsedImplicitly]
    public class OpenGoals : OpenTrackingField
    {
        // Methods
        protected override string GetUrl()
        {
            return "/sitecore/shell/~/xaml/Sitecore.Shell.Applications.Analytics.TrackingField.Goals.aspx";
        }
    }
}