using System.Diagnostics;
using System.Globalization;
using ConfluencePOC.Web.Configuration;
using ConfluencePOC.Web.Converters;
using ConfluencePOC.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using ConfluencePOC.Web.Models;
using ConfluencePOC.Web.Models.PageTypes;
using ConfluencePOC.Web.Models.Resolvers;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace ConfluencePOC.Web.Controllers;

public abstract class ContentfulController : Controller
{
    protected readonly ILogger Logger;
    protected readonly IContentfulClient Client;
    protected readonly IDistributedCache Cache;
    protected readonly DistributedCacheEntryOptions Options;
    protected readonly CachingOptions CachingOptions;
    protected Models.Contentful.ConfigurationEntity Configuration;

    public ContentfulController(ILogger logger, IContentfulClient client, IDistributedCache cache, IOptions<CachingOptions> cachingOptions)
    {
        Logger = logger;
        Client = client;
        Client.ContentTypeResolver = new EntityResolver();
        Client.SerializerSettings.Converters.RemoveAt(0);
        Client.SerializerSettings.Converters.Insert(0, new GDSAssetJsonConverter());
        Client.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        Cache = cache;
        Options = new DistributedCacheEntryOptions().SetSlidingExpiration(cachingOptions.Value.Duration);

        CachingOptions = cachingOptions.Value;

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        bool bypassCache = context.HttpContext.Request.Query["bypass-cache"] == "true" || !CachingOptions.Enabled;
        
        var configuration = Cache.GetOrSetAsync("configuration",
            async () =>
            {
                var builder = new QueryBuilder<Models.Contentful.ConfigurationEntity>().ContentTypeIs(Models.Contentful.ConfigurationEntity.ContentType).Include(5).Limit(1);
                return (await Client.GetEntries(builder)).FirstOrDefault();
            }, Options, bypassCache).GetAwaiter().GetResult();
        

        var navigation = Cache.GetOrSetAsync("navigation",
            async () =>
            {
                var links = configuration.Navigation.Select(link => new Link() { Slug = link.Slug, Title = link.Title }).ToList();
                return links;
            }, Options, bypassCache).GetAwaiter().GetResult();
        
        ViewBag.Navigation = new Navigation() { Links = navigation ?? [] };;
        ViewBag.Homepage = configuration?.HomePage?.Slug;
        ViewBag.Footer = configuration?.Footer;
        
        Cache.SetString("homepage", configuration?.HomePage?.Slug ?? string.Empty);

        Configuration = configuration;
        
        base.OnActionExecuting(context);
    }

    
}