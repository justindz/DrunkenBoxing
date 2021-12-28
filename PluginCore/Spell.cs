using System.Collections.Generic;

namespace DrunkenBoxing {
    public class Spell {
        public int id;
        public string text;
        public bool has;
        public double animationSeconds;
        public double effectDurationSeconds;

        public Spell(int id, string text, bool has, double animationSeconds, double effectDurationSeconds=0.0) {
            this.id = id;
            this.text = text;
            this.has = has;
            this.animationSeconds = animationSeconds;
            this.effectDurationSeconds = effectDurationSeconds;
        }

        public static Dictionary<string, Spell> BuildSpellTable() {
            Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
            spells.Add("Bloodstone Bolt I", new Spell(5525, "Slavu Zhapaj", false, 1.0));
            spells.Add("Bloodstone Bolt II", new Spell(5526, "Slavu Zhapaj", false, 1.1));
            spells.Add("Bloodstone Bolt III", new Spell(5527, "Slavu Zhapaj", false, 1.2));
            spells.Add("Bloodstone Bolt IV", new Spell(5528, "Slavu Zhapaj", false, 1.3));
            spells.Add("Bloodstone Bolt V", new Spell(5529, "Slavu Zhapaj", false, 1.4));
            spells.Add("Bloodstone Bolt VI", new Spell(5530, "Slavu Zhapaj", false, 1.5));
            spells.Add("Bloodstone Bolt VII", new Spell(5531, "Slavu Zhapaj", false, 1.6));
            spells.Add("Incantation of Bloodstone Bolt", new Spell(5532, "Slavu Zhapaj", false, 1.7));
            spells.Add("Hunter's Lash", new Spell(2970, "Tugak Quati", false, 1.2));
            spells.Add("Burning Curse", new Spell(4716, "Equin Zhapaj", false, 1.6));
            spells.Add("Ring of Death", new Spell(4239, "Tugak Zha", false, 3.5));
            spells.Add("Exsanguinating Wave", new Spell(3940, "Tugak Zhapaj", false, 1.6));
            spells.Add("Corrupted Touch", new Spell(5980, "Helkas Quasith", false, 1.7, 30.0));
            // spells.Add("Ward of Rebirth", new Spell(3071, false, 3600.0));
            // spells.Add("Fellowship Heal I", new Spell(2981, false));
            // spells.Add("Blessing of Unity", new Spell(5314, false, 50.0));
            return spells;
        }
    }
}