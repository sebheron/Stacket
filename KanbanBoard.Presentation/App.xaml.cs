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
        protected override void OnStartup(StartupEventArgs e)
        {
            TextBoxHighlightBehavior.Initialize();

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IDialogService>(new DialogService());
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Board>();
        }
    }
}