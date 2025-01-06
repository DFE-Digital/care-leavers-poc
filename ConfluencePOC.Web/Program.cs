using ConfluencePOC.Web.Configuration;
using ConfluencePOC.Web.Converters;
using ConfluencePOC.Web.Extensions;
using ConfluencePOC.Web.GDSRenderers;
using ConfluencePOC.Web.Middleware;
using ConfluencePOC.Web.Models.Resolvers;
using Contentful.AspNetCore;
using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Joonasw.AspNetCore.SecurityHeaders;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddGovUkFrontend();
builder.Services.AddCsp(nonceByteAmount: 32);

// Bind configs
builder.Services.Configure<CachingOptions>(
    builder.Configuration.GetSection(CachingOptions.Name)
);

var homepage = builder.Configuration.GetValue<string>("Homepage");
if (homepage != null)
    Options.Homepage = homepage;

var googleApiKey = builder.Configuration.GetValue<string>("GoogleApiKey");
if (googleApiKey != null)
    Options.GoogleApiKey = googleApiKey;

// Validate configs
builder.Configuration.Get<CachingOptions>()?.Validate();

if (builder.Configuration.Get<CachingOptions>() is { UseRedis: true })
{
    // Add distributed Redis cache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.Get<CachingOptions>()?.ConnectionString;
        options.InstanceName = builder.Configuration.Get<CachingOptions>()?.InstanceName;
    });
}
else
{
    // Use the distributed memory cache in development
    builder.Services.AddDistributedMemoryCache();
}

// Add Contentful
builder.Services.AddContentful(builder.Configuration);

// Fix rendering list items with paragraph tags
builder.Services.AddTransient<HtmlRenderer>((c) => {
    var renderer = new HtmlRenderer(new HtmlRendererOptions
    {
        ListItemOptions = new ListItemContentRendererOptions
        {
            OmitParagraphTagsInsideListItems = true
        }
    });
    
    // Add custom GDS renderer
    renderer.AddRenderer(new GDSAssetRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSAssetHyperlinkRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSHeadingRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSHorizontalRulerContentRenderer());
    renderer.AddRenderer(new GDSHyperlinkContentRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSListContentRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSParagraphRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSEntityLinkContentRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSGridRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSCardRenderer(renderer.Renderers));
    renderer.AddRenderer(new GDSExternalAgencyRenderer(renderer.Renderers));
    
    return renderer;
});

var app = builder.Build();

// Set default serializer
var contentful = app.Services.GetService(typeof(IContentfulClient)) as ContentfulClient;
contentful!.SerializerSettings.Converters.RemoveAt(0);
contentful!.SerializerSettings.Converters.Insert(0, new GDSAssetJsonConverter());
contentful!.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

DistributedCacheExtensions.SerializerSettings = contentful?.SerializerSettings;
DistributedCacheExtensions.Serializer = contentful?.Serializer;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseGoogleTranslate();

app.Run();