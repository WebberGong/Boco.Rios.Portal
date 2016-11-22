using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Boco.Rios.Portal.Framework.UI;
using Boco.Rios.Portal.Framework.UI.Extension;
using Boco.Rios.Portal.Framework.UI.Models;
using ServiceStack.MiniProfiler;
using UIShell.Extension;
using UIShell.OSGi;
using UIShell.OSGi.Core.Service;
using UIShell.OSGi.MvcCore;

[assembly: PreApplicationStartMethod(typeof (MvcApplication), "StartBundleRuntime")]

namespace Boco.Rios.Portal.Framework.UI
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private ExtensionHooker _extensionHooker;

        private ApplicationViewModel ViewModel { get; set; }

        public static void StartBundleRuntime()
        {
            var bootstapper = new Bootstrapper();
            bootstapper.StartBundleRuntime();
        }

        private void MonitorExtension()
        {
            ViewModel = new ApplicationViewModel();

            ViewModel.MainMenuItems.Add(new MenuItem
            {
                Text = "主页",
                Url = "/"
            });
            BundleRuntime.Instance.AddService<ApplicationViewModel>(ViewModel);
            _extensionHooker = new ExtensionHooker(BundleRuntime.Instance.GetFirstOrDefaultService<IExtensionManager>());
            _extensionHooker.HookExtension("MainMenu", new MainMenuExtensionHandler(ViewModel));
        }

        protected void Application_Start()
        {
            ViewEngines.Engines.Add(new BundleRuntimeViewEngine(new BundleRazorViewEngineFactory()));
            ViewEngines.Engines.Add(new BundleRuntimeViewEngine(new BundleWebFormViewEngineFactory()));

            //WebApiConfig.Register(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            MonitorExtension();
        }

        protected void Application_BeginRequest(object src, EventArgs e)
        {
            if (Request.IsLocal)
                Profiler.Start();
        }

        protected void Application_EndRequest(object src, EventArgs e)
        {
            Profiler.Stop();
        }
    }
}