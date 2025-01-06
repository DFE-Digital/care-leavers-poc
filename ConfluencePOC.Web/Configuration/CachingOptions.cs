namespace ConfluencePOC.Web.Configuration;

public class CachingOptions
{
    public const string Name = "Caching";
    public bool UseRedis { get; set; } = false;
    public string ConnectionString { get; set; } = String.Empty;
    public string InstanceName { get; set; } = String.Empty;

    public void Validate()
    {
        if (UseRedis)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException(nameof(ConnectionString), "Missing Caching:ConnectionString in appsettings");
            }
            
            if (string.IsNullOrEmpty(InstanceName))
            {
                throw new ArgumentNullException(nameof(InstanceName), "Missing Caching:InstanceName in appsettings");
            }
        }
    }

}