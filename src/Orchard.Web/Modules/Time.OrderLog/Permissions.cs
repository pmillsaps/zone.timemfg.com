using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.OrderLog
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ViewOrders = new Permission { Description = "View Orders", Name = "ViewOrders" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ViewOrders
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ViewOrders}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {ViewOrders}
                }
            };
        }
    }
}