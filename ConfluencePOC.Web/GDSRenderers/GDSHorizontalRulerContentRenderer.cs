using Contentful.Core.Models;

namespace ConfluencePOC.Web.GDSRenderers;

/// <summary>
/// A content renderer that renders a horizontal ruler.
/// </summary>
public class GDSHorizontalRulerContentRenderer : IContentRenderer
{
    /// <summary>
    /// The order of this renderer in the collection.
    /// </summary>
    public int Order { get; set; } = 10;

    /// <summary>
    /// Whether or not this renderer supports the provided content.
    /// </summary>
    /// <param name="content">The content to evaluate.</param>
    /// <returns>Returns true if the content is a horizontal ruler, otherwise false.</returns>
    public bool SupportsContent(IContent content)
    {
        return content is HorizontalRuler;
    }

    /// <summary>
    /// Renders the content asynchronously.
    /// </summary>
    /// <param name="content">The content to render.</param>
    /// <returns>The rendered string.</returns>
    public Task<string> RenderAsync(IContent content)
    {
        return Task.FromResult("<hr class=\"govuk-section-break govuk-section-break--visible\">");
    }
}