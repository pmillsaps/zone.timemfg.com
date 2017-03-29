using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.Epicor
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission EpicorAccess = new Permission { Description = "Epicor Access", Name = "EpicorAccess" };
        public static readonly Permission LoadListEditor = new Permission { Description = "LoadList Editor", Name = "LoadListEditor" };
        public static readonly Permission V8Transfer = new Permission { Description = "V8 Transfer", Name = "V8Transfer" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                EpicorAccess,
                LoadListEditor,
                V8Transfer
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {EpicorAccess, LoadListEditor, V8Transfer}
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {EpicorAccess}
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] {EpicorAccess, LoadListEditor, V8Transfer }
                }
            };
        }
    }
}