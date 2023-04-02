using System;
using System.IO;    //For data read/write methods
using System.Collections;   //Working with Lists and Collections
using System.Collections.Generic;   //Working with Lists and Collections
using System.Linq;   //More advanced manipulation of lists/collections
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;  //Needed for most Unity Enginer manipulations: Vectors, GameObjects, Audio, etc.
using ReikaKalseki.DIAlterra;

namespace ReikaKalseki.Exscansion {
	
	[HarmonyPatch(typeof(uGUI_MapRoomScanner))]
	[HarmonyPatch("RebuildResourceList")]
	public static class ScannerTypeFilteringHook {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {
				CodeInstruction call = InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "filterScannerRoomResourceList", false, typeof(uGUI_MapRoomScanner));
				InstructionHandlers.patchInitialHook(codes, new CodeInstruction(OpCodes.Ldarg_0), call);
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	[HarmonyPatch(typeof(MapRoomFunctionality), MethodType.Getter)]
	[HarmonyPatch("mapScale")]
	public static class UpdateScannerHoloScale {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {
				RangePatchLib.replaceMaxRangeReference(codes);
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	[HarmonyPatch(typeof(MapRoomFunctionality))]
	[HarmonyPatch("OnResourceDiscovered")]
	public static class UpdateScannerResourceDistanceCheck {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {
				List<CodeInstruction> li = new List<CodeInstruction>{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerMaxRangeSq", false, new string[0])};
				InstructionHandlers.replaceConstantWithMethodCall(codes, 250000F, li);
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	[HarmonyPatch]
	public static class LoadMapRangeHook {
		
	    public static MethodBase TargetMethod() {
			return AccessTools.Method(typeof(MapRoomFunctionality).GetNestedType("<LoadMapWorld>d__51", BindingFlags.NonPublic | BindingFlags.Instance), "MoveNext");
	    }
		
	    public static Type TargetType() {
			 return typeof(MapRoomFunctionality);
	    }
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {
				RangePatchLib.replaceMaxRangeReference(codes);
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	[HarmonyPatch(typeof(MapRoomFunctionality))]
	[HarmonyPatch("GetScanRange")]
	public static class MainScannerRangePatch {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {
				RangePatchLib.replaceBaseRangeReference(codes);
				RangePatchLib.replaceMaxRangeReference(codes);
				RangePatchLib.replaceRangeBonusReference(codes);
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	[HarmonyPatch(typeof(MapRoomFunctionality))]
	[HarmonyPatch("GetScanInterval")]
	public static class ScannerSpeedPatch {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {
				RangePatchLib.replaceBaseSpeedReference(codes);
				RangePatchLib.replaceSpeedBonusReference(codes);
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	static class RangePatchLib {
		
		internal static void replaceMaxRangeReference(List<CodeInstruction> codes) {
			List<CodeInstruction> li = new List<CodeInstruction>{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerMaxRange", false, new string[0])};
			InstructionHandlers.replaceConstantWithMethodCall(codes, 500F, li);
		}
		
		internal static void replaceBaseRangeReference(List<CodeInstruction> codes) {
			List<CodeInstruction> li = new List<CodeInstruction>{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerBaseRange", false, new string[0])};
			InstructionHandlers.replaceConstantWithMethodCall(codes, 300F, li);
		}
		
		internal static void replaceRangeBonusReference(List<CodeInstruction> codes) {
			List<CodeInstruction> li = new List<CodeInstruction>{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getRangeUpgradeValue", false, new string[0])};
			InstructionHandlers.replaceConstantWithMethodCall(codes, 50F, li);
		}
		
		internal static void replaceBaseSpeedReference(List<CodeInstruction> codes) {
			List<CodeInstruction> li = new List<CodeInstruction>{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerBaseSpeed", false, new string[0])};
			InstructionHandlers.replaceConstantWithMethodCall(codes, 14F, li);
		}
		
		internal static void replaceSpeedBonusReference(List<CodeInstruction> codes) {
			List<CodeInstruction> li = new List<CodeInstruction>{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getSpeedUpgradeValue", false, new string[0])};
			InstructionHandlers.replaceConstantWithMethodCall(codes, 3F, li);
		}
		
	}
	
	[HarmonyPatch(typeof(ResourceTracker))]
	[HarmonyPatch("Register")]
	public static class ScannerFilteringHook {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {/*
				codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
				codes.Add(InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "registerResourceTracker", false, typeof(ResourceTracker)));
				codes.Add(new CodeInstruction(OpCodes.Ret));*/
				CodeInstruction br = codes[2];
				InstructionHandlers.patchInitialHook(codes, new CodeInstruction(OpCodes.Ldarg_0), InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "isObjectVisibleToScannerRoom", false, typeof(ResourceTracker)), new CodeInstruction(OpCodes.Brfalse, br.operand));
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	[HarmonyPatch(typeof(ResourceTracker))]
	[HarmonyPatch("Start")]
	public static class ScannerFilteringHook2 {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {/*
				codes.Add(new CodeInstruction(OpCodes.Ldarg_0));
				codes.Add(InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "registerResourceTracker", false, typeof(ResourceTracker)));
				codes.Add(new CodeInstruction(OpCodes.Ret));*/
				InstructionHandlers.patchInitialHook(codes, new CodeInstruction(OpCodes.Ldarg_0), InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "initializeResourceTracker", false, typeof(ResourceTracker)));
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
	
	[HarmonyPatch(typeof(uGUI_ResourceTracker))]
	[HarmonyPatch("UpdateVisibility")]
	public static class PingHUDVisibility {
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
			try {
				CodeInstruction call = InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "updatePingHUDVisibility", false, typeof(uGUI_ResourceTracker));
				InstructionHandlers.patchEveryReturnPre(codes, new CodeInstruction(OpCodes.Ldarg_0), call);
				FileLog.Log("Done patch "+MethodBase.GetCurrentMethod().DeclaringType);
			}
			catch (Exception e) {
				FileLog.Log("Caught exception when running patch "+MethodBase.GetCurrentMethod().DeclaringType+"!");
				FileLog.Log(e.Message);
				FileLog.Log(e.StackTrace);
				FileLog.Log(e.ToString());
			}
			return codes.AsEnumerable();
		}
	}
}
