﻿namespace Processes.Core
{
	public interface IProcessTracker
	{
		void KillAll();
		void Track(int processId);
	}
}