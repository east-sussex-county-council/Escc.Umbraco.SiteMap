using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Escc.Umbraco.SiteMap.Models;
using Escc.Umbraco.SiteMap.Services;
using Escc.Umbraco.SiteMap.Services.Interfaces;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Escc.Umbraco.SiteMap.Controllers
{
    public class SitemapController : PluginController
    {
        private readonly IMediaLibraryService _mediaLibraryService;

        public SitemapController() : this(UmbracoContext.Current)
        {
            
        }

        public SitemapController(UmbracoContext umbracoContext) : base(umbracoContext)
        {
            _mediaLibraryService = new MediaLibraryService();
        }

        public ActionResult Index()
        {
            return HttpNotFound();
        }

        public ActionResult PdfFiles()
        {
            try
            {
                var pdfNodes = _mediaLibraryService.GetPdfFileNodes();
                var xml = GetSitemapDocument(pdfNodes);
                return Content(xml, "text/xml", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return HttpNotFound(ex.Message);
            }
        }

        /// <summary>
        /// Create a full sitemap using the supplied nodes collection
        /// </summary>
        /// <param name="sitemapNodes">
        /// list of nodes to be included in the sitemap
        /// </param>
        /// <returns>
        /// Formatted XML Sitemap
        /// </returns>
        private string GetSitemapDocument(IEnumerable<UmbracoSitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            foreach (var sitemapNode in sitemapNodes)
            {
                var urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(xmlns + "lastmod", sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(xmlns + "changefreq", sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(xmlns + "priority", sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));

                root.Add(urlElement);
            }

            var document = new XDocument(root);
            return document.ToString();
        }
    }
}