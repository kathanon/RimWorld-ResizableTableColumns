using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ResizableTableColumns {
    public static class Patches_Column {
        public static TableState WidthsFrom = null;

        internal static void Patch(Harmony harmony) {
            var baseType = typeof(PawnColumnWorker);
            var thisType = typeof(Patches_Column);

            harmony.Patch(AccessTools.Method(baseType, "GetInteractableHeaderRect"),
                postfix: new HarmonyMethod(thisType, nameof(GetInteractableHeaderRect)));

            var minPost = new HarmonyMethod(thisType, nameof(GetMinWidth));
            var maxPost = new HarmonyMethod(thisType, nameof(GetMaxWidth));

            foreach (var type in baseType.AllSubclasses().AddItem(baseType)) {
                var min = type.GetMethod("GetMinWidth", AccessTools.allDeclared);
                if (min != null) harmony.Patch(min, postfix: minPost);
                var max = type.GetMethod("GetMaxWidth", AccessTools.allDeclared);
                if (max != null) harmony.Patch(max, postfix: maxPost);
            }
        }

        public static void GetInteractableHeaderRect(ref Rect __result) {
            __result = __result.ContractedBy(2f, 0f);
        }

        public static void GetMinWidth(PawnColumnWorker __instance, ref int __result) 
            => WidthsFrom?.SetMin(__instance, ref __result);

        public static void GetMaxWidth(PawnColumnWorker __instance, ref int __result) 
            => WidthsFrom?.SetMax(__instance, ref __result);
    }
}
