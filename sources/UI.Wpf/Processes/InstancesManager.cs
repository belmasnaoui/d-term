﻿using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using UI.Wpf.Properties;
using WinApi.User32;

namespace UI.Wpf.Processes
{
	public interface IInstancesManager
	{
		IReactiveDerivedList<IInstanceViewModel> GetAllInstances();
		IReactiveDerivedList<IInstanceViewModel> GetMinimizedInstances();
		void Track(IInstanceViewModel instance);
	}

	public class InstancesManager : ReactiveObject, IInstancesManager
	{
		private readonly IntPtr _shellViewHandle;
		private readonly IReactiveList<IInstanceViewModel> _instancesSource;
		private readonly IReactiveDerivedList<IInstanceViewModel> _allInstancesList;
		private readonly IReactiveDerivedList<IInstanceViewModel> _minimizedInstancesList;

		private Dictionary<int, (WinEventDelegate hookDelegate, IntPtr hookHandle)> _processHooksTracker;

		/// <summary>
		/// Constructor method.
		/// </summary>
		public InstancesManager(IntPtr shellViewHandle)
		{
			_shellViewHandle = shellViewHandle;
			_processHooksTracker = new Dictionary<int, (WinEventDelegate hookDelegate, IntPtr hookHandle)>();

			_instancesSource = new ReactiveList<IInstanceViewModel>()
			{
				ChangeTrackingEnabled = true
			};

			_instancesSource.ItemsAdded.Subscribe(instance => Integrate(instance));
			_instancesSource.ItemsRemoved.Subscribe(instance => Release(instance));

			_allInstancesList = _instancesSource.CreateDerivedCollection(
				selector: instance => instance
			);

			_minimizedInstancesList = _instancesSource.CreateDerivedCollection(
				filter: instance => instance.IsMinimized,
				selector: instance => instance
			);
		}

		public IReactiveDerivedList<IInstanceViewModel> GetAllInstances() => _allInstancesList;

		public IReactiveDerivedList<IInstanceViewModel> GetMinimizedInstances() => _minimizedInstancesList;

		public void Track(IInstanceViewModel instance)
		{
			var subscription = instance.ProcessTerminated.ObserveOnDispatcher().Subscribe(@event =>
			{
				_instancesSource.Remove(instance);
			});

			_instancesSource.Add(instance);
		}

		private void Integrate(IInstanceViewModel instance)
		{
			var iconHandle = Resources.dTermIcon.Handle;
			var instanceHandle = instance.ProcessMainWindowHandle;

			Win32Api.HideFromTaskbar(instanceHandle);
			Win32Api.MakeLayeredWindow(instanceHandle);
			Win32Api.SetProcessWindowIcon(instanceHandle, iconHandle);
			Win32Api.SetProcessWindowOwner(instanceHandle, _shellViewHandle);

			SetHooks(instance.ProcessId, instanceHandle);
		}

		private void SetHooks(int processId, IntPtr instanceHandle)
		{
			var hookProc = new WinEventDelegate(WinEventProc);

			var hookHandle = Win32Api.AddMinimizeEventHook(instanceHandle, hookProc);

			if (hookHandle != IntPtr.Zero)
			{
				_processHooksTracker.Add(processId, (hookProc, hookHandle));
			}
		}

		public void Release(IInstanceViewModel instance)
		{
			var hookData = _processHooksTracker[instance.ProcessId];
			_processHooksTracker.Remove(instance.ProcessId);
			Win32Api.RemoveHook(hookData.hookHandle);
		}

		private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			if (idObject != 0 || idChild != 0)
			{
				return;
			}

			switch (eventType)
			{
				case 0x0016:
					var instance = _instancesSource.Where(i => i.ProcessMainWindowHandle == hwnd).SingleOrDefault();
					if (instance != null)
					{
						instance.IsMinimized = true;
						User32Methods.ShowWindow(instance.ProcessMainWindowHandle, ShowWindowCommands.SW_HIDE);
					}
					break;
			}

			//if (eventType == EVENT_OBJECT_NAMECHANGE) Console.WriteLine("Text of hwnd changed {0:x8}", hwnd.ToInt32());
			//if (eventType == EVENT_SYSTEM_MINIMIZESTART) Console.WriteLine("Minimization detected for {0:x8}", hwnd.ToInt32());
			//if (eventType == EVENT_SYSTEM_FOREGROUND) Console.WriteLine("Foreground change detected for {0:x8}", hwnd.ToInt32());
		}
	}
}