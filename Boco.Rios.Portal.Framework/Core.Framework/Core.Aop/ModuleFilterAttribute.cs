using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Core.Aop
{
    /// <summary>
    /// 模块拦截属性
    /// </summary>
    public class ModuleFilterAttribute : FilterAttribute, IActionFilter, IResultFilter, IExceptionFilter
    {
        private readonly string _moduleName;

        public ModuleFilterAttribute(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            System.Diagnostics.Trace.WriteLine("调用模块完成：" + _moduleName);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            System.Diagnostics.Trace.WriteLine("开始调用模块：" + _moduleName);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            System.Diagnostics.Trace.WriteLine("获取模块完成：" + _moduleName);
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            System.Diagnostics.Trace.WriteLine("开始获取模块：" + _moduleName);
        }

        public void OnException(ExceptionContext filterContext)
        {
            System.Diagnostics.Trace.WriteLine("获取模块发生异常：" + _moduleName);
        }
    }
}
