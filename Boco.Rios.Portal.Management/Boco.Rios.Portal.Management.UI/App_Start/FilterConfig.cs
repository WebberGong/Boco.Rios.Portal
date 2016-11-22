using System.Web;
using System.Web.Mvc;

namespace Boco.Rios.Portal.Management.UI
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}