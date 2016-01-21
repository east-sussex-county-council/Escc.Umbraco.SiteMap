using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace Escc.Umbraco.SiteMap
{
    public class EventHandler : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            RouteTable.Routes.MapRoute(
                "",
                "Sitemap/{action}/{id}",
                new
                {
                    controller = "Sitemap",
                    action = "Index",
                    id = UrlParameter.Optional
                }
            );
        }
    }
}