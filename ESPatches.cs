using System;
using System.Collections;   //Working with Lists and Collections
using System.Collections.Generic;   //Working with Lists and Collections
using System.IO;    //For data read/write methods
using System.Linq;   //More advanced manipulation of lists/collections
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

using ReikaKalseki.DIAlterra;

using UnityEngine;  //Needed for most Unity Enginer manipulations: Vectors, GameObjects, Audio, etc.

namespace ReikaKalseki.Exscansion {

	static class ESPatches {

		[HarmonyPatch(typeof(MapRoomFunctionality), MethodType.Getter)]
		[HarmonyPatch("mapScale")]
		public static class UpdateScannerHoloScale {

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {
					RangePatchLib.replaceMaxRangeReference(codes);
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
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
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {
					InsnList li = new InsnList{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerMaxRangeSq", false, new string[0])};
					codes.replaceConstantWithMethodCall(250000F, li);
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
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
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {
					RangePatchLib.replaceMaxRangeReference(codes);
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
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
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {
					RangePatchLib.replaceBaseRangeReference(codes);
					RangePatchLib.replaceMaxRangeReference(codes);
					RangePatchLib.replaceRangeBonusReference(codes);
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
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
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {
					RangePatchLib.replaceBaseSpeedReference(codes);
					RangePatchLib.replaceSpeedBonusReference(codes);
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
					FileLog.Log(e.Message);
					FileLog.Log(e.StackTrace);
					FileLog.Log(e.ToString());
				}
				return codes.AsEnumerable();
			}
		}

		static class RangePatchLib {

			internal static void replaceMaxRangeReference(InsnList codes) {
				InsnList li = new InsnList{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerMaxRange", false, new string[0])};
				codes.replaceConstantWithMethodCall(500F, li);
			}

			internal static void replaceBaseRangeReference(InsnList codes) {
				InsnList li = new InsnList{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerBaseRange", false, new string[0])};
				codes.replaceConstantWithMethodCall(300F, li);
			}

			internal static void replaceRangeBonusReference(InsnList codes) {
				InsnList li = new InsnList{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getRangeUpgradeValue", false, new string[0])};
				codes.replaceConstantWithMethodCall(50F, li);
			}

			internal static void replaceBaseSpeedReference(InsnList codes) {
				InsnList li = new InsnList{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getScannerBaseSpeed", false, new string[0])};
				codes.replaceConstantWithMethodCall(14F, li);
			}

			internal static void replaceSpeedBonusReference(InsnList codes) {
				InsnList li = new InsnList{InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "getSpeedUpgradeValue", false, new string[0])};
				codes.replaceConstantWithMethodCall(3F, li);
			}

		}

		[HarmonyPatch(typeof(ResourceTracker))]
		[HarmonyPatch("Register")]
		public static class ScannerFilteringHook {

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {/*
				codes.add(OpCodes.Ldarg_0);
				codes.invoke("ReikaKalseki.Exscansion.ESHooks", "registerResourceTracker", false, typeof(ResourceTracker));
				codes.add(OpCodes.Ret);*/
					CodeInstruction br = codes[2];
					codes.patchInitialHook(new CodeInstruction(OpCodes.Ldarg_0), InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "isObjectVisibleToScannerRoom", false, typeof(ResourceTracker)), new CodeInstruction(OpCodes.Brfalse, br.operand));
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
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
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {/*
				codes.add(OpCodes.Ldarg_0);
				codes.invoke("ReikaKalseki.Exscansion.ESHooks", "registerResourceTracker", false, typeof(ResourceTracker));
				codes.add(OpCodes.Ret);*/
					codes.patchInitialHook(new CodeInstruction(OpCodes.Ldarg_0), InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "initializeResourceTracker", false, typeof(ResourceTracker)));
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
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
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {
					CodeInstruction call = InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "updatePingHUDVisibility", false, typeof(uGUI_ResourceTracker));
					codes.patchEveryReturnPre(new CodeInstruction(OpCodes.Ldarg_0), call);
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
					FileLog.Log(e.Message);
					FileLog.Log(e.StackTrace);
					FileLog.Log(e.ToString());
				}
				return codes.AsEnumerable();
			}
		}

		[HarmonyPatch(typeof(uGUI_ResourceTracker))]
		[HarmonyPatch("UpdateBlips")]
		public static class PingHUDGenerationHook {

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
				InstructionHandlers.logPatchStart(MethodBase.GetCurrentMethod(), instructions);
				InsnList codes = new InsnList(instructions);
				try {
					int idx = InstructionHandlers.getInstruction(codes, 0, 1, OpCodes.Stfld, "uGUI_ResourceTracker+Blip", "techType");
					codes[idx] = InstructionHandlers.createMethodCall("ReikaKalseki.Exscansion.ESHooks", "setResourcePingType", false, typeof(uGUI_ResourceTracker.Blip), typeof(TechType));
					InstructionHandlers.logCompletedPatch(MethodBase.GetCurrentMethod(), instructions);
				}
				catch (Exception e) {
					InstructionHandlers.logErroredPatch(MethodBase.GetCurrentMethod());
					FileLog.Log(e.Message);
					FileLog.Log(e.StackTrace);
					FileLog.Log(e.ToString());
				}
				return codes.AsEnumerable();
			}
		}

	}
}
