using Core.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
