using System.Web.Mvc;
using Boco.Rios.Portal.Framework.UI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boco.Rios.Portal.Framework.UI.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // 排列
            var controller = new HomeController();

            // 操作
            var result = controller.Index() as ViewResult;

            // 断言
            Assert.AreEqual("修改此模板以快速启动你的 ASP.NET MVC 应用程序。", result.ViewBag.Message);
        }

        [TestMethod]
        public void About()
        {
            // 排列
            var controller = new HomeController();

            // 操作
            var result = controller.About() as ViewResult;

            // 断言
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Contact()
        {
            // 排列
            var controller = new HomeController();

            // 操作
            var result = controller.Contact() as ViewResult;

            // 断言
            Assert.IsNotNull(result);
        }
    }
}