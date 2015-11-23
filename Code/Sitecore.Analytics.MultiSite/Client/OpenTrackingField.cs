using Sitecore;
using Sitecore.Analytics.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.StringExtensions;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.XamlSharp.Continuations;
using Sitecore.Shell.Applications.Analytics.TrackingField;
using System;
using System.Collections.Specialized;

namespace Sitecore.Analytics.MultiSite.Client
{
    /// <summary>
    /// Defines the open tracking field class.
    /// </summary>
    [UsedImplicitly]
    [Serializable]
    public class OpenTrackingField : Sitecore.Shell.Applications.Analytics.TrackingField.OpenTrackingField
    {
        private CommandContext context;

        /// <summary>
        /// Executes the command in the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(CommandContext context)
        {
            this.context = context;
            base.Execute(context);
        }

        /// <summary>
        /// Runs the pipeline.
        /// </summary>
        /// <param name="args">The arguments.</param>
        [UsedImplicitly]
        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Item obj = this.DeserializeItems(args.Parameters["items"])[0];
            if (!SheerResponse.CheckModified())
                return;

            string index = args.Parameters["fieldid"];
            if (string.IsNullOrEmpty(index))
                index = "__Tracking";
            
            if (args.IsPostBack)
            {
                if (!args.HasResult)
                    return;
                
                using (new StatisticDisabler(StatisticDisablerState.ForItemsWithoutVersionOnly))
                {
                    obj.Editing.BeginEdit();
                    obj[index] = args.Result;
                    obj.Editing.EndEdit();
                }
                
                if (AjaxScriptManager.Current != null)
                {
                    AjaxScriptManager.Current.Dispatch("analytics:trackingchanged");
                }
                else
                {
                    Context.ClientPage.SendMessage((object)this, "analytics:trackingchanged");
                    Context.ClientPage.SendMessage((object)this, "item:refresh(id={0})".FormatWith(new object[] {obj.ID.ToString()} ));
                }
            }
            else if (obj.Appearance.ReadOnly)
            {
                SheerResponse.Alert("You cannot edit the '{0}' item because it is protected.", new string[1]
                {
                  obj.DisplayName
                });
            }
            else if (!obj.Access.CanWrite())
            {
                SheerResponse.Alert("You cannot edit this item because you do not have write access to it.");
            }
            else
            {
                UrlString urlString = new UrlString(this.GetUrl());
                urlString.Add("id", context.Items[0].ID.ToShortID().ToString());

                UrlHandle urlHandle = new UrlHandle();
                urlHandle["tracking"] = obj[index];
                urlHandle.Add(urlString);
                
                this.ShowDialog(urlString.ToString());
                args.WaitForPostBack();
            }
        }
    }
}
