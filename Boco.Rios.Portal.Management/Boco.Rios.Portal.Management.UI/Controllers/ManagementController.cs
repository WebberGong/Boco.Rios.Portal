using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Core.Aop;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Boco.Rios.Portal.Management.UI.Filters;
using Boco.Rios.Portal.Management.UI.Models;

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
