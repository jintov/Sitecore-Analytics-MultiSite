using Sitecore;
using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Data;
using Sitecore.Configuration;
using Sitecore.Controls;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using Sitecore.Web.UI.XamlSharp.Continuations;
using Sitecore.Web.UI.XamlSharp.Xaml;
using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Sitecore.Analytics.Data.Items;
using Sitecore.Analytics.MultiSite.Pipelines;
using Sitecore.Pipelines;
using Sitecore.Shell.Applications.Analytics.Personalization;
using SC = Sitecore.Shell.Applications.Analytics.Personalization;
using Sitecore.Pipelines.GetItemPersonalizationVisibility;
using Sitecore.Exceptions;

namespace Sitecore.Analytics.MultiSite.Client
{
    public class ProfileCardsForm : DialogPage, IHasCommandContext
    {
        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public Database Database
        {
            get
            {
                return Assert.ResultNotNull<Database>(this.GetDatabase());
            }
        }

        /// <summary>
        /// Gets or sets the context item id.
        /// </summary>
        /// <value>
        /// The context item id.
        /// </value>
        protected string ContextItemId
        {
            get
            {
                return StringUtil.GetString(this.ViewState["ContextItemId"]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.ViewState["ContextItemId"] = (object)value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>
        /// The name of the database.
        /// </value>
        protected string DatabaseName
        {
            get
            {
                return StringUtil.GetString(this.ViewState["DatabaseName"]);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.ViewState["DatabaseName"] = (object)value;
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public Language Language
        {
            get
            {
                if (this.ViewState["Language"] == null)
                    return Sitecore.Context.Language;
                return Language.Parse(this.ViewState["Language"] as string);
            }
            set
            {
                Assert.ArgumentNotNull((object)value, "value");
                this.ViewState["Language"] = (object)value.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the profile cards.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The profile cards.
        /// </value>
        protected new ProfileCards ProfileCards { get; set; }

        /// <summary>
        /// Gets or sets the profile cards container.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The profile cards container.
        /// </value>
        protected HtmlGenericControl ProfileCardsContainer { get; set; }

        /// <summary>
        /// The get command context.
        /// 
        /// </summary>
        /// 
        /// <returns/>
        public CommandContext GetCommandContext()
        {
            return (CommandContext)null;
        }

        /// <summary>
        /// Edits the profile.
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        protected void EditProfile(string profileId)
        {
            ContinuationManager.Current.Start((ISupportsContinuation)this, "OpenEditProfileForm", new ClientPipelineArgs(new NameValueCollection()
            {
                {
                  "id",
                  profileId
                },
                {
                  "la",
                  this.Language.Name
                }
             }));
        }

        /// <summary>
        /// Starts the edit profile dialog.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected new void OpenEditProfileForm(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");

            if (args.IsPostBack)
            {
                if (string.Compare(args.Result, "refresh", StringComparison.InvariantCultureIgnoreCase) != 0)
                    return;
                Item contextItem = this.GetItem(this.ContextItemId);
                Assert.IsNotNull((object)contextItem, "item");
                (this.ProfileCards as ProfileCards).InitializeControl(contextItem);
                SheerResponse.SetInnerHtml(this.ProfileCardsContainer.ClientID, HtmlUtil.RenderControl((Control)this.ProfileCards));
                this.Page.Session["TrackingFieldModified"] = (object)"1";
            }
            else
            {
                this.OpenEditProfileForm(args);
            }
        }

        protected Database GetDatabase()
        {
            Assert.IsNotNullOrEmpty(this.DatabaseName, "database name");
            return Assert.ResultNotNull<Database>(Factory.GetDatabase(this.DatabaseName));
        }

        /// <summary>
        /// Initializes the controls.
        /// </summary>
        protected new void InitializeControls()
        {
            if (this.Cancel != null)
                this.Cancel.Visible = false;
            Item contextItem = this.GetItem(this.ContextItemId);
            Assert.IsNotNull((object)contextItem, "item");
            this.ProfileCards.ReadOnly = contextItem.Appearance.ReadOnly || !contextItem.Access.CanWrite();
            this.ProfileCards.InitializeControl(contextItem);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.
        /// </param><exception cref="T:Sitecore.Exceptions.AccessDeniedException">Application access denied</exception>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
            if (!AnalyticsSettings.Enabled)
                return;
            if (!XamlControl.AjaxScriptManager.IsEvent)
            {
                UrlHandle urlHandleSecure = ProfileCardsForm.GetUrlHandleSecure();
                this.DatabaseName = ProfileCardsForm.UrlHandleNonEmptyValue(urlHandleSecure, "databasename");
                this.ContextItemId = ProfileCardsForm.UrlHandleNonEmptyValue(urlHandleSecure, "itemid");
                string @string = StringUtil.GetString(new string[1]
                {
                  urlHandleSecure["la"]
                });
                if (!string.IsNullOrEmpty(@string))
                    this.Language = Language.Parse(@string);
                ProfileCardsForm.CheckAccess(this.GetItem(this.ContextItemId));
            }
            this.InitializeControls();
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns>
        /// The Sitecore Item.
        /// </returns>
        protected new Item GetItem(string itemId)
        {
            Assert.ArgumentNotNull((object)itemId, "itemId");
            return this.Database.GetItem(itemId, this.Language);
        }

        /// <summary>
        /// Gets the URL handle.
        /// 
        /// </summary>
        /// 
        /// <returns/>
        /// <exception cref="T:Sitecore.Exceptions.AccessDeniedException">Application access denied</exception>
        internal static UrlHandle GetUrlHandleSecure()
        {
            UrlHandle urlHandle;
            try
            {
                urlHandle = UrlHandle.Get();
            }
            catch
            {
                urlHandle = (UrlHandle)null;
            }
            if (urlHandle == null || urlHandle.Keys.Count == 0)
                throw new AccessDeniedException("Application access denied");
            return urlHandle;
        }

        /// <summary>
        /// Checks the access.
        /// 
        /// </summary>
        /// <param name="item">The item.</param>
        internal static void CheckAccess(Sitecore.Data.Items.Item item)
        {
            if (CorePipelineFactory.GetPipeline("getItemPersonalizationVisibility", string.Empty) == null)
                return;
            GetItemPersonalizationVisibilityArgs personalizationVisibilityArgs = new GetItemPersonalizationVisibilityArgs(true, item);
            CorePipeline.Run("getItemPersonalizationVisibility", (PipelineArgs)personalizationVisibilityArgs);
            Assert.HasAccess(personalizationVisibilityArgs.Visible, "Application access denied.");
        }

        /// <summary>
        /// Gets non-empty value from UrlHandle.
        /// 
        /// </summary>
        /// <param name="handle">The handle.</param><param name="key">The key.</param>
        /// <returns/>
        /// 
        /// <remarks>
        /// Throws AccessDeniedException if value is empty.
        /// </remarks>
        internal static string UrlHandleNonEmptyValue(UrlHandle handle, string key)
        {
            Assert.ArgumentNotNull((object)handle, "handle");
            Assert.ArgumentNotNull((object)key, "key");
            string str = handle[key];
            Assert.HasAccess(!string.IsNullOrEmpty(str), "Application access denied.");
            return str;
        }
    }
}
