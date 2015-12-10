using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web;
using Escc.Umbraco.SiteMap.Models;
using Escc.Umbraco.SiteMap.Services.Interfaces;
using Examine;
using Umbraco.Core;

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
        /// Search the Media library for PDF files and return a sitemap
        /// </summary>
        /// <returns></returns>
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