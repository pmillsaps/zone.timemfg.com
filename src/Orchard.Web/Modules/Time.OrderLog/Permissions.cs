using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.OrderLog
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ViewOrders = new Permission { Description = "View Orders", Name = "ViewOrders" };
        public static readonly Permission EditOrders = new Permission { Description = "Edit Orders", Name = "EditOrders" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ViewOrders, EditOrders
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ViewOrders, EditOrders}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {ViewOrders}
                }
            };
        }
    }
}