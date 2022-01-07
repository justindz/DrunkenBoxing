using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public class Caster {
        public static bool IsCaster(WorldObject item) {
            if (item.ObjectClass == ObjectClass.WandStaffOrb)
                return true;
            
            return false;
        }
    }
}