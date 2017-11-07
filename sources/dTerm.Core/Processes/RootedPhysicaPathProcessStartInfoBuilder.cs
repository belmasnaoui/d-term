﻿using System;
using System.Diagnostics;
using System.IO;

namespace dTerm.Core.Processes
{
	public class RootedPhysicaPathProcessStartInfoBuilder : ProcessStartInfoBuilderBase
	{
		string _rootedPhysicalFileName;

		public RootedPhysicaPathProcessStartInfoBuilder(string rootedPhysicalFileName)
		{
			_rootedPhysicalFileName = rootedPhysicalFileName ?? throw new ArgumentNullException(nameof(rootedPhysicalFileName), nameof(RootedPhysicaPathProcessStartInfoBuilder));
		}

		public static implicit operator ProcessStartInfo(RootedPhysicaPathProcessStartInfoBuilder builder) => builder.GetProcessStartInfo();

		internal override ProcessStartInfo GetProcessStartInfo()
		{
			var fileInfo = new FileInfo(_rootedPhysicalFileName);

			return new ProcessStartInfo(fileInfo.FullName);
		}
	}
}
