using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Time.Drawings
{
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
                        "Drawings/{action}/{id}",
                        new RouteValueDictionary {
                                                    {"area", "Time.Drawings"},
                                                    {"controller", "Home"},
                                                    {"action", "Index"},
                                                    {"id", null}
                                                },
                        new RouteValueDictionary {
                                                    {"area", "Time.Drawings"},
                                                    {"controller", "Home"},
                                                },
                        new RouteValueDictionary {
                                                    {"area", "Time.Drawings"}
                                                },
                        new MvcRouteHandler())}
            };
        }
    }
}