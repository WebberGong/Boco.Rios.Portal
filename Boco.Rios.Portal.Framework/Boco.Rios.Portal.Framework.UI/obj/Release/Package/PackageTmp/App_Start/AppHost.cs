using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using ServiceStack;
using ServiceStack.Configuration;
using ServiceStack.Metadata;
using ServiceStack.Mvc;
using ServiceStack.Razor.Compilation;
using System.Xml;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Boco.Rios.Portal.Framework.UI.AppHost), "Start")]
namespace Boco.Rios.Portal.Framework.UI
{
    public class AppHost : AppHostBase
    {
        private static readonly DirectoryCatalog _catalog = new DirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));
        private static readonly CompositionContainer _container = new CompositionContainer(_catalog);
        private static readonly IList<Assembly> _serviceAssembly = new List<Assembly>();
        private static Dictionary<string, string> _serviceUrl = new Dictionary<string, string>();

        private AppHost()
            : base("Boco.Rios.Portal.Service", _serviceAssembly.ToArray())
        {
        }

        public static Dictionary<string, string> ServiceUrl
        {
            get
            {
                return _serviceUrl;
            }
        }

        public override void Configure(Funq.Container container)
        {
            SetConfig(new HostConfig()
            {
                 HandlerFactoryPath = "api"
            });
        }

        public static void Start()
        {
            FillServiceAssembly();
            FillServiceUrl();

            var host = new AppHost();
            host.Init();
        }

        private static void FillServiceAssembly()
        {
            var services = _container.GetExports<Service>();
            foreach (Lazy<Service> service in services)
            {
                _serviceAssembly.Add(service.Value.GetType().Assembly);
            }
        }

        private static void FillServiceUrl()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceConfig.xml"));
            XmlNodeList serviceConfig = doc.GetElementsByTagName("ServiceConfig");
            foreach (XmlNode node in serviceConfig)
            {
                string key = "";
                string value = "";
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name == "ServiceName")
                    {
                        key = childNode.InnerText;
                    }
                    if (childNode.Name == "Url")
                    {
                        value = childNode.InnerText;
                    }
                }
                if (!string.IsNullOrEmpty(key) && !_serviceUrl.ContainsKey(key))
                {
                    _serviceUrl.Add(key, value);
                }
            }
        }
    }
}
