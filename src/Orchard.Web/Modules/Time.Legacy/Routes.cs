using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Time.Legacy
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
                        "Legacy/{action}/{id}",
                        new RouteValueDictionary {
                                                {"area", "Time.Legacy"},
                                                {"controller", "Home"},
                                                {"action", "Index"},
                                                {"id", null}
                                            },
                        new RouteValueDictionary {
                                                {"area", "Time.Legacy"},
                                                {"controller", "Home"},
                                            },
                        new RouteValueDictionary {
                                                {"area", "Time.Legacy"}
                                            },
                        new MvcRouteHandler())}};
        }
    }
}