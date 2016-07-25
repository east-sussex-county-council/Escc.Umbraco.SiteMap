using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Escc.Umbraco.SiteMap.Models;
using Escc.Umbraco.SiteMap.Services.Interfaces;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Escc.Umbraco.SiteMap.Services
{
    public class MediaLibraryService : IMediaLibraryService
    {
        private readonly string _cdnDomain;
        private const string ContentTypeFolder = "folder";

        public MediaLibraryService()
        {
            _cdnDomain = GetCdnDomain();
        }

        /// <summary>
        /// Search the Media library for PDF files via Media library
        /// </summary>
        /// <returns>
        /// List of PDF nodes
        /// </returns>
        public IEnumerable<UmbracoSitemapNode> GetMediaFileNodes()
        {
            var exclusionList = MediaFilesExclusionList();

            var helper = new UmbracoHelper(UmbracoContext.Current);
            var nodes = new List<UmbracoSitemapNode>();
            
            var rootMedia = helper.TypedMediaAtRoot();

            if (rootMedia == null) return nodes;


            var mediaFiles = rootMedia as List<IPublishedContent> ?? rootMedia.ToList();

            // Get all media nodes, ignoring folders and types listed in the exclusions list
            var rootFiles =
                mediaFiles.Select(x => x)
                    .Where(
                        x =>
                            exclusionList.Contains(x.GetPropertyValue<string>("umbracoExtension")) == false &&
                            x.ContentType.Alias.ToLowerInvariant() != ContentTypeFolder);

            var descendantFiles =
                mediaFiles.SelectMany(x => x.Descendants())
                    .Where(
                        x =>
                            exclusionList.Contains(x.GetPropertyValue<string>("umbracoExtension")) == false &&
                            x.ContentType.Alias.ToLowerInvariant() != ContentTypeFolder);

            var mediaItems = rootFiles.Union(descendantFiles).OrderByDescending(x => x.CreateDate);

            foreach (var mediaItem in mediaItems)
            {
                var path = mediaItem.Url;

                // if there is no path then this is a media item with no attached file
                if (string.IsNullOrEmpty(path)) continue;

                var url = HttpUtility.UrlPathEncode(string.Format("{0}{1}", _cdnDomain, path));

                nodes.Add(
                    new UmbracoSitemapNode
                    {
                        Url = url,
                        Priority = 1,
                        LastModified = mediaItem.UpdateDate
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

        /// <summary>
        /// Get a list of extensions to exclude from the site map
        /// </summary>
        /// <returns>
        /// A list of excluded file extensions
        /// </returns>
        private static IList<string> MediaFilesExclusionList()
        {
            //<add key="sitemapMediaFilesExclusions" value="jpeg, jpg, png, gif, bmp, svg" />
            var exclusions = ConfigurationManager.AppSettings["sitemapMediaFilesExclusions"];
            if (String.IsNullOrEmpty(exclusions))
            {
                return new string[0];
            }

            return exclusions.Split(',').Select(s => s.Trim()).ToList();
        }
    }
}