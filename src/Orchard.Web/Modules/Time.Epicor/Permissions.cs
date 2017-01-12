using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.Epicor
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission EpicorAccess = new Permission { Description = "Epicor Access", Name = "EpicorAccess" };
        public static readonly Permission LoadListEditor = new Permission { Description = "LoadList Editor", Name = "LoadListEditor" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                EpicorAccess,
                LoadListEditor
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {EpicorAccess, LoadListEditor}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {EpicorAccess}
                }
            };
        }
    }
}