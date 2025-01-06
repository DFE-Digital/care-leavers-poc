using System.ComponentModel;
using Contentful.Core.Models;

namespace ConfluencePOC.Web.Models.Contentful;

public class RowContent : ContentfulContent
{
    public static string ContentType { get; } = "rowContent";

    public string Title  { get; set; }
    
    public string Width { get; set; }
    
    public Document? Content { get; set; }
    
    public Grid? Grid { get; set; }

}