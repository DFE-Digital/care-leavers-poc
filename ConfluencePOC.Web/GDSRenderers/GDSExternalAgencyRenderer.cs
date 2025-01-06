using ConfluencePOC.Web.Models.Contentful;
using ConfluencePOC.Web.TagHelpers;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A renderer for a paragraph.
/// </summary>
public class GDSExternalAgencyRenderer : IContentRenderer
{
    private readonly ContentRendererCollection _rendererCollection;

    public enum CardType { Card, AlternatingImageAndText }

    public CardType Type { get; set; } = CardType.Card;
    

    /// <summary>
    /// Initializes a new PragraphRenderer
    /// </summary>
    /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
    public GDSExternalAgencyRenderer(ContentRendererCollection rendererCollection)
    {
        _rendererCollection = rendererCollection;
    }

    /// <summary>
    /// The order of this renderer in the collection.
    /// </summary>
    public int Order { get; set; } = 10;

    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is a paragraph, otherwise false.</returns>
    public bool SupportsContent(IContent content)
    {
        if (content is EntryStructure)
        {
            var structure = content as EntryStructure;
            if (structure.NodeType == "embedded-entry-block")
            {
                if (structure.Data.Target is ExternalAgency)
                    return true;
            }
        }

        return content is ExternalAgency;
    }

    /// <summary>
    /// Renders the content to an html p-tag.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The p-tag as a string.</returns>
    public async Task<string> RenderAsync(IContent content)
    {
        ExternalAgency externalAgency;
        if (content is ExternalAgency)
        {
            externalAgency = content as ExternalAgency;
        }
        else
        {
            externalAgency = (content as EntryStructure).Data.Target as ExternalAgency;
        }

        var box = new TagBuilder("div");
        box.AddCssClass("dfe-box box-ext");

        // Logo
        var icon = new TagBuilder("div");
        icon.AddCssClass("box-icon");
        if (externalAgency.Logo != null)
        {
            icon.InnerHtml.AppendHtml($"<img class=\"full-width-image\" src=\"{externalAgency.Logo.File.Url}\" alt=\"{externalAgency.Logo.Title}\" />");

        }
        box.InnerHtml.AppendHtml(icon);

        // Main Content
        var boxContent = new TagBuilder("div");
        boxContent.AddCssClass("box-content");

        // Name and hyperlink
        var name = new TagBuilder("h3");
        name.AddCssClass("govuk-heading-s");
        var link = new TagBuilder("a");
        link.Attributes.Add("target", "_blank");
        link.Attributes.Add("href", externalAgency.Url);
        link.InnerHtml.Append(externalAgency.Name);
        name.InnerHtml.AppendHtml(link);
        name.InnerHtml.AppendHtml("<span class =\"govuk-body-s\">(Opens in a new tab)</span>");
        boxContent.InnerHtml.AppendHtml(name);
           
        // Description
        var paragraph = new TagBuilder("p");
        paragraph.AddCssClass("govuk-body-s");
        paragraph.InnerHtml.Append(externalAgency.Description);
        boxContent.InnerHtml.AppendHtml(paragraph);
            
        // Further details
        paragraph.InnerHtml.Clear();
        paragraph.InnerHtml.AppendHtml($"<strong>Call:</strong>{externalAgency.Call}");
        boxContent.InnerHtml.AppendHtml(paragraph);
        
        paragraph.InnerHtml.Clear();
        paragraph.InnerHtml.AppendHtml($"<strong>Opening:</strong>{externalAgency.OpeningTimes}");
        boxContent.InnerHtml.AppendHtml(paragraph);
        
        paragraph.InnerHtml.Clear();paragraph.InnerHtml.AppendHtml($"<strong>Free:</strong>{(externalAgency.Free ? "Yes" : "No")}");
        boxContent.InnerHtml.AppendHtml(paragraph);
        
        paragraph.InnerHtml.Clear();paragraph.InnerHtml.AppendHtml($"<strong>Accessibility:</strong>{externalAgency.Accessibility}");
        boxContent.InnerHtml.AppendHtml(paragraph);
        
        box.InnerHtml.AppendHtml(boxContent);

        return box.ToHtmlString();

    }
}