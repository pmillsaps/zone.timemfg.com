using Orchard.ContentManagement;


namespace Time.Support.Models
{
    public class SupportSettingsPart : ContentPart
    {
        public string EmailServer
        {
            get { return this.Retrieve(x => x.EmailServer); }
            set { this.Store(x => x.EmailServer, value); }
        }
    }
}