using System.Windows;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Presentation.Services;
using KanbanBoard.Presentation.Views;
using Prism.Ioc;

namespace KanbanBoard.Presentation
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnInitialized()
        {
            TextBoxHighlightBehavior.Initialize();
            StartupHandling.Initialize(Container.Resolve<IDialogService>(), Container.Resolve<IRegistryService>());
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IRegistryService>(new RegistryService());
            containerRegistry.Register<IDialogService, DialogService>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Board>();
        }
    }
}