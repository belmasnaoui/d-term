﻿using Consoles.Core;
using Consoles.Data.LiteDB;
using Consoles.Processes;
using Notebook.Core;
using Notebook.Data.LiteDB;
using ReactiveUI;
using Splat;
using System.Reflection;
using UI.Wpf.Consoles;
using UI.Wpf.Mappings;
using UI.Wpf.Settings;
using UI.Wpf.Shell;

namespace UI.Wpf
{
	/// <summary>
	/// Main app container bootstrapper.
	/// </summary>
	public static class AppBootstrap
	{
		//
		private static IMutableDependencyResolver _container;

		/// <summary>
		/// Constructor method.
		/// </summary>
		static AppBootstrap()
		{
			Locator.CurrentMutable.InitializeSplat();
			Locator.CurrentMutable.InitializeReactiveUI();

			_container = Locator.CurrentMutable;
		}

		/// <summary>
		/// Constructor method.
		/// </summary>
		public static void Initialize()
		{
			var dbConnectionString = @"dTerm.db";

			//
			_container.Register(() => new MapperProfileConsoles());
			_container.Register(() => new MapperProfileNotebooks());

			//
			_container.Register<IProcessTracker>(() => new ProcessTracker());
			_container.Register<IProcessPathBuilder>(() => new ProcessPathBuilder());

			//
			_container.Register<IConsoleOptionsRepository>(() => new ConsoleOptionsRepository(dbConnectionString));
			_container.Register<INotebooksRepository>(() => new NotebooksRepository(dbConnectionString));

			//
			_container.Register<IConsoleProcessService>(() => new ConsoleProcessService());
			_container.Register<IConsolesPanelViewModel>(() => new ConsolesPanelViewModel());
			_container.Register<IConsoleConfigsViewModel>(() => new ConsoleConfigsViewModel());
			_container.Register<IConsoleOptionFormViewModel>(() => new ConsoleOptionFormViewModel());
			_container.Register<IConsoleOptionViewModel>(() => new ConsoleOptionViewModel());

			//
			_container.Register<ISettingsViewModel>(() => new SettingsViewModel());

			//
			_container.Register<IShellViewModel>(() => new ShellViewModel());

			//
			_container.RegisterViewsForViewModels(Assembly.GetExecutingAssembly());
		}
	}
}
