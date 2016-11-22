using Boco.Rios.Portal.Framework.UI.Models;
using UIShell.Extension;

namespace Boco.Rios.Portal.Framework.UI.Extension
{
    internal class MainMenuExtensionHandler : ExtensionChangeHandler
    {
        private readonly ApplicationViewModel _viewModel;

        public MainMenuExtensionHandler(ApplicationViewModel viewModel)
        {
            _viewModel = viewModel;
            Initialize(new MainMenuBuilder(), OnNewItem, OnRemoveItem);
        }

        private void OnNewItem(object item)
        {
            _viewModel.MainMenuItems.Add((MenuItem) item);
        }

        private void OnRemoveItem(object item)
        {
            _viewModel.MainMenuItems.Remove((MenuItem) item);
        }
    }
}