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
        public static readonly Permission EmployeeMaintenance = new Permission { Description = "Employee Maintenance", Name = "EmployeeMaintenance" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                IT, ITAdmin, EmployeeMaintenance
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {IT, EmployeeMaintenance }
                },
                new PermissionStereotype {
                    Name = "IT",
                    Permissions = new[] {IT, EmployeeMaintenance }
                },
                new PermissionStereotype {
                    Name = "Maintenance",
                    Permissions = new[] { EmployeeMaintenance }
                },
            };
        }
    }
}