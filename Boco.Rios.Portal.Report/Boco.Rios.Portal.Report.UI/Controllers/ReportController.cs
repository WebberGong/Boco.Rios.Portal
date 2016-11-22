using System.Web.Mvc;
using Core.Aop;

namespace Boco.Rios.Portal.Report.UI.Controllers
{
    public class ReportController : Controller
    {
        [ModuleFilter("报表分析")]
        public ActionResult Report()
        {
            return View();
        }
    }
}