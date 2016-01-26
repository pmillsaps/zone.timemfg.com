using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.Invoicing
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission InvoicingAdmin = new Permission { Description = "Invoicing Admin", Name = "InvoicingAdmin" };
        public static readonly Permission EditInvoicingOrders = new Permission { Description = "Edit Invoicing Orders", Name = "EditInvoicingOrders" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                InvoicingAdmin, EditInvoicingOrders
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {EditInvoicingOrders, InvoicingAdmin}
                },
            };
        }
    }
}