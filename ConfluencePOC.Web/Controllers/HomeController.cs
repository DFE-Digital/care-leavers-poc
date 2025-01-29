using ConfluencePOC.Web.Configuration;
using ConfluencePOC.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ConfluencePOC.Web.Controllers;

public class HomeController : ContentfulController
{
    
    public HomeController(ILogger<HomeController> logger, IContentfulClient client, IDistributedCache cache, IOptions<CachingOptions> cachingOptions) : 
        base(logger, client, cache, cachingOptions)
    {
        
    }

    [Route("/{**slug}")]
    public async Task<IActionResult> CatchAll(string? slug, [FromQuery(Name = "bypass-cache")] bool bypassCache = false)
    {
        if (!CachingOptions.Enabled)
        {
            bypassCache = true;
        }

        if (string.IsNullOrEmpty(slug))
        {
            slug = Configuration?.HomePage?.Slug;
        }
        
        var result = await Cache.GetOrSetAsync($"content:{slug}",
            async () =>
            {
                var pages = new QueryBuilder<Page>()
                    .ContentTypeIs(Page.ContentType)
                    .Include(5)
                    .FieldEquals(c => c.Slug, slug)
                    .Limit(1);

                return (await Client.GetEntries(pages)).FirstOrDefault();
                
            }, Options, bypassCache);

        if (result == null)
        {
            return NotFound();
        }

        return View("Page", result);
        
    }
}