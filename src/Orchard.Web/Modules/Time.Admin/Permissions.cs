using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.Admin
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageEquipment = new Permission { Description = "Manage Versalift.com Equipment Lists", Name = "ManageEquipment" };
        public static readonly Permission ViewEquipment = new Permission { Description = "View Versalift.com Equipment Lists", Name = "ViewEquipment" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                ManageEquipment,
                ViewEquipment
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageEquipment}
                },
                new PermissionStereotype {
                    Name = "Marketing",
                    Permissions = new[] {ManageEquipment}
                }
            };
        }
    }
}