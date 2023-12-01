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
        
        TechTypeMappingConfig<Color>.loadInline("scanner_ping_colors", TechTypeMappingConfig<Color>.ColorParser.instance, TechTypeMappingConfig<Color>.dictionaryAssign(ESHooks.pingColors));
    }

  }
}
