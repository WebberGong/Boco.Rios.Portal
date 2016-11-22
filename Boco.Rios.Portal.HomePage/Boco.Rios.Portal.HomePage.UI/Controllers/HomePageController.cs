using System.Web.Mvc;
using Core.Aop;

namespace Boco.Rios.Portal.HomePage.UI.Controllers
{
    public class HomePageController : Controller
    {
        [ModuleFilter("小区查询")]
        public ActionResult QueryCells()
        {
            return View();
        }

        [ModuleFilter("门户首页")]
        public ActionResult HomePage()
        {
            return View();
        }
    }
}