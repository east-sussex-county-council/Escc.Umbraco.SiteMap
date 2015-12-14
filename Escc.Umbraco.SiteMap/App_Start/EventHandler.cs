using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Escc.Umbraco.SiteMap.Controllers;
using Escc.Umbraco.SiteMap.Services;
using Escc.Umbraco.SiteMap.Services.Interfaces;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;

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
                });

            var builder = new ContainerBuilder();

            // web api controllers
            builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly);
            builder.RegisterApiControllers(typeof(SitemapController).Assembly);

            //add custom class to the container as Transient instance
            builder.RegisterType<RepositoryFactory>();
            builder.RegisterType<MediaService>().As<IMediaService>();
            builder.RegisterType<MediaLibraryService>().As<IMediaLibraryService>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            var resolver = new AutofacWebApiDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}