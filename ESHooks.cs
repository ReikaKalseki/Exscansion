using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using ReikaKalseki.DIAlterra;
using ReikaKalseki.Exscansion;

using SMLHelper.V2.Assets;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Utility;

using UnityEngine;
using UnityEngine.UI;

namespace ReikaKalseki.Exscansion {

	public static class ESHooks {

		public static event Action<ResourceScanCheck> scannabilityEvent;

		private static readonly Dictionary<string, TechType> scannerInjections;

		private static readonly HashSet<TechType> leviathans = new HashSet<TechType>() {
			TechType.ReaperLeviathan,
			TechType.SeaDragon,
			TechType.GhostLeviathanJuvenile,
			TechType.GhostLeviathan,
			TechType.SeaTreader,
			TechType.SeaEmperorLeviathan,
			TechType.Reefback,
		};

		internal static readonly Color DEFAULT_PING_COLOR = new Color(1, 186 / 255F, 0, 1);

		internal static readonly Dictionary<TechType, Color> pingColors = new Dictionary<TechType, Color>();

		static ESHooks() {
			SNUtil.log("Initializing ESHooks");
			DIHooks.onSkyApplierSpawnEvent += onSkyApplierSpawn;
			DIHooks.onWorldLoadedEvent += onWorldLoaded;
			DIHooks.scannerRoomTechTypeListingEvent += (gui) => gui.availableTechTypes.RemoveWhere(item => !playerCanScanFor(item));

			scannerInjections = new Dictionary<string, TechType> {
				["61ac1241-e990-4646-a618-bddb6960325b"] = TechType.SeaTreaderPoop,
				["54701bfc-bb1a-4a84-8f79-ba4f76691bef"] = TechType.GhostLeviathan,
				["5ea36b37-300f-4f01-96fa-003ae47c61e5"] = TechType.GhostLeviathanJuvenile,
				["35ee775a-d54c-4e63-a058-95306346d582"] = TechType.SeaTreader,
				["ff43eacd-1a9e-4182-ab7b-aa43c16d1e53"] = TechType.SeaDragon,
				["c129d979-4f68-41d8-b9bc-557676d18a5a"] = TechType.TimeCapsule,
			};

			if (ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.BASES)) {
				scannerInjections["2921736c-c898-4213-9615-ea1a72e28178"] = ExscansionMod.abandonedBase.markerType; //jellyshroom
				scannerInjections["42a80cbc-d9fd-49d2-94b3-b5178024b3cb"] = ExscansionMod.abandonedBase.markerType; //dgr
				scannerInjections["99b164ac-dfb4-4a14-b305-8666fa227717"] = ExscansionMod.abandonedBase.markerType; //float ctr
				scannerInjections["569f22e0-274d-49b0-ae5e-21ef0ce907ca"] = ExscansionMod.abandonedBase.markerType; //float mtn1
				scannerInjections["0e394d55-da8c-4b3e-b038-979477ce77c1"] = ExscansionMod.abandonedBase.markerType; //float mtn2

				SeabaseReconstruction.WorldgenSeabaseController.onWorldgenSeabaseLoad += bb => bb.gameObject.makeMapRoomScannable(ExscansionMod.abandonedBase.markerType);
			}
			if (ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.ALIEN)) {
				scannerInjections["22fb9ee9-690d-426c-844f-a80e527b5fe6"] = ExscansionMod.alienBase.markerType; //gun
				scannerInjections["80f6c46a-ecfe-4a19-b05f-0466eafde411"] = ExscansionMod.alienBase.markerType; //drf

				scannerInjections["5cebe7f6-b1ce-4ae0-8008-ccfdac5d5690"] = ExscansionMod.alienBase.markerType; //mushroom arch
				scannerInjections["2a579d8a-5833-4415-acf9-5d7c4d00c65e"] = ExscansionMod.alienBase.markerType; //lr arch
				scannerInjections["beb02a51-139f-4cb1-b7fd-831f8d00e55e"] = ExscansionMod.alienBase.markerType; //koosh arch
				scannerInjections["50716be9-fb9c-4da4-9f3f-8916cbdbfdaf"] = ExscansionMod.alienBase.markerType; //crag arch

				foreach (string s in ObjectUtil.getVents())
					scannerInjections[s] = ExscansionMod.alienBase.markerType;

				//scannerInjections[""] = ExscansionMod.alienBase.markerType; //
				//scannerInjections[""] = ExscansionMod.alienBase.markerType; //
				//scannerInjections[""] = ExscansionMod.alienBase.markerType; //
			}
			if (ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.FOSSILS)) {
				foreach (string s in ObjectUtil.getFossils())
					scannerInjections[s] = ExscansionMod.fossils.markerType;
			}
		}

		public static void onWorldLoaded() {
			//NotificationManager.main.Subscribe(new NotificationListener(), new List<NotificationManager.NotificationId>{new NotificationManager.NotificationId(NotificationManager.Group.Encyclopedia, Stalker)});
		}

		public static void addLeviathan(TechType tt) {
			leviathans.Add(tt);
		}

		public static void onSkyApplierSpawn(SkyApplier pk) {
			GameObject go = pk.gameObject;
			if (go.name.StartsWith("Seamoth", StringComparison.InvariantCultureIgnoreCase) && go.name.EndsWith("Arm(Clone)", StringComparison.InvariantCultureIgnoreCase))
				return;
			PrefabIdentifier pi = go.GetComponentInParent<PrefabIdentifier>();
			if (pi && scannerInjections.ContainsKey(pi.ClassId)) {
				TechType tt = scannerInjections[pi.ClassId];
				if (tt == TechType.TimeCapsule && !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.TIMECAPSULE))
					return;
				if (tt == ExscansionMod.alienBase.TechType && !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.ALIEN))
					return;
				go.makeMapRoomScannable(tt, true);
			}
			else if (pi && PrefabData.getPrefab(pi.ClassId) != null && PrefabData.getPrefab(pi.ClassId).Contains("Coral_reef_jeweled_disk")) {
				go.makeMapRoomScannable(TechType.JeweledDiskPiece);
			}
			else if (go.isPDA()) {
				go.makeMapRoomScannable(TechType.PDA);
			}
			else if (ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.BASES) && go.GetComponent<SeabaseReconstruction.WorldgenSeabaseController>()) {
				go.makeMapRoomScannable(ExscansionMod.abandonedBase.TechType);
			}/*
	    	else if (ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.ALIEN) && go.GetComponent<PrecursorTeleporter>()) {
	    		ObjectUtil.makeMapRoomScannable(go, ExscansionMod.alienBase.markerType);
	    	}*/
			Drillable dr = pk.GetComponent<Drillable>();
			if (dr && dr.resources.Length == 1 && PDAScanner.GetEntryData(dr.resources[0].techType) != null && !pk.GetComponent<TechTag>()) {
				go.EnsureComponent<TechTag>().type = dr.resources[0].techType;
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
			return r * r;
		}

		public static float getScannerBaseSpeed() {
			return ExscansionMod.config.getFloat(ESConfig.ConfigEntries.BASESPEED);
		}

		public static float getSpeedUpgradeValue() {
			return ExscansionMod.config.getFloat(ESConfig.ConfigEntries.SPDAMT);
		}

		private static bool playerCanScanFor(TechType tt) {
			if (leviathans.Contains(tt))
				return PDAScanner.complete.Contains(tt) || !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.LEVISCAN);
			switch (tt) {
				case TechType.LimestoneChunk:
				case TechType.SandstoneChunk:
				case TechType.ShaleChunk:
				case TechType.Sulphur:
				case TechType.Magnetite:
				case TechType.BasaltChunk:
				case TechType.PrecursorIonCrystal:
				case TechType.AluminumOxide:
				//case TechType.Diamond: these two have no scan entries, do not gate
				//case TechType.Lithium:
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
					return !ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.TOOTHSCAN) || PDAScanner.complete.Contains(TechType.StalkerTooth);
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
			bool allow = true;
			if (rt.gameObject.GetComponentInChildren<Drillable>() && !(KnownTech.knownTech.Contains(TechType.ExosuitDrillArmModule) && KnownTech.knownTech.Contains(TechType.Exosuit)))
				allow = false;
			BlueprintHandTarget bpt = rt.gameObject.FindAncestor<BlueprintHandTarget>();
			if (bpt && bpt.used)
				allow = false;
			if (scannabilityEvent != null) {
				ResourceScanCheck rs = new ResourceScanCheck(rt, allow);
				scannabilityEvent.Invoke(rs);
				allow = rs.isDetectable;
			}
			//SNUtil.log("Checking scanner visibility of "+rt.gameObject+" @ "+rt.gameObject.transform.position+": "+rt.gameObject.GetComponentInChildren<Drillable>());

			return allow;
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
			if (gui && Player.main && Camera.main)
				gui.gameObject.SetActive(Player.main.currentSub == null || !Player.main.currentSub.isBase || Player.main.GetBiomeString() == "observatory" || Vector3.Distance(Camera.main.transform.position, Player.main.transform.position) > 4);
		}

		public static void setResourcePingType(uGUI_ResourceTracker.Blip blip, TechType type) { //default color is 0xFFBA00
			blip.techType = type;
			if (!ExscansionMod.config.getBoolean(ESConfig.ConfigEntries.PINGCOLOR))
				return;
			CanvasRenderer render = blip.gameObject.GetComponent<CanvasRenderer>();
			Image img = blip.gameObject.GetComponent<Image>();
			img.sprite = img.sprite.setTexture(TextureManager.getTexture(ExscansionMod.modDLL, "Textures/blip"));
			Color c = DEFAULT_PING_COLOR;
			if (pingColors.ContainsKey(type))
				c = pingColors[type];
			img.material.SetColor("_Color", c);
			render.SetColor(c);
			blip.gameObject.GetComponentInChildren<Text>().color = c;
			if (blip.techType == TechType.Quartz)
				blip.gameObject.GetComponentInChildren<CanvasRenderer>().SetColor(Color.red);
		}

		public class ResourceScanCheck {

			public readonly ResourceTracker resource;
			public readonly bool defaultVisibility;

			public bool isDetectable;

			internal ResourceScanCheck(ResourceTracker rt, bool def) {
				resource = rt;
				defaultVisibility = def;
				isDetectable = defaultVisibility;
			}
		}
	}
}
