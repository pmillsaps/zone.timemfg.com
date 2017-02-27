using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Time.Admin
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
                        "TimeAdmin/{controller}/{action}/{id}",
                        new RouteValueDictionary {
                                                    {"area", "Time.Admin"},
                                                    {"controller", "Equipment"},
                                                    {"action", "Index"},
                                                    {"id", null}
                                                },
                        new RouteValueDictionary (),
                        new RouteValueDictionary {
                                                    {"area", "Time.Admin"}
                                                },
                        new MvcRouteHandler())}
                 
            };
        }
    }
}