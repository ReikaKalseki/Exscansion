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

		public static event Func<ResourceTracker, bool> scannabilityEvent;
	    
	    private static readonly Dictionary<string, TechType> scannerInjections = new Dictionary<string, TechType>() {
	    	{"61ac1241-e990-4646-a618-bddb6960325b", TechType.SeaTreaderPoop},
	    	{"54701bfc-bb1a-4a84-8f79-ba4f76691bef", TechType.GhostLeviathan},
	    	{"5ea36b37-300f-4f01-96fa-003ae47c61e5", TechType.GhostLeviathanJuvenile},
	    	{"35ee775a-d54c-4e63-a058-95306346d582", TechType.SeaTreader},
	    	{"ff43eacd-1a9e-4182-ab7b-aa43c16d1e53", TechType.SeaDragon},
	    	{"c129d979-4f68-41d8-b9bc-557676d18a5a", TechType.TimeCapsule},
	    };
	    
	    private static readonly HashSet<TechType> leviathans = new HashSet<TechType>() {
			TechType.ReaperLeviathan,
	    	TechType.SeaDragon,
	    	TechType.GhostLeviathanJuvenile,
	    	TechType.GhostLeviathan,
	    	TechType.SeaTreader,
	    	TechType.SeaEmperorLeviathan,
	    	TechType.Reefback,
	    };
	    
	    static ESHooks() {
	    	DIHooks.onSkyApplierSpawnEvent += onSkyApplierSpawn;
	    	DIHooks.onWorldLoadedEvent += onWorldLoaded;
	    }
		
		public static void onWorldLoaded() {
			//NotificationManager.main.Subscribe(new NotificationListener(), new List<NotificationManager.NotificationId>{new NotificationManager.NotificationId(NotificationManager.Group.Encyclopedia, Stalker)});
		}
		
		public static void addLeviathan(TechType tt) {
			leviathans.Add(tt);
		}
	    
	    public static void onSkyApplierSpawn(SkyApplier pk) {
	    	GameObject go = pk.gameObject;
	    	PrefabIdentifier pi = go.GetComponentInParent<PrefabIdentifier>();
			if (pi && scannerInjections.ContainsKey(pi.ClassId)) {
				TechType tt = scannerInjections[pi.ClassId];
				if (tt == TechType.TimeCapsule && !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.TIMECAPSULE))
					return;
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
	    
	    public static void filterScannerRoomResourceList(uGUI_MapRoomScanner gui) {
	    	gui.availableTechTypes.RemoveWhere(item => !playerCanScanFor(item));
	    }
	    
	    private static bool playerCanScanFor(TechType tt) {
			if (leviathans.Contains(tt))
				return PDAScanner.complete.Contains(tt) || !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.LEVISCAN);
	    	switch(tt) {
	    		case TechType.LimestoneChunk:
	    		case TechType.SandstoneChunk:
	    		case TechType.BasaltChunk:	    			
	    		case TechType.PrecursorIonCrystal:
				case TechType.AluminumOxide:
				case TechType.Diamond:
				case TechType.Lithium:
				case TechType.Nickel:
				case TechType.Kyanite:
				case TechType.UraniniteCrystal:
	    			return !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.RESSCAN) || PDAScanner.complete.Contains(tt);
	    		case TechType.PrecursorKey_Purple:
	    		case TechType.PrecursorKey_Blue:
	    		case TechType.PrecursorKey_Red:
	    		case TechType.PrecursorKey_White:
	    		case TechType.PrecursorKey_Orange:
	    			return false;//KnownTech.knownTech.Contains(tt);
	    		case TechType.StalkerTooth:
	    			return PDAScanner.complete.Contains(TechType.StalkerTooth);
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
	    			return PDAEncyclopedia.entries.ContainsKey("UnknownEgg");//PDAScanner.complete.Contains(TechType.GenericEgg);
	    		case TechType.TimeCapsule:
	    			return ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.TIMECAPSULE);
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
	   		if (scannabilityEvent != null)
	   			if (!scannabilityEvent.Invoke(rt))
	   				return false;
	   		//SNUtil.log("Checking scanner visibility of "+rt.gameObject+" @ "+rt.gameObject.transform.position+": "+rt.gameObject.GetComponentInChildren<Drillable>());
	    	if (rt.gameObject.GetComponentInChildren<Drillable>() && !(KnownTech.knownTech.Contains(TechType.ExosuitDrillArmModule) && KnownTech.knownTech.Contains(TechType.Exosuit)))
	    		return false;
	    	BlueprintHandTarget bpt = rt.gameObject.FindAncestor<BlueprintHandTarget>();
	    	if (bpt && bpt.used)
	    		return false;
	    	return true;
	    }
	   
	   public static void initializeResourceTracker(ResourceTracker rt) {
	   	if (!isObjectVisibleToScannerRoom(rt)) {
	   		rt.Unregister();
	   	}
	   }
	   
	   public static void updatePingHUDVisibility(uGUI_ResourceTracker gui) {/*
	   	bool orig = gui.showGUI;
	   	gui.showGUI = gui.showGUI && !Player.main.IsInBase();
	   	SNUtil.writeToChat(orig+"+"+Player.main.currentSub+">"+gui.showGUI);*/
	   	if (gui && Player.main)
	   		gui.gameObject.SetActive(!Player.main.IsInBase());
	   }
	}
}
