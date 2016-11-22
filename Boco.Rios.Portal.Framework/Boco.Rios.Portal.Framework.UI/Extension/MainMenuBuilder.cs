using System.Xml;
using Boco.Rios.Portal.Framework.UI.Models;
using UIShell.Extension;
using UIShell.OSGi;

namespace Boco.Rios.Portal.Framework.UI.Extension
{
    public class MainMenuBuilder : BuilderBase<MenuItem>
    {
        public override void Build(XmlNode xmlNode, IBundle owner)
        {
            var newItem = MenuItemParser.Build(xmlNode, owner);
            Items.Add(newItem);
            OnItemAdded(newItem);
        }
    }
}