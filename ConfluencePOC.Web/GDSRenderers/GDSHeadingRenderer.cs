using Contentful.Core;
using Contentful.Core.Extensions;
using Contentful.Core.Models;
using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConfluencePOC.Web.GDSRenderers
{
    /// <summary>
    /// A renderer for a heading.
    /// </summary>
    public class GDSHeadingRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new HeadingRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public GDSHeadingRenderer(ContentRendererCollection rendererCollection)
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
        /// <returns>Returns true if the content is a heading, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Heading1 || content is Heading2 || content is Heading3 || content is Heading4 ||
                   content is Heading5 || content is Heading6;
        }

        /// <summary>
        /// Renders the content to an html h-tag.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The p-tag as a string.</returns>
        public async Task<string> RenderAsync(IContent content)
        {
            var headingSize = 1;
            var headingClass = "xl";

            switch (content)
            {
                case Heading1 _:
                    break;
                case Heading2 _:
                    headingSize = 2;
                    headingClass = "l";
                    break;
                case Heading3 _:
                    headingSize = 3;
                    headingClass = "m";
                    break;
                case Heading4 _:
                    headingSize = 4;
                    headingClass = "s";
                    break;
                case Heading5 _:
                    headingSize = 5;
                    headingClass = "s";
                    break;
                case Heading6 _:
                    headingSize = 6;
                    headingClass = "s";
                    break;
            }

            var heading = content as IHeading;

            var headingTag = new TagBuilder($"h{headingSize}");
            headingTag.AddCssClass($"govuk-heading-{headingClass}");

            foreach (var subContent in heading.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                headingTag.InnerHtml.AppendHtml(await renderer.RenderAsync(subContent));
            }
            headingTag.GenerateId(headingTag.InnerHtml.ToHtmlString(), "-");
            
            return headingTag.ToHtmlString();
        }
    }
}