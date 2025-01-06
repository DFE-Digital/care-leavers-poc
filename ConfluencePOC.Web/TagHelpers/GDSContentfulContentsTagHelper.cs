using ConfluencePOC.Web.Models.Contentful;
using Contentful.AspNetCore.TagHelpers;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ConfluencePOC.Web.TagHelpers;

[HtmlTargetElement("gds-contentful-contents", TagStructure = TagStructure.WithoutEndTag)]
public class GDSContentfulContentsTagHelper : TagHelper
{
    private readonly HtmlRenderer _renderer;
    
    /// <summary>
    /// The document to render.
    /// </summary>
    public Document? Document { get; set; }
    
    public List<Grid>? Grids { get; set; }

    public string Levels { get; set; } = "2";
    
    public GDSContentfulContentsTagHelper(HtmlRenderer renderer)
    {
        _renderer = renderer;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        // Set this up as a navigation element
        output.TagName = "";

        TagBuilder contents = new TagBuilder("nav");
        TagBuilder list = new TagBuilder("ul");
        list.AddCssClass("govuk-list");
        
        // Get our rendered HTML content
        var html = new HtmlDocument();
        html.LoadHtml(await _renderer.ToHtml(Document));

        var headingsList = new List<string>();
        
        foreach (var level in Levels.Split(",", StringSplitOptions.RemoveEmptyEntries))
        {
            headingsList.Add("self::h" + level);
        }
        
        // Get our list of heading tags with IDs and their text
        var headings = html.DocumentNode.SelectNodes($"//*[({string.Join(" or ", headingsList)}) and @id!=\"\"]");

        if (headings != null)
        {
            
            // Loop through
            foreach (var heading in headings)
            {
                TagBuilder item = new TagBuilder("li");
                item.AddCssClass("govuk-body-s");
                item.InnerHtml.AppendHtml($"<a href=\"#{heading.Id}\">{heading.InnerHtml}</a>");
                list.InnerHtml.AppendHtml(item);
            }

            

           
        }
        
        // Loop through our grid if we have one
        if (Grids != null && Grids.Count != 0)
        {
            foreach (var grid in Grids)
            {
                if (grid.ShowTitle)
                {
                    TagBuilder item = new TagBuilder("li");
                    item.AddCssClass("govuk-body-s");
                    item.InnerHtml.AppendHtml($"<a href=\"#{TagBuilder.CreateSanitizedId(grid.Title, "-")}\">{grid.Title}</a>");
                    list.InnerHtml.AppendHtml(item);
                }
            }
        }

        if (list.HasInnerHtml)
        {
            contents.InnerHtml.AppendHtml("<h2 class=\"govuk-heading-s\">Contents</h2>");
            contents.InnerHtml.AppendHtml(list);
        }
        output.Content.SetHtmlContent(contents);
    }
}