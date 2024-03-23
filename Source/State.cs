using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ResizableTableColumns {
    public static class State {
        private static readonly ConditionalWeakTable<PawnTable, TableState> states = 
            new ConditionalWeakTable<PawnTable, TableState>();

        public static TableState Get(PawnTable table) 
            => states.GetValue(table, t => new TableState(t));
    }
}
