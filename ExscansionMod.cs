using UnityEngine;  //Needed for most Unity Enginer manipulations: Vectors, GameObjects, Audio, etc.
using System.IO;    //For data read/write methods
using System;    //For data read/write methods
using System.Collections.Generic;   //Working with Lists and Collections
using System.Reflection;
using System.Linq;   //More advanced manipulation of lists/collections
using HarmonyLib;
using QModManager.API.ModLoading;
using ReikaKalseki.DIAlterra;
using ReikaKalseki.Exscansion;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Assets;

namespace ReikaKalseki.Exscansion
{
  [QModCore]
  public static class ExscansionMod {
  	
    public const string MOD_KEY = "ReikaKalseki.Exscansion";
    
    //public static readonly ModLogger logger = new ModLogger();
	public static readonly Assembly modDLL = Assembly.GetExecutingAssembly();
    
    public static readonly Config<ESConfig.ConfigEntries> config = new Config<ESConfig.ConfigEntries>(modDLL);

    [QModPatch]
    public static void Load() {
        config.load();
        
        Harmony harmony = new Harmony(MOD_KEY);
        Harmony.DEBUG = true;
        FileLog.logPath = Path.Combine(Path.GetDirectoryName(modDLL.Location), "harmony-log.txt");
        FileLog.Log("Ran mod register, started harmony (harmony log)");
        SNUtil.log("Ran mod register, started harmony");
        try {
        	harmony.PatchAll(modDLL);
        }
        catch (Exception ex) {
			FileLog.Log("Caught exception when running patcher!");
			FileLog.Log(ex.Message);
			FileLog.Log(ex.StackTrace);
			FileLog.Log(ex.ToString());
        }
        
        ModVersionCheck.getFromGitVsInstall("Exscansion", modDLL, "Exscansion").register();
        SNUtil.checkModHash(modDLL);
		
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(ESHooks).TypeHandle);
        
        foreach (string line in File.ReadAllLines(Path.Combine(Path.GetDirectoryName(modDLL.Location), "scanner_ping_colors.txt"))) {
        	string[] split = line.Split(new char[]{'='}, StringSplitOptions.RemoveEmptyEntries);
        	if (split.Length == 2) {
        		TechType find = SNUtil.getTechType(split[0]);
        		if (find != TechType.None) {
        			string[] parts = split[1].Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
        			int red = 0;
        			int green = 0;
        			int blue = 0;
        			if (parts.Length >= 3 && int.TryParse(parts[0], out red) && int.TryParse(parts[1], out green) && int.TryParse(parts[2], out blue)) {
        				ESHooks.pingColors[find] = new Color(red/255F, green/255F, blue/255F, 1);
        				SNUtil.log("Setting scanner ping color: "+find+" = "+ESHooks.pingColors[find]);
        			}
        		}
        	}
        }
    }

  }
}
