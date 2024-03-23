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
    [HarmonyPatch]
    public static class Patches_Table {
        public static readonly float XAdj = Compat_GroupedPawnsLists.Active ? 8f : 0f;

        [HarmonyPostfix]
        [HarmonyBefore(Strings.Numbers_Harmony)]
        [HarmonyPatch(typeof(PawnTable), nameof(PawnTable.PawnTableOnGUI))]
        public static void PawnTableOnGUI(PawnTable __instance, Vector2 position, 
                List<float> ___cachedColumnWidths, float ___cachedHeaderHeight) {
            var table = __instance;
            var state = State.Get(table);
            float h = Mathf.Min(25f, ___cachedHeaderHeight);
            float y = position.y + ___cachedHeaderHeight - h;
            var columns = table.Columns;
            int n = columns.Count - 1;
            bool doubleClick = DetectDoubleClick();

            bool[] handles = new bool[n];
            bool after = false;
            for (int i = n; i >= 0; i--) {
                var worker = columns[i].Worker;
                bool sizable = worker.GetMinWidth(table) < worker.GetMaxWidth(table);
                if (i < n) handles[i] = sizable && after;
                after |= sizable;
            }

            Rect area = new Rect(position.x - 2f + XAdj, y, 4f, h);
            Rect line = area.ContractedBy(1f);
            for (int i = 0; i < n; i++) {
                area.x += (int) ___cachedColumnWidths[i];
                line.x = area.x + 1f;
                if (handles[i]) {
                    bool isOver = Mouse.IsOver(area);
                    if (isOver) {
                        Widgets.DrawBoxSolid(line, Color.white);
                    }
                    state.DoMouse(isOver, columns[i], doubleClick);
                }
            }

            if (state.DragUpdate(___cachedColumnWidths)) {
                table.SetDirty();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PawnTable), "RecacheColumnWidths")]
        public static void RecacheColumnWidths_pre(PawnTable __instance) 
            => Patches_Column.WidthsFrom = State.Get(__instance);

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PawnTable), "RecacheColumnWidths")]
        public static void RecacheColumnWidths_post() 
            => Patches_Column.WidthsFrom = null;

        private const long DCThreshold = 1000;
        private static long dcPrev = 0;
        private static long dcLast = 0;

        private static bool DetectDoubleClick() {
            if (Event.current.type == EventType.MouseDown) {
                dcPrev = dcLast;
                dcLast = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            } if (Event.current.type == EventType.MouseUp) {
                long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                return now - dcPrev < DCThreshold;
            }
            return false;
        }
    }
}
