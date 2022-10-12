using System;

using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Xml;
using ReikaKalseki.DIAlterra;

namespace ReikaKalseki.Exscansion
{
	public class ESConfig
	{		
		public enum ConfigEntries {
			[ConfigEntry("Leviathans Need To Be Scanned Before Scanner Room Detection", true)]LEVISCAN,
			[ConfigEntry("Scanner Room Can Detect Time Capsules", false)]TIMECAPSULE,
			[ConfigEntry("Base Scanner Range (m)", typeof(float), 300, 50, 1000, 300)]BASERANGE,
			[ConfigEntry("Range Upgrade Value (m)", typeof(float), 50, 10, 250, 50)]RANGEAMT,
			[ConfigEntry("Max Scanner Range (m)", typeof(float), 500, 100, 2000, 500)]MAXRANGE,
			[ConfigEntry("Base Scanner Speed Per Ping (s)", typeof(float), 14, 1, 3600, 14)]BASESPEED,
			[ConfigEntry("Speed Upgrade Value (s)", typeof(float), 3, 1, 30, 3)]SPDAMT,
		}
	}
}
