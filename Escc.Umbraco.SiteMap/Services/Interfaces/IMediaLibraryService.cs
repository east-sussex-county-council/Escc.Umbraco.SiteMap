using System.Collections.Generic;
using Escc.Umbraco.SiteMap.Models;

namespace Escc.Umbraco.SiteMap.Services.Interfaces
{
    public interface IMediaLibraryService
    {
        IEnumerable<UmbracoSitemapNode> GetPdfFileNodes();
        IEnumerable<UmbracoSitemapNode> GetPdfFileMediaNodes();
    }
}
