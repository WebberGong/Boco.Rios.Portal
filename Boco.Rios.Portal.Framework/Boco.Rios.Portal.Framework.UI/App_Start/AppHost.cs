using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Boco.Rios.Portal.Framework.UI;
using Funq;
using ServiceStack.ServiceInterface;
using ServiceStack.WebHost.Endpoints;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (AppHost), "Start")]

namespace Boco.Rios.Portal.Framework.UI
{
    public class AppHost : AppHostBase
    {
        private static readonly DirectoryCatalog catalog =
            new DirectoryCatalog(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));

        private static readonly CompositionContainer container = new CompositionContainer(catalog);
        private static readonly IList<Assembly> serviceAssembly = new List<Assembly>();
        private static readonly Dictionary<string, string> serviceUrl = new Dictionary<string, string>();

        private AppHost()
            : base("Boco.Rios.Portal.Service", serviceAssembly.ToArray())
        {
        }

        public static Dictionary<string, string> ServiceUrl
        {
            get { return serviceUrl; }
        }

        public override void Configure(Container container)
        {
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
            var services = container.GetExports<Service>("Service");
            foreach (var service in services)
            {
                serviceAssembly.Add(service.Value.GetType().Assembly);
            }
        }

        private static void FillServiceUrl()
        {
            var doc = new XmlDocument();
            doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceConfig.xml"));
            var serviceConfig = doc.GetElementsByTagName("ServiceConfig");
            foreach (XmlNode node in serviceConfig)
            {
                var key = "";
                var value = "";
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
                if (!string.IsNullOrEmpty(key) && !serviceUrl.ContainsKey(key))
                {
                    serviceUrl.Add(key, value);
                }
            }
        }
    }
}