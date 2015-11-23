using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Analytics.Data.Items;
using Sitecore.Analytics.MultiSite.Pipelines;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using SC = Sitecore.Shell.Applications.Analytics.Personalization;

namespace Sitecore.Analytics.MultiSite.Client
{
    public class ProfileCards : SC.ProfileCards
    {
        public void InitializeControl(Item contextItem)
        {
            Assert.IsNotNull((object)contextItem, "context item");
            this.EnsureChildControls();
            //this.container.Controls.Clear();

            ProfilesPipelineArgs args = new ProfilesPipelineArgs()
            {
                RulesFolderID = SitecoreIDs.ProfilesRulesFolderID,
                ContextItem = contextItem
            };

            CorePipeline.Run("uiGetProfiles", args);
            if (args.Profiles == null || args.Profiles.Count() == 0)
            {
                this.BuildEmptyProfileView();
                return;
            }

            TrackingField trackingField = this.GetTrackingField(contextItem);
            if (trackingField == null)
                return;

            List<ProfileCards.ProfileItemSortWrapper> list = new List<ProfileCards.ProfileItemSortWrapper>();
            foreach (ProfileItem profileItem in args.Profiles)
            {
                Item item = (Item)profileItem;
                if (!(item.TemplateID != AnalyticsIds.Profile))
                {
                    bool withPercentage;
                    list.Add(new ProfileCards.ProfileItemSortWrapper(item, ProfileUtil.HasPresetData(item, trackingField), ProfileUtil.IsMultiplePreset(item, out withPercentage)));
                }
            }
            list.Sort((Comparison<ProfileCards.ProfileItemSortWrapper>)((x, y) =>
            {
                if (x.SortingKey == y.SortingKey)
                    return x.ProfileItem.Key.CompareTo(y.ProfileItem.Key);
                return x.SortingKey.CompareTo(y.SortingKey) * -1;
            }));

            if (list.Count > 0)
            {
                foreach (ProfileCards.ProfileItemSortWrapper profileItemSortWrapper in list)
                {
                    bool profileCardsVisible;
                    this.BuildProfileView(profileItemSortWrapper.ProfileItem, trackingField, out profileCardsVisible);
                }
            }
            else
                this.BuildEmptyProfileView();
        }

        private TrackingField GetTrackingField(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            Field innerField = item.Fields["__Tracking"];
            if (innerField != null)
                return new TrackingField(innerField);
            return (TrackingField)null;
        }

        private class ProfileItemSortWrapper
        {
            /// <summary>
            /// The profile item.
            /// 
            /// </summary>
            private readonly Item profileItem;
            /// <summary>
            /// The sorting key.
            /// 
            /// </summary>
            private readonly int sortingKey;

            /// <summary>
            /// Gets ProfileItem.
            /// 
            /// </summary>
            public Item ProfileItem
            {
                get
                {
                    return this.profileItem;
                }
            }

            /// <summary>
            /// Gets SortingKey.
            /// 
            /// </summary>
            public int SortingKey
            {
                get
                {
                    return this.sortingKey;
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Sitecore.Shell.Applications.Analytics.Personalization.ProfileCards.ProfileItemSortWrapper"/> class.
            /// 
            /// </summary>
            /// <param name="profileItem">The profile item.
            ///             </param><param name="hasValue">The has value.
            ///             </param><param name="isMultiple">The is multiple.
            ///             </param>
            public ProfileItemSortWrapper(Item profileItem, bool hasValue, bool isMultiple)
            {
                Assert.ArgumentNotNull((object)profileItem, "profileItem");
                this.profileItem = profileItem;
                this.sortingKey = 0;
                this.sortingKey |= isMultiple ? 1 : 0;
                this.sortingKey |= hasValue ? 2 : 0;
            }
        }
    }
}
