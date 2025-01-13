using System.Globalization;
using ConfluencePOC.Web.Configuration;
using ConfluencePOC.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using ConfluencePOC.Web.Models.PageTypes;
using Contentful.Core;
using Contentful.Core.Models;
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
    public async Task<IActionResult> CatchAll(string slug, [FromQuery(Name = "bypass-cache")] bool bypassCache = false)
    {
        if (!CachingOptions.Enabled)
        {
            bypassCache = true;
        }
        
        if (string.IsNullOrEmpty(slug) || slug == Configuration.Options.Homepage)
        {
            var homepage = await Cache.GetOrSetAsync($"content:{Configuration.Options.Homepage}:{CultureInfo.CurrentCulture.Name}",
                async () =>
                {
                    var builder = new QueryBuilder<Homepage>().ContentTypeIs(Homepage.ContentType).Include(5).Limit(1);
                    return (await Client.GetEntries(builder)).FirstOrDefault();
                }, Options, bypassCache);
        
        
            return View("Homepage", homepage);
        }

        var result = await Cache.GetOrSetAsync($"content:{slug}:" + CultureInfo.CurrentCulture.Name,
            async () =>
            {
                var generalPages = new QueryBuilder<GeneralSupportPage>()
                    .ContentTypeIs(GeneralSupportPage.ContentType)
                    .Include(5)
                    .FieldEquals(c => c.Slug, slug)
                    .Limit(1);
                var listingPages = new QueryBuilder<ListingPage>()
                    .ContentTypeIs(ListingPage.ContentType)
                    .Include(5)
                    .FieldEquals(c => c.Slug, slug)
                    .Limit(1);
                var pages = new QueryBuilder<Page>()
                    .ContentTypeIs(Page.ContentType)
                    .Include(5)
                    .FieldEquals(c => c.Slug, slug)
                    .Limit(1);

                List<ContentfulPage> results = new List<ContentfulPage>();
                results.AddRange(await Client.GetEntries(generalPages));
                results.AddRange(await Client.GetEntries(listingPages));
                results.AddRange(await Client.GetEntries(pages));
                
                return results.FirstOrDefault();
            }, Options, bypassCache);

        if (result == null)
        {
            return NotFound();
        }

        switch (result)
        {
            case GeneralSupportPage generalSupportPage:
                return View("GeneralSupport", generalSupportPage);
            case ListingPage listingPage:
                return View("ListingPage", listingPage);
            case Page page:
                return View("Page", page);
            default:
                return NotFound();
        }
    }
}