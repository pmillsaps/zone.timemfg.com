using JetBrains.Annotations;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Time.Support.Models;

namespace Time.Support.Handlers
{
    [UsedImplicitly]
    public class SupportSettingsPartHandler : ContentHandler
    {
        public SupportSettingsPartHandler(IRepository<SupportSettingsPartRecord> repository)
        {
            Filters.Add(new ActivatingFilter<SupportSettingsPart>("User"));
            Filters.Add(new TemplateFilterForPart<SupportSettingsPart>("SupportSettings", "Parts/Time.Support.SupportSettings", "Modules"));
            //Filters.Add(StorageFilter.For(repository));
        }

        //protected override void GetItemMetadata(GetContentItemMetadataContext context) {
        //    var part = context.ContentItem.As<SupportSettingsPart>();

        //    if (part != null) {
        //        context.Metadata.Identity.Add("User.UserName", part);
        //        context.Metadata.DisplayText = part.UserName;
        //    }
        //}


        //public SupportSettingsPartHandler()
        //{
        //    Filters.Add(new ActivatingFilter<SupportSettingsPart>("Site"));
        //    Filters.Add(new TemplateFilterForPart<SupportSettingsPart>("SupportSettings", "Parts/Time.Support.SupportSettings", "Modules"));
        //}
    }
}