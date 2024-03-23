using HarmonyLib;
using Verse;
using UnityEngine;
using RimWorld;

namespace ResizableTableColumns {
    [StaticConstructorOnStartup]
    public class Main {
        static Main() {
            var harmony = new Harmony(Strings.ID);
            harmony.PatchAll();
            Patches_Column.Patch(harmony);
            Compat_Numbers.Patch(harmony);
        }
    }
}
