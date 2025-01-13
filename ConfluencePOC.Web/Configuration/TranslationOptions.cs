namespace ConfluencePOC.Web.Configuration;

public sealed class TranslationOptions
{
    public const string Name = "Translation";

    public enum TranslationMethod
    {
        None,
        Azure,
        Google
    }

    public TranslationMethod Method { get; set; } = TranslationMethod.None;

    public string GoogleApiKey { get; set; } = String.Empty;
    public string AzureApiKey { get; set; } = String.Empty;

    public void Validate()
    {
        switch (Method)
        {
            case TranslationMethod.Azure:
                if (string.IsNullOrEmpty(AzureApiKey))
                {
                    throw new ArgumentNullException(nameof(AzureApiKey),
                        "Missing Translation:AzureApiKey in appsettings");
                }

                break;
            case TranslationMethod.Google:
                if (string.IsNullOrEmpty(GoogleApiKey))
                {
                    throw new ArgumentNullException(nameof(GoogleApiKey),
                        "Missing Translation:GoogleApiKey in appsettings");
                }

                break;
        }
    }

}
