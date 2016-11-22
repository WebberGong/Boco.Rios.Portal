using Boco.Rios.Portal.Framework.UI.Models;
using System.Xml;
using UIShell.Extension;
using UIShell.OSGi;

namespace Boco.Rios.Portal.Framework.UI.Extension
{
    public class MainMenuBuilder : BuilderBase<MenuItem>
    {
        public override void Build(XmlNode xmlNode, IBundle owner)
        {
            MenuItem newItem = MenuItemParser.Build(xmlNode, owner);
            Items.Add(newItem);
            OnItemAdded(newItem);
        }
    }
}