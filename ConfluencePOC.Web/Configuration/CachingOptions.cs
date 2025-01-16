namespace ConfluencePOC.Web.Configuration;

public sealed class CachingOptions
{
    public const string Name = "Caching";
    public bool Enabled { get; set; } = true;
    public bool UseRedis { get; set; } = false;
    public string? ConnectionString { get; set; }
    public string? InstanceName { get; set; }
    public TimeSpan Duration { get; set; } = TimeSpan.FromDays(365);

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
