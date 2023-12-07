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
    
    internal static ScannerRoomMarker abandonedBase;
    internal static ScannerRoomMarker alienBase;

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
        
        abandonedBase = new ScannerRoomMarker(TechTypeHandler.AddTechType(modDLL, "AbandonedBase", "Titanium Mass", ""));
		alienBase = new ScannerRoomMarker(TechTypeHandler.AddTechType(modDLL, "AlienBase", "Unidentified Object", ""));
		abandonedBase.Patch();
		alienBase.Patch();
		/*
		if (config.getBoolean(ESConfig.ConfigEntries.BASES)) {
			GenUtil.registerWorldgen(new PositionedPrefab(abandonedBase.ClassID, new Vector3(0, 0, 0)));
			GenUtil.registerWorldgen(new PositionedPrefab(abandonedBase.ClassID, new Vector3(0, 0, 0)));
			GenUtil.registerWorldgen(new PositionedPrefab(abandonedBase.ClassID, new Vector3(0, 0, 0)));
			
			GenUtil.registerWorldgen(new PositionedPrefab(abandonedBase.ClassID, new Vector3(0, 0, 0)));
			GenUtil.registerWorldgen(new PositionedPrefab(abandonedBase.ClassID, new Vector3(0, 0, 0)));
		}*/
		if (config.getBoolean(ESConfig.ConfigEntries.ALIEN)) {
			GenUtil.registerWorldgen(new PositionedPrefab(alienBase.ClassID, new Vector3(-56, -1211, 116)));
			GenUtil.registerWorldgen(new PositionedPrefab(alienBase.ClassID, new Vector3(265, -1440, -347)));
			GenUtil.registerWorldgen(new PositionedPrefab(alienBase.ClassID, new Vector3(-890, -311, -816))); //sparse reef
			GenUtil.registerWorldgen(new PositionedPrefab(alienBase.ClassID, new Vector3(-1224, -395, 1072.5F))); //meteor
			GenUtil.registerWorldgen(new PositionedPrefab(alienBase.ClassID, new Vector3(-628.5F, -559, 1485))); //nbkelp
			GenUtil.registerWorldgen(new PositionedPrefab(alienBase.ClassID, new Vector3(-1119, -685, -692))); //lr lab cache
		}
		
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(ESHooks).TypeHandle);
        
        TechTypeMappingConfig<Color>.loadInline("scanner_ping_colors", TechTypeMappingConfig<Color>.ColorParser.instance, TechTypeMappingConfig<Color>.dictionaryAssign(ESHooks.pingColors));
    }

  }
}
