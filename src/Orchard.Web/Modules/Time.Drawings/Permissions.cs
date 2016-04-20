using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;

namespace Time.Drawings
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission DrawingAdmin = new Permission { Description = "Drawing Admin", Name = "DrawingAdmin" };
        public static readonly Permission DrawingManager = new Permission { Description = "Drawing Manager", Name = "DrawingManager" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                DrawingAdmin, DrawingManager
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {DrawingAdmin, DrawingManager}
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] {DrawingAdmin, DrawingManager}
                },
                  new PermissionStereotype {
                    Name = "Drawings",
                    Permissions = new[] {DrawingManager}
                },
            };
        }
    }
}