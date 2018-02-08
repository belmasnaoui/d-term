﻿using Consoles.Core;
using System;

namespace UI.Wpf.Consoles
{
	public class ConsoleInstanceViewModel
	{
		private readonly IConsoleProcess _consoleProcess = null;
		private readonly ConsoleHwndHost _processHost = null;

		public ConsoleInstanceViewModel(IConsoleProcess consoleProcess)
		{
			_consoleProcess = consoleProcess ?? throw new ArgumentNullException(nameof(consoleProcess), nameof(ConsoleInstanceViewModel));
			_processHost = new ConsoleHwndHost(_consoleProcess);
		}

		public string Name { get; set; }

		public ConsoleHwndHost ProcessHost => _processHost;
	}
}