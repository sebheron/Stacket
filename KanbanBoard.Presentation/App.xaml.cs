using System;
using System.ComponentModel;
using System.Windows;
using KanbanBoard.Presentation.Behaviors;
using KanbanBoard.Logic.Properties;
using KanbanBoard.Presentation.Factories;
using KanbanBoard.Presentation.Services;
using KanbanBoard.Presentation.Views;
using Prism.Ioc;
using Prism.Logging;
using Prism.Events;

namespace KanbanBoard.Presentation
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += ExceptionHandler;
            Settings.Default.PropertyChanged += SaveSettings;
            TextBoxHighlightBehavior.Initialize();
            base.OnStartup(e);
        }

        private void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            if (args.ExceptionObject is Exception e)
            {
                MainWindow.Hide();
                var logger = Container.Resolve<ILoggerFacade>() as StringLogger;
                logger.Log(e.Message + e.StackTrace, Category.Exception, Priority.None);
                Container.Resolve<CrashService>().SendCrash(logger.Builder.ToString());
            }
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
            containerRegistry.RegisterInstance<ILoggerFacade>(new StringLogger());
            containerRegistry.RegisterInstance<IFTPService>(new FTPService(Container.Resolve<IEventAggregator>()));
            containerRegistry.Register<IRegistryService, RegistryService>();
            containerRegistry.Register<IDialogService, DialogService>();
            containerRegistry.Register<IStartupService, StartupService>();
            containerRegistry.Register<ICrashService, CrashService>();
            containerRegistry.Register<IItemViewModelFactory, ItemViewModelFactory>();
            containerRegistry.Register<IColumnViewModelFactory, ColumnViewModelFactory>();
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