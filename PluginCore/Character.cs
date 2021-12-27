using Decal.Adapter.Wrappers;
using System.Collections.Generic;

namespace DrunkenBoxing {
    class Character {
        public static Character instance = new Character();
        public int id;
        public List<Spell> spells;
        public List<Caster> casters;
        public CoordsObject coords;

        private Character() {
            instance = this;
            this.spells = new List<Spell>();
            this.spells.Add(new Spell("Bloodstone Bolt I", 5525, false));
            this.spells.Add(new Spell("Bloodstone Bolt II", 5526, false));
            this.spells.Add(new Spell("Bloodstone Bolt III", 5527, false));
            this.spells.Add(new Spell("Bloodstone Bolt IV", 5528, false));
            this.spells.Add(new Spell("Bloodstone Bolt V", 5529, false));
            this.spells.Add(new Spell("Bloodstone Bolt VI", 5530, false));
            this.spells.Add(new Spell("Bloodstone Bolt VII", 5531, false));
            this.spells.Add(new Spell("Incantation of Bloodstone Bolt", 5532, false));
            this.spells.Add(new Spell("Hunter's Lash", 2970, false));
            this.spells.Add(new Spell("Burning Curse", 4716, false));
            this.spells.Add(new Spell("Ring of Death", 4239, false));
            this.spells.Add(new Spell("Exsanguinating Wave", 3940, false));
            this.spells.Add(new Spell("Corrupted Touch", 5980, false));
            this.spells.Add(new Spell("Ward of Rebirth", 3071, false));
            this.spells.Add(new Spell("Fellowship Heal I", 2981, false));
            this.spells.Add(new Spell("Blessing of Unity", 5314, false));
        }

        public bool IsSpellEnabled(string name) {
            if (spells.Exists(x => x.name == name && x.has == true))
                return true;
            
            return false;
        }

        public Caster SelectCasterByTarget(Enemy e) {
            List<Caster> slayers = casters.FindAll(x => x.slayer == e.race);

            if (slayers.Count > 0) {
                Caster best = slayers[0];

                foreach (Caster c in slayers) {
                    if (c.slayerMulti > best.slayerMulti)
                        best = c;
                }

                return best;
            }
            else {
                Caster best = casters[0];

                if (e.boss) {
                    foreach (Caster c in casters) {
                        if (c.critMulti > best.critMulti)
                            best = c;
                    }
                }
                else {
                    foreach (Caster c in casters) {
                        if (c.critChance > best.critChance)
                            best = c;
                    }
                }

                return best;
            }
        }
    }
}