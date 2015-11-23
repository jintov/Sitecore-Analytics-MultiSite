using Sitecore;
using Sitecore.Analytics;
using Sitecore.Analytics.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Extensions.XElementExtensions;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
using Sitecore.Shell.Applications.Analytics.TrackingField;
using Sitecore.Analytics.MultiSite.Client;
using Sitecore.Analytics.MultiSite.Pipelines;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Sitecore.Analytics.MultiSite.Client
{
    /// <summary>
    /// Defines the goals page class.
    /// </summary>
    [UsedImplicitly]
    public class GoalsPage : Sitecore.Shell.Applications.Analytics.TrackingField.GoalsPage
    {
        /// <summary>
        /// Renders the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        protected override void Render(XDocument doc)
        {
            Assert.ArgumentNotNull((object)doc, "doc");
            this.RenderGoals(doc);
        }

        /// <summary>
        /// Renders the goals.
        /// </summary>
        /// <param name="doc">The document.</param>
        private void RenderGoals(XDocument doc)
        {
            Assert.ArgumentNotNull((object)doc, "doc");
            
            List<string> selected = new List<string>();
            foreach (XElement element in doc.Descendants((XName)"event"))
                selected.Add(XElementExtensions.GetAttributeValue(element, "name"));
            
            CheckBoxList checkBoxList = new CheckBoxList() { ID = "GoalsCheckBoxList" };
            this.GoalsList.Controls.Add((System.Web.UI.Control)checkBoxList);
            
            System.Web.UI.Page page = TrackingFieldPageBase.GetPage();
            if (page == null || page.IsPostBack)
                return;

            GoalsPipelineArgs args = new GoalsPipelineArgs();
            args.RulesFolderID = SitecoreIDs.GoalsRulesFolderID;
            ID id = ShortID.DecodeID(Sitecore.Context.Request.QueryString["id"]);
            if (!id.IsNull)
                args.ContextItem = Sitecore.Context.ContentDatabase.GetItem(id);

            CorePipeline.Run("uiGetGoals", args);

            TrackingFieldPageBase.RenderCheckBoxList(checkBoxList, args.PageEvents.OrderBy(e => e.DisplayName), selected);
        }
    }
}
