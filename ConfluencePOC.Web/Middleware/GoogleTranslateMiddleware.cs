using System.Globalization;
using System.Text;
using Google.Cloud.Translation.V2;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ConfluencePOC.Web.Middleware;

public class GoogleTranslateMiddleware
{
    private readonly RequestDelegate _next;

    public GoogleTranslateMiddleware(RequestDelegate next)
    {
        _next = next;
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
                    
                    // Now send our HTML through to Google
                    var client = AdvancedTranslationClient.CreateFromApiKey(Configuration.Options.GoogleApiKey);
                    
                    //Modify the response in some way (Example)
                    var result = await client.TranslateHtmlAsync(
                        html: responseBody, 
                        targetLanguage: CultureInfo.CurrentCulture.Name, 
                        sourceLanguage: "en-GB");
                    responseBody = result.TranslatedText;

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

public static class GoogleTranslateMiddlewareExtensions
{
    public static IApplicationBuilder UseGoogleTranslate(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GoogleTranslateMiddleware>();
    }
}