using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.DataPlates
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission DataPlateEditor = new Permission { Description = "Edit Data Plates", Name = "DataPlateEditor" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                DataPlateEditor
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] { DataPlateEditor }
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] { DataPlateEditor }
                },
            };
        }
    }
}