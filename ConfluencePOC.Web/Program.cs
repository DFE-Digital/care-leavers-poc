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

/*
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
   // .AddJsonFile("appsettings.json", optional: false, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true,
        reloadOnChange: true);
*/

// Bind configs
builder.Services.Configure<CachingOptions>(
    builder.Configuration.GetSection(CachingOptions.Name)
);
builder.Services.Configure<TranslationOptions>(
    builder.Configuration.GetSection(TranslationOptions.Name)
);

var homepage = builder.Configuration.GetValue<string>("Homepage");
if (homepage != null)
    Options.Homepage = homepage;

// Validate configs
var cachingOptions = builder.Configuration.GetSection(CachingOptions.Name).Get<CachingOptions>();
cachingOptions?.Validate();
var translationOptions = builder.Configuration.GetSection(TranslationOptions.Name).Get<TranslationOptions>();
translationOptions?.Validate();

if (cachingOptions is { Enabled: true })
{
    if (cachingOptions is { UseRedis: true })
    {
        // Add distributed Redis cache
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = cachingOptions.ConnectionString;
            options.InstanceName = cachingOptions.InstanceName;
        });
    }
    else
    {
        // Use the distributed memory cache in development
        builder.Services.AddDistributedMemoryCache();
    }
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

if (translationOptions is { Method: TranslationOptions.TranslationMethod.Google })
    app.UseGoogleTranslate();
if (translationOptions is { Method: TranslationOptions.TranslationMethod.Azure })
    app.UseAzureTranslate();
app.Run();