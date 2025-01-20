using System.Globalization;
using System.Text;
using ConfluencePOC.Web.Configuration;
using ConfluencePOC.Web.Extensions;
using Google.Cloud.Translation.V2;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ConfluencePOC.Web.Middleware;

public class GoogleTranslateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TranslationOptions _translationOptions;
    private readonly CachingOptions _cachingOptions;

    public GoogleTranslateMiddleware(RequestDelegate next, IOptions<TranslationOptions> translationOptions,
        IOptions<CachingOptions> cachingOptions)
    {
        _next = next;
        _translationOptions = translationOptions.Value;
        _cachingOptions = cachingOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, IDistributedCache cache)
    {
        string? controllerName = context.Request.RouteValues["controller"]?.ToString();
        string? actionName = context.Request.RouteValues["action"]?.ToString();

        if (controllerName == "Home")
        {

            var cultureQuery = context.Request.Query["culture"];
            if (string.IsNullOrWhiteSpace(cultureQuery))
                cultureQuery = context.Request.Cookies["culture"];

            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new CultureInfo(cultureQuery);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                context.Response.Cookies.Append("culture", culture.Name);
            }

            var slug = context.Request.Path.Value;
            if (slug == PathString.Empty || slug == "/")
            {
                // Get the config homepage
                slug = cache.GetString("homepage");
            }

            if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != "en")
            {
                // We need to go and attempt to fetch the translation for the page
                // from either the cache, or from Google Translate
                bool bypassCache = context.Request.Query["bypass-cache"] == "true" || !_cachingOptions.Enabled;

                Dictionary<string, string[]> translations;
                cache.TryGetValue("translations", out translations);
                // Get our existing translations, if any, for this page
                if (translations == null)
                {
                    translations = new Dictionary<string, string[]>();
                }

                var response = context.Response;

                // Get our original body
                var originalBody = response.Body;

                // Setup our new body
                using var newBody = new MemoryStream();
                response.Body = newBody;

                // Run other middlewares
                await _next(context);

                // Grab the original body
                var stream = response.Body;
                using var reader = new StreamReader(stream, leaveOpen: true);
                stream.Seek(0, SeekOrigin.Begin);

                // Get our stream as a string
                var originalRepsonse = await reader.ReadToEndAsync();

                var responseBody = await cache.GetOrSetAsync(
                    $"translation:{slug}:{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}",
                    async () =>
                    {
                        try
                        {

                            // Now send our HTML through to Google
                            var client = AdvancedTranslationClient.CreateFromApiKey(_translationOptions.GoogleApiKey);

                            //Modify the response in some way (Example)
                            var result = await client.TranslateHtmlAsync(
                                html: originalRepsonse,
                                targetLanguage: CultureInfo.CurrentCulture.Name,
                                sourceLanguage: "en-GB");
                            originalRepsonse = result.TranslatedText;

                        }
                        catch
                        {
                            originalRepsonse = originalRepsonse?.Replace("Care", "Scare",
                                StringComparison.InvariantCultureIgnoreCase);
                        }

                        var cacheKeys = translations.GetValueOrDefault(slug);
                        if (cacheKeys == null)
                        {
                            cacheKeys = [];
                        }

                        if (!cacheKeys.Contains(
                                $"translation:{slug}:{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}"))
                        {
                            var list = cacheKeys.ToList();
                            list.Add($"translation:{slug}:{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}");
                            cacheKeys = list.ToArray();
                        }

                        translations[slug] = cacheKeys;

                        // Save back to the cache
                        await cache.SetAsync("translations", translations);


                        return originalRepsonse;
                    });

                stream.SetLength(0);
                using var writer = new StreamWriter(stream, leaveOpen: true);
                await writer.WriteAsync(responseBody);
                await writer.FlushAsync();
                response.ContentLength = stream.Length;


                newBody.Seek(0, SeekOrigin.Begin);
                await newBody.CopyToAsync(originalBody);
                response.Body = originalBody;

            }
            else
            {
                // Call the next delegate/middleware in the pipeline.
                await _next(context);
            }
        }
        else
        {
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }
    }

}

public static class GoogleTranslateMiddlewareExtensions
{
    public static IApplicationBuilder UseGoogleTranslate(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GoogleTranslateMiddleware>();
    }
}