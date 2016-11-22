using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Boco.Rios.Portal.Management.Service;
using Boco.Rios.Portal.Management.UI;
using Funq;
using ServiceStack.WebHost.Endpoints;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (AppHost), "Start")]

namespace Boco.Rios.Portal.Management.UI
{
    public class AppHost : AppHostBase
    {
        private static readonly Dictionary<string, string> serviceUrl = new Dictionary<string, string>();

        private AppHost()
            : base("Boco.Rios.Portal.Service", typeof (ManagementService).Assembly)
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
            FillServiceUrl();

            var host = new AppHost();
            host.Init();
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