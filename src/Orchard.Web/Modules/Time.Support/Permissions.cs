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


        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                SupportAdmin, SupportApprover, SupportIT
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {SupportAdmin, SupportApprover}
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] {SupportIT}
                },
            };
        }
    }
}