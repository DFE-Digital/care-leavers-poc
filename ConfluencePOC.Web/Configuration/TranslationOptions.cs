namespace ConfluencePOC.Web.Configuration;

public class TranslationOptions
{
    public enum TranslationMethod
    {
        Azure,
        Google,
        None
    }

    private TranslationMethod _method;
    
    public const string Name = "Translation";

    public string Method
    {
        get
        {
            return _method.ToString();
        }
        set
        {
            Enum.TryParse<TranslationMethod>(value: value, ignoreCase: true, out _method);
        }
    }

    public TranslationMethod TranslateMethod => _method;

    public string GoogleApiKey { get; set; } = String.Empty;
    public string AzureApiKey { get; set; } = String.Empty;

    public void Validate()
    {
        switch (_method)
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
