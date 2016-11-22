using System.Diagnostics;
using System.Web.Mvc;

namespace Core.Aop
{
    /// <summary>
    ///     模块拦截属性
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
            Trace.WriteLine("调用模块完成：" + _moduleName);
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Trace.WriteLine("开始调用模块：" + _moduleName);
        }

        public void OnException(ExceptionContext filterContext)
        {
            Trace.WriteLine("获取模块发生异常：" + _moduleName);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            Trace.WriteLine("获取模块完成：" + _moduleName);
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            Trace.WriteLine("开始获取模块：" + _moduleName);
        }
    }
}