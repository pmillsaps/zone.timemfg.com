using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace Time.Support
{
    [OrchardFeature("Time.Support")]
	public class AdminMenu : INavigationProvider
	{
		public Localizer T { get; set; }

		public string MenuName
		{
			get { return "admin"; }
		}

		public void GetNavigation(NavigationBuilder builder)
		{
			builder
				.Add(T("Settings"), menu => menu
					.Add(T("Time.Support"), "1", item =>
                        item.Action("Settings", "Support", new { area = "Time.Support" }).Permission(StandardPermissions.SiteOwner)));
		}
	}
}