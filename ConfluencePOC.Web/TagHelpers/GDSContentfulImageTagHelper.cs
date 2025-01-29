using Contentful.AspNetCore.TagHelpers;
using Contentful.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ConfluencePOC.Web.TagHelpers;

[HtmlTargetElement("gds-contentful-image", TagStructure = TagStructure.WithoutEndTag)]
public class GDSContentfulImageTagHelper : ContentfulImageTagHelper
{
    public string Class { get; set; } = "full-width-image";
    
    public string Style { get; set; } = String.Empty;
    
    public GDSContentfulImageTagHelper(IContentfulClient client) : base(client)
    {
       
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("class", Class);
        output.Attributes.SetAttribute("style", Style);
        base.Process(context, output);
    }

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.SetAttribute("class", Class);
        output.Attributes.SetAttribute("style", Style);
        return base.ProcessAsync(context, output);
    }
}