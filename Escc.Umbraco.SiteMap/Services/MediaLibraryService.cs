using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Escc.Umbraco.SiteMap.Models;
using Escc.Umbraco.SiteMap.Services.Interfaces;
using Examine;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Escc.Umbraco.SiteMap.Services
{
    public class MediaLibraryService : IMediaLibraryService
    {
        private readonly string _cdnDomain;

        public MediaLibraryService()
        {
            _cdnDomain = GetCdnDomain();
        }

        /// <summary>
        /// Search the Media library for PDF files via Examine index
        /// </summary>
        /// <returns>
        /// List of PDF nodes
        /// </returns>
        public IEnumerable<UmbracoSitemapNode> GetPdfFileNodes()
        {
            var nodes = new List<UmbracoSitemapNode>();

            // Search media Library for PDF files.
            var examineIndex = ExamineManager.Instance.SearchProviderCollection["InternalSearcher"];
            var criteria = examineIndex.CreateSearchCriteria("media");
            var filter = criteria.Field("umbracoExtension", "pdf");
            var results = examineIndex.Search(filter.Compile());

            foreach (var result in results)
            {
                var id = result.Id;
                var mediaFile = ApplicationContext.Current.Services.MediaService.GetById(id);
                var url = HttpUtility.UrlPathEncode(string.Format("{0}{1}", _cdnDomain, mediaFile.GetValue("umbracoFile")));

                DateTime updateDate;

                //if (updateDate.Length > 8) updateDate = adt.Substring(0, 8);
                DateTime.TryParseExact(result.Fields["updateDate"], "yyyyMMddHmmssFFF", CultureInfo.InvariantCulture, DateTimeStyles.None, out updateDate);

                nodes.Add(
                    new UmbracoSitemapNode
                    {
                        Url = url,
                        Priority = 1,
                        LastModified = updateDate
                    }
                );
            }

            return nodes;
        }

        /// <summary>
        /// Search the Media library for PDF files via Media library
        /// </summary>
        /// <returns>
        /// List of PDF nodes
        /// </returns>
        public IEnumerable<UmbracoSitemapNode> GetPdfFileMediaNodes()
        {
            var helper = new UmbracoHelper(UmbracoContext.Current);
            var nodes = new List<UmbracoSitemapNode>();
            
            var rootMedia = helper.TypedMediaAtRoot();

            if (rootMedia == null) return nodes;


            var mediaFiles = rootMedia as IList<IPublishedContent> ?? rootMedia.ToList();

            var rootFiles = mediaFiles.Select(x => x).Where(x => x.GetPropertyValue<string>("umbracoExtension") == "pdf");
            var descendantFiles = mediaFiles.SelectMany(x => x.Descendants()).Where(x => x.GetPropertyValue<string>("umbracoExtension") == "pdf");

            var images = rootFiles.Union(descendantFiles).OrderByDescending(x => x.CreateDate);

            foreach (var pdf in images)
            {
                var id = pdf.Id;
                var mediaFile = ApplicationContext.Current.Services.MediaService.GetById(id);
                if (mediaFile == null) continue;

                var url = HttpUtility.UrlPathEncode(string.Format("{0}{1}", _cdnDomain, mediaFile.GetValue("umbracoFile")));

                nodes.Add(
                    new UmbracoSitemapNode
                    {
                        Url = url,
                        Priority = 1,
                        LastModified = pdf.UpdateDate
                    }
                );
            }

            return nodes;
        } 

        /// <summary>
        /// Get the CDN Url from AppSettings in web.config
        /// </summary>
        /// <returns>
        /// Url of CDN
        /// </returns>
        private static string GetCdnDomain()
        {
            return ConfigurationManager.AppSettings["cdnDomain"];
        }
    }
}