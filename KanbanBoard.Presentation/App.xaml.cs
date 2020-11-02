using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Presentation.Properties;
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
            Settings.Default.PropertyChanged += SaveSettings;
            TextBoxHighlightBehavior.Initialize();
            base.OnStartup(e);
        }

        protected override void OnInitialized()
        {
            if (Container.Resolve<StartupService>().Initialize())
            {
                base.OnInitialized();
            }
            else
            {
                Current.Shutdown();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IRegistryService>(new RegistryService());
            containerRegistry.Register<IDialogService, DialogService>();
            containerRegistry.Register<IStartupService, StartupService>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<Board>();
        }

        private void SaveSettings(object sender, PropertyChangedEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}