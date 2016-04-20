using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.Support
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission SupportAdmin = new Permission { Description = "Support Admin", Name = "SupportAdmin" };
        public static readonly Permission SupportApprover = new Permission { Description = "SupportApprover", Name = "EditInvoicingOrders" };
        public static readonly Permission SupportIT = new Permission { Description = "Support IT", Name = "SupportIT" };
        public static readonly Permission ViewWaterReports = new Permission { Description = "View Water Reports", Name = "ViewWaterReports" };
        public static readonly Permission EnterWaterReports = new Permission { Description = "Enter Water Reports", Name = "EnterWaterReports" };
        public static readonly Permission EditWaterReports = new Permission { Description = "Edit Water Reports", Name = "EditWaterReports" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                SupportAdmin, SupportApprover, SupportIT, ViewWaterReports, EnterWaterReports, EditWaterReports
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {SupportAdmin}
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] {SupportIT}
                },
                  new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {ViewWaterReports, EnterWaterReports}
                },
            };
        }
    }
}