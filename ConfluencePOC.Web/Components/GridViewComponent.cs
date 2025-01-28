using ConfluencePOC.Web.Models.Contentful;
using Microsoft.AspNetCore.Mvc;

namespace ConfluencePOC.Web.Components;

public class GridViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(Grid grid)
    {
        return View(grid);
    }
}