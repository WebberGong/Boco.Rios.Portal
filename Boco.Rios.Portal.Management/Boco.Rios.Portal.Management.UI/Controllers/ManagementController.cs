using System.Web.Mvc;
using Core.Aop;

namespace Boco.Rios.Portal.Management.UI.Controllers
{
    public class ManagementController : Controller
    {
        [ModuleFilter("公告管理")]
        public ActionResult NoticeManagement()
        {
            return View();
        }
    }
}