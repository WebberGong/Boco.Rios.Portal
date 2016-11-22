using System.Collections.Generic;

namespace Boco.Rios.Portal.Framework.UI.Models
{
    public class ApplicationViewModel
    {
        public ApplicationViewModel()
        {
            MainMenuItems = new List<MenuItem>();
        }

        public List<MenuItem> MainMenuItems { get; private set; }
    }
}