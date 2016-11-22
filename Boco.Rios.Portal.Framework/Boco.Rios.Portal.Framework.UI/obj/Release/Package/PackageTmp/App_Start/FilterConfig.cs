using System.Web;
using System.Web.Mvc;

namespace Boco.Rios.Portal.Framework.UI
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}