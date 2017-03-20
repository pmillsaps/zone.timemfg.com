using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Time.Install
{
    //public class Permissions : IPermissionProvider
    //{
    //    public static readonly Permission ConfiguratorAdmin = new Permission { Description = "Configurator Admin", Name = "ConfiguratorAdmin" };
    //    public static readonly Permission ConfiguratorSales = new Permission { Description = "Sales Configurator", Name = "ConfiguratorSales" };

    //    public virtual Feature Feature { get; set; }

    //    public IEnumerable<Permission> GetPermissions()
    //    {
    //        return new[] {
    //            ConfiguratorAdmin, ConfiguratorSales
    //        };
    //    }

    //    public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
    //    {
    //        return new[] {
    //            new PermissionStereotype {
    //                Name = "Administrator",
    //                Permissions = new[] { ConfiguratorAdmin, ConfiguratorSales }
    //            },
    //            new PermissionStereotype {
    //                Name = "IT",
    //                Permissions = new[] { ConfiguratorAdmin, ConfiguratorSales }
    //            },
    //              new PermissionStereotype {
    //                Name = "Sales",
    //                Permissions = new[] { ConfiguratorSales }
    //            },
    //        };
    //    }
    //}
}