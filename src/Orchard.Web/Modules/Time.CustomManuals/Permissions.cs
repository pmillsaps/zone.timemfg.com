using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.CustomManuals
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission CustomManualsAdmin = new Permission { Description = "Custom Manuals Admin", Name = "CustomManualsAdmin" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                CustomManualsAdmin
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { CustomManualsAdmin }
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] { CustomManualsAdmin }
                },
             };
        }
    }
}