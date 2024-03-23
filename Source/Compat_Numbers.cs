using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ResizableTableColumns {
    public static class Compat_Numbers {
        internal static void Patch(Harmony harmony) {
            if (!LoadedModManager.RunningMods.Any(x => x.PackageId == Strings.Numbers_ID)) return;
            var type = AccessTools.TypeByName("Numbers.Numbers");
            var method = AccessTools.Method(type, "CallReorderableWidget", 
                new Type[] { typeof(int), typeof(Rect) });
            var prefix = new HarmonyMethod(typeof(Compat_Numbers), nameof(CallReorderableWidget));
            harmony.Patch(method, prefix: prefix);
        }

        private static bool CallReorderableWidget() => !TableState.Dragging();
    }
}
