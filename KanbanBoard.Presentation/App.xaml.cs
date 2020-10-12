using System.Windows;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Presentation.Views;
using Prism.Ioc;
using Prism.Unity;

namespace KanbanBoard.Presentation
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            TextBoxHighlightBehavior.Initialize();

            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //throw new System.NotImplementedException();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Board>();
        }
    }
}