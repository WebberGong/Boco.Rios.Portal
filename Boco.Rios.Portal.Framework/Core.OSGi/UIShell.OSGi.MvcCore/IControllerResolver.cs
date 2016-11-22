using System;

namespace UIShell.OSGi.MvcCore
{
    public interface IControllerResolver
    {
        object Resolve(Type type);
    }
}