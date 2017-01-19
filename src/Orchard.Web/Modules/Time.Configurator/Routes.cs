using Orchard.Mvc.Routes;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Time.Configurator
{
    [Themed]
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Priority = 5,
                     Route = new Route(
                        "Configurator/{controller}/{action}/{id}",
                        new RouteValueDictionary {
                                                    {"area", "Time.Configurator"},
                                                    {"controller", "Configurator"},
                                                    {"action", "Index"},
                                                    {"id", null}
                                                },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Time.Configurator"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}