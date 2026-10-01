using Microsoft.AspNetCore.Mvc.Razor;
using QuanLiQuanCaffee.Models;

public class GlobalViewDataExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    public IEnumerable<string> ExpandViewLocations(
        ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        return viewLocations;
    }
}