using System.Globalization;
using System.Text;
using Azure;
using ConfluencePOC.Web.Configuration;
using Azure.AI.Translation.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ConfluencePOC.Web.Middleware;

public class AzureTranslateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly TranslationOptions _options;

    public AzureTranslateMiddleware(RequestDelegate next, IOptions<TranslationOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context,  IDistributedCache cache)
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
        
        
        if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName != "en")
        {
            // We need to go and attempt to fetch the translation for the page
            // from either the cache, or from Google Translate
            
            // Get our existing body
            using (var responseBodyStream = new MemoryStream())
            {
                var bodyStream = context.Response.Body;

                try
                {
                    
                    context.Response.Body = responseBodyStream;

                    await _next(context);

                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
                    
                    // Now send our HTML through to Azure
                    var translateOptions =
                        new TextTranslationTranslateOptions(targetLanguage: CultureInfo.CurrentCulture.Name,
                            responseBody)
                        {
                            TextType = TextType.Html
                        };

                    var client = new TextTranslationClient(new AzureKeyCredential(_options.AzureApiKey));
                    
                    //Modify the response in some way (Example)
                    var result = (await client.TranslateAsync(translateOptions)).Value;
                    var translation = result.FirstOrDefault();
                    responseBody = translation?.Translations?.FirstOrDefault()?.Text;

                    using (var newStream = new MemoryStream())
                    {
                        var sw = new StreamWriter(newStream);
                        sw.Write(responseBody);
                        sw.Flush();

                        newStream.Seek(0, SeekOrigin.Begin);

                        await newStream.CopyToAsync(bodyStream);
                    }
                }
                finally
                {
                    context.Response.Body = bodyStream;
                }
            }
            
            
            return;
        }
        

        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }
}

public static class AzureTranslateMiddlewareExtensions
{
    public static IApplicationBuilder UseAzureTranslate(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AzureTranslateMiddleware>();
    }
}