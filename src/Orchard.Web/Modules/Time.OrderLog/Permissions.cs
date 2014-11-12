using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.OrderLog
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ViewOrders = new Permission { Description = "View Orders", Name = "ViewOrders" };
        public static readonly Permission EditOrders = new Permission { Description = "Edit Orders", Name = "EditOrders" };
        public static readonly Permission OrderLogReporting = new Permission { Description = "Order Log Reporting", Name = "OrderLogReporting" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ViewOrders, EditOrders, OrderLogReporting
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ViewOrders, EditOrders, OrderLogReporting}
                },
            };
        }
    }
}