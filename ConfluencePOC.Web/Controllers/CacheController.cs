using ConfluencePOC.Web.Configuration;
using ConfluencePOC.Web.Converters;
using ConfluencePOC.Web.Extensions;
using ConfluencePOC.Web.Models;
using ConfluencePOC.Web.Models.PageTypes;
using ConfluencePOC.Web.Models.Resolvers;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ConfluencePOC.Web.Controllers;

[ApiController]
[Route("cache")]
public class CacheController : ControllerBase
{
    private readonly IDistributedCache _cache;
    private readonly IContentfulClient _client;
    private readonly DistributedCacheEntryOptions _options;

    public CacheController(IContentfulClient client, IDistributedCache cache, IOptions<CachingOptions> cachingOptions)
    {
        _client = client;
        _client.ContentTypeResolver = new EntityResolver();
        _client.SerializerSettings.Converters.RemoveAt(0);
        _client.SerializerSettings.Converters.Insert(0, new GDSAssetJsonConverter());
        _client.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        _cache = cache;
        _options = new DistributedCacheEntryOptions().SetSlidingExpiration(cachingOptions.Value.Duration);
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<CacheResult>> Get(string slug)
    {
        // Let's see if we have the page cached
        if (_cache.TryGetValue($"content:{slug}", out Page? page))
        {
            // Lets' grab all our translations, so we can check if we have any cached entries for the slug
            _cache.TryGetValue("translations", out Dictionary<string, string[]>? translations);

            // If we have it, let's go through and expire them all
            string[]? keys = null;
            translations?.TryGetValue(slug, out keys);

            return new CacheResult(slug, page?.Sys?.UpdatedAt, page?.Fetched, keys);
        }

        return new CacheResult(slug);
    }

    [HttpPost]
    public async Task<ActionResult> Publish(string slug)
    {
        // Clear the cache for this entry first
        await Delete(slug);

        // Now re-add it to the cache with the updated data
        var result = await _cache.GetOrSetAsync($"content:{slug}",
            async () =>
            {
                var pages = new QueryBuilder<Page>()
                    .ContentTypeIs(Page.ContentType)
                    .Include(5)
                    .FieldEquals(c => c.Slug, slug)
                    .Limit(1);

                return (await _client.GetEntries(pages)).FirstOrDefault();

            }, _options, true);

        return Accepted();

    }

    [HttpDelete]
    public async Task<ActionResult> Delete(string slug)
    {
        // Lets' grab all our translations, so we can check if we have any cached entries for the slug
        _cache.TryGetValue("translations", out Dictionary<string, string[]>? translations);

        // If we have it, let's go through and expire them all
        if (translations != null && translations.TryGetValue(slug, out var keys))
        {
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key);
            }
        }

        // Purge our entry from the slug
        await _cache.RemoveAsync($"content:{slug}");
        
        return Accepted();
    }

    
    
}