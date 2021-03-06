﻿using System.Web.Mvc;
using System.Web.Routing;

namespace Boco.Rios.Portal.HomePage.UI
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("api/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "HomePage", action = "HomePage", id = UrlParameter.Optional}
                );
        }
    }
}