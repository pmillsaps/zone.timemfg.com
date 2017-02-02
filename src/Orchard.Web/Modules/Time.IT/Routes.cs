using Orchard.Mvc.Routes;
using Orchard.Themes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Time.IT
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
                        "IT/{controller}/{action}/{id}",
                        new RouteValueDictionary {
                                                    {"area", "Time.IT"},
                                                    {"controller", "IT"},
                                                    {"action", "Index"},
                                                    {"id", null}
                                                },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Time.IT"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}