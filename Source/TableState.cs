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
    public class TableState {
        private static TableState current = null;

        private readonly Dictionary<PawnColumnDef, float> widths = 
            new Dictionary<PawnColumnDef, float>();
        private readonly PawnTable table;
        private PawnColumnDef dragged = null;
        private bool reset = false;

        public static bool Dragging() => current?.dragged != null;

        public TableState(PawnTable table) {
            this.table = table;
        }

        public void DoMouse(bool isOver, PawnColumnDef def, bool doubleClick) {
            current = this;
            if (Event.current.type == EventType.MouseDown && isOver) {
                dragged = def;
            }

            if (Event.current.type == EventType.MouseUp) {
                dragged = null;
            }

            if (doubleClick && isOver) {
                reset = widths.Remove(def);
            }
        }

        public bool DragUpdate(List<float> currentWidths) {
            bool change;
            if (dragged == null || Event.current.type != EventType.MouseDrag) {
                change = reset;
                reset = false;
                return change;
            }
            if (!widths.TryGetValue(dragged, out var old)) {
                int i = table.Columns.IndexOf(dragged);
                if (i < 0) return false;
                old = currentWidths[i];
            }
            var worker = dragged.Worker;
            var width = Mathf.Clamp(
                old + Event.current.delta.x,
                worker.GetMinWidth(table),
                worker.GetMaxWidth(table));
            // TODO: Calculate available width?

            change = old != width;
            if (change) widths[dragged] = width;
            return change;
        }

        public void SetMin(PawnColumnWorker worker, ref int result) {
            if (widths.TryGetValue(worker.def, out var value) && result < value) {
                result = Mathf.FloorToInt(value);
            }
        }

        public void SetMax(PawnColumnWorker worker, ref int result) {
            if (widths.TryGetValue(worker.def, out var value) && result > value) {
                result = Mathf.CeilToInt(value);
            }
        }
    }
}
