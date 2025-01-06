using System.Runtime.Serialization;

namespace ConfluencePOC.Web.Models;

public class Navigation
{
    public List<Link> Links { get; set; }
}

public class Link
{
    public string Title { get; set; }
    public string Slug { get; set; }
}