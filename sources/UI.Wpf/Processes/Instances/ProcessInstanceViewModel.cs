﻿using Processes.Core;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace UI.Wpf.Processes
{
	//
	public interface IProcessInstanceViewModel
	{
		string Name { get; set; }
		bool IsConsole { get; set; }
		int ProcessId { get; }
		IProcessHost ProcessHost { get; }
		IntPtr ProcessMainModuleHandle { get; }
		IntPtr ProcessMainWindowHandle { get; }
		uint ProcessThreadId { get; }
		IObservable<EventPattern<EventArgs>> ProcessExited { get; }
		void TerminateProcess();
	}

	//
	public class ProcessInstanceViewModel : ReactiveObject, IProcessInstanceViewModel
	{
		//
		private readonly IProcess _process;
		private readonly IProcessHostFactory _processHostFactory;
		private readonly IObservable<EventPattern<EventArgs>> _terminated;

		//
		private string _name;
		private bool _isConsole;
		private IProcessHost _processHost;

		/// <summary>
		/// Constructor method.
		/// </summary>
		public ProcessInstanceViewModel(IProcess process, IProcessHostFactory processHostFactory)
		{
			_process = process ?? throw new ArgumentNullException(nameof(process), nameof(ProcessInstanceViewModel));
			_processHostFactory = processHostFactory ?? throw new ArgumentNullException(nameof(processHostFactory), nameof(ProcessInstanceViewModel));

			_terminated = Observable.FromEventPattern<EventHandler, EventArgs>(
				handler => _process.Exited += handler,
				handler => _process.Exited -= handler);
		}

		public string Name
		{
			get => _name;
			set => this.RaiseAndSetIfChanged(ref _name, value);
		}

		public bool IsConsole
		{
			get => _isConsole;
			set => this.RaiseAndSetIfChanged(ref _isConsole, value);
		}

		public IProcessHost ProcessHost
		{
			get
			{
				if (_processHost == null)
				{
					_processHost = _processHostFactory.Create(_process);
				}

				return _processHost;
			}
		}

		public int ProcessId => _process.Id;

		public IntPtr ProcessMainModuleHandle => _process.MainModuleHandle;

		public IntPtr ProcessMainWindowHandle => _process.MainWindowHandle;

		public uint ProcessThreadId => _process.ThreadId;

		public IObservable<EventPattern<EventArgs>> ProcessExited => _terminated;

		public void TerminateProcess()
		{
			_process.Stop();
		}
	}
}
