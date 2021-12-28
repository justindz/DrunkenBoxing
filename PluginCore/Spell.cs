using System.Collections.Generic;

namespace DrunkenBoxing {
    public class Spell {
        public int id;
        public bool has;

        public Spell(int id, bool has) {
            this.id = id;
            this.has = has;
        }

        public static Dictionary<string, Spell> BuildSpellTable() {
            Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
            spells.Add("Bloodstone Bolt I", new Spell(5525, false));
            spells.Add("Bloodstone Bolt II", new Spell(5526, false));
            spells.Add("Bloodstone Bolt III", new Spell(5527, false));
            spells.Add("Bloodstone Bolt IV", new Spell(5528, false));
            spells.Add("Bloodstone Bolt V", new Spell(5529, false));
            spells.Add("Bloodstone Bolt VI", new Spell(5530, false));
            spells.Add("Bloodstone Bolt VII", new Spell(5531, false));
            spells.Add("Incantation of Bloodstone Bolt", new Spell(5532, false));
            spells.Add("Hunter's Lash", new Spell(2970, false));
            spells.Add("Burning Curse", new Spell(4716, false));
            spells.Add("Ring of Death", new Spell(4239, false));
            spells.Add("Exsanguinating Wave", new Spell(3940, false));
            spells.Add("Corrupted Touch", new Spell(5980, false));
            spells.Add("Ward of Rebirth", new Spell(3071, false));
            spells.Add("Fellowship Heal I", new Spell(2981, false));
            spells.Add("Blessing of Unity", new Spell(5314, false));
            return spells;
        }
    }
}