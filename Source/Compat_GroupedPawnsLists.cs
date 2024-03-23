using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ResizableTableColumns {
    public static class Compat_GroupedPawnsLists {
        public static readonly bool Active =
            LoadedModManager.RunningMods.Any(x => x.PackageId == Strings.GroupedPawnsLists_ID);
    }
}
