﻿using System;

namespace Processes.Core
{
	public class ProcessEntity
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int OrderIndex { get; set; }
		public string PicturePath { get; set; }
		public ProcessType Type { get; set; }
		public ProcessBasePath ProcessBasePath { get; set; }
		public string ProcessExecutableName { get; set; }
		public string ProcessStartupArgs { get; set; }
		public DateTime UTCCreation { get; set; }
	}
}
