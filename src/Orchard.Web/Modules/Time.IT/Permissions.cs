using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.IT
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission IT = new Permission { Description = "IT", Name = "IT" };
        public static readonly Permission ITAdmin = new Permission { Description = "IT Admin", Name = "ITAdmin" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                IT, ITAdmin
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {IT, ITAdmin}
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] {IT, ITAdmin}
                },
            };
        }
    }
}