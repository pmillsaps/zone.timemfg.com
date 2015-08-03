using System.Collections.Generic;
using Ventajou.ActiveDirectory.Models;

namespace Ventajou.ActiveDirectory.ViewModels
{
	public class SettingsViewModel
	{
		public string DefaultDomain { get; set; }
		public IEnumerable<DomainRecord> Domains { get; set; }
	}
}
