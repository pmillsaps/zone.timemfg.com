﻿using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Time.HelloWorld
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
                                                         "Hello/{action}",
                                                         new RouteValueDictionary {
                                                                                      {"area", "Time.HelloWorld"},
                                                                                      {"controller", "Home"},
                                                                                      {"action", "Index"}
                                                                                  },
                                                         new RouteValueDictionary {
                                                                                      {"area", "Time.HelloWorld"},
                                                                                      {"controller", "Home"},
                                                                                  },
                                                         new RouteValueDictionary {
                                                                                      {"area", "Time.HelloWorld"}
                                                                                  },
                                                         new MvcRouteHandler())}};
        }
    }
}