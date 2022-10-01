using System;
using System.IO;
using System.Xml;
using System.Reflection;

using System.Collections.Generic;
using System.Linq;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Utility;

using UnityEngine;

using ReikaKalseki.DIAlterra;
using ReikaKalseki.Exscansion;

namespace ReikaKalseki.Exscansion {
	
	public static class ESHooks {
	    
	    private static readonly Dictionary<string, TechType> scannerInjections = new Dictionary<string, TechType>() {
	    	{"61ac1241-e990-4646-a618-bddb6960325b", TechType.SeaTreaderPoop},
	    	{"54701bfc-bb1a-4a84-8f79-ba4f76691bef", TechType.GhostLeviathan},
	    	{"35ee775a-d54c-4e63-a058-95306346d582", TechType.SeaTreader},
	    	{"ff43eacd-1a9e-4182-ab7b-aa43c16d1e53", TechType.SeaDragon},
	    };
	    
	    static ESHooks() {
	    	DIHooks.onSkyApplierSpawnEvent += onSkyApplierSpawn;
	    }
	    
	    public static void onSkyApplierSpawn(SkyApplier pk) {
	    	GameObject go = pk.gameObject;
	    	PrefabIdentifier pi = go.GetComponentInParent<PrefabIdentifier>();
			if (pi && scannerInjections.ContainsKey(pi.ClassId)) {
				TechType tt = scannerInjections[pi.ClassId];
				ObjectUtil.makeMapRoomScannable(go, tt, true);
	    	}
	    	else if (pi && PrefabData.getPrefab(pi.ClassId) != null && PrefabData.getPrefab(pi.ClassId).Contains("Coral_reef_jeweled_disk")) {
	    		ObjectUtil.makeMapRoomScannable(go, TechType.JeweledDiskPiece);
	    	}
	    	else if (ObjectUtil.isPDA(go)) {
				ObjectUtil.makeMapRoomScannable(go, TechType.PDA);
	    	}
	    }
		
		public static float getScannerBaseRange() {
			return ExscansionMod.config.getFloat(ESConfig.ConfigEntries.BASERANGE);
		}
		
		public static float getRangeUpgradeValue() {
			return ExscansionMod.config.getFloat(ESConfig.ConfigEntries.RANGEAMT);
		}
		
		public static float getScannerMaxRange() {
			return ExscansionMod.config.getFloat(ESConfig.ConfigEntries.MAXRANGE);
		}
		
		public static float getScannerMaxRangeSq() {
			float r = getScannerMaxRange();
			return r*r;
		}
		
		public static float getScannerBaseSpeed() {
			return ExscansionMod.config.getFloat(ESConfig.ConfigEntries.BASESPEED);
		}
		
		public static float getSpeedUpgradeValue() {
			return ExscansionMod.config.getFloat(ESConfig.ConfigEntries.SPDAMT);
		}
	    
	    public static void generateScannerRoomResourceList(uGUI_MapRoomScanner gui) {
	    	gui.availableTechTypes.RemoveWhere(item => !playerCanScanFor(item));
	    	gui.RebuildResourceList();
	    }
	    
	    private static bool playerCanScanFor(TechType tt) {
	    	switch(tt) {
	    		case TechType.ReaperLeviathan:
	    		case TechType.SeaDragon:
	    		case TechType.GhostLeviathanJuvenile:
	    		case TechType.GhostLeviathan:
	    		case TechType.SeaTreader:
	    		case TechType.SeaEmperorLeviathan:
	    		case TechType.Reefback:
					return PDAScanner.complete.Contains(tt) || !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.LEVISCAN);
	    		case TechType.LimestoneChunk:
	    		case TechType.SandstoneChunk:
	    		case TechType.BasaltChunk:
	    			
	    		case TechType.PrecursorIonCrystal:
	    		case TechType.PrecursorKey_Purple:
	    		case TechType.PrecursorKey_Blue:
	    		case TechType.PrecursorKey_Red:
	    		case TechType.PrecursorKey_White:
	    		case TechType.PrecursorKey_Orange:
	    			return PDAScanner.complete.Contains(tt);
	    		case TechType.StalkerTooth:
	    			return PDAScanner.complete.Contains(TechType.Stalker);
	    		case TechType.GenericEgg:
	    		case TechType.StalkerEgg:
	    		case TechType.BonesharkEgg:
	    		case TechType.CrabsnakeEgg:
	    		case TechType.CrashEgg:
	    		case TechType.CrabsquidEgg:
	    		case TechType.CutefishEgg:
	    		case TechType.JellyrayEgg:
	    		case TechType.RabbitrayEgg:
	    		case TechType.SandsharkEgg:
	    		case TechType.ShockerEgg:
	    		case TechType.ReefbackEgg:
	    		case TechType.MesmerEgg:
	    		case TechType.LavaLizardEgg:
	    		case TechType.JumperEgg:
	    		case TechType.SpadefishEgg:
	    			return PDAScanner.complete.Contains(TechType.GenericEgg);
	    		default:
	    			return true;
	    	}
	    }
	    /*
	    public static void registerResourceTracker(ResourceTracker rt) {
	    	if (rt.techType != TechType.None && isObjectVisibleToScannerRoom(rt)) {
				Dictionary<string, ResourceTracker.ResourceInfo> orAddNew = ResourceTracker.resources.GetOrAddNew(rt.techType);
				string key = rt.uniqueId;
				ResourceTracker.ResourceInfo resourceInfo;
				if (!orAddNew.TryGetValue(key, out resourceInfo)) {
					resourceInfo = new ResourceTracker.ResourceInfo();
					resourceInfo.uniqueId = key;
					resourceInfo.position = rt.transform.position;
					resourceInfo.techType = rt.techType;
					orAddNew.Add(key, resourceInfo);
					if (ResourceTracker.onResourceDiscovered != null) {
						ResourceTracker.onResourceDiscovered.Invoke(resourceInfo);
						return;
					}
				}
				else {
					resourceInfo.position = rt.transform.position;
				}
			}
	    }
	    */
	    public static bool isObjectVisibleToScannerRoom(ResourceTracker rt) {
	   		//SNUtil.log("Checking scanner visibility of "+rt.gameObject+" @ "+rt.gameObject.transform.position+": "+rt.gameObject.GetComponentInChildren<Drillable>());
	    	if (rt.gameObject.GetComponentInChildren<Drillable>() && !(KnownTech.knownTech.Contains(TechType.ExosuitDrillArmModule) && KnownTech.knownTech.Contains(TechType.Exosuit)))
	    		return false;
	    	return true;
	    }
	}
}
