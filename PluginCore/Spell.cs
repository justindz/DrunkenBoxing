using System.Collections.Generic;

namespace DrunkenBoxing {
    public class Spell {
        public int id;
        public int level;
        public string text;
        public bool has;
        public double animationSeconds;
        public double effectDurationSeconds;

        public Spell(int id, int level, string text, bool has, double animationSeconds, double effectDurationSeconds=0.0) {
            this.id = id;
            this.level = level;
            this.text = text;
            this.has = has;
            this.animationSeconds = animationSeconds;
            this.effectDurationSeconds = effectDurationSeconds;
        }

        public static Dictionary<string, Spell> BuildSpellTable() {
            Dictionary<string, Spell> spells = new Dictionary<string, Spell>();
            
            #region Used
            spells.Add("Bloodstone Bolt I", new Spell(5525, 1, "Slavu Zhapaj", false, 1.0));
            spells.Add("Bloodstone Bolt II", new Spell(5526, 2, "Slavu Zhapaj", false, 1.1));
            spells.Add("Bloodstone Bolt III", new Spell(5527, 3, "Slavu Zhapaj", false, 1.2));
            spells.Add("Bloodstone Bolt IV", new Spell(5528, 4, "Slavu Zhapaj", false, 1.3));
            spells.Add("Bloodstone Bolt V", new Spell(5529, 5, "Slavu Zhapaj", false, 1.4));
            spells.Add("Bloodstone Bolt VI", new Spell(5530, 6, "Slavu Zhapaj", false, 1.5));
            spells.Add("Bloodstone Bolt VII", new Spell(5531, 7, "Slavu Zhapaj", false, 1.6));
            spells.Add("Incantation of Bloodstone Bolt", new Spell(5532, 8, "Slavu Zhapaj", false, 1.7));
            spells.Add("Ring of Death", new Spell(4239, 6, "Tugak Zha", false, 3.5));
            spells.Add("Corrupted Touch", new Spell(5980, 8, "Helkas Quasith", false, 1.7, 30.0));
            // spells.Add("Ward of Rebirth", new Spell(3071, false, 3600.0));
            // spells.Add("Blessing of Unity", new Spell(5314, false, 50.0));
            #endregion

            #region Unused
            // spells.Add("Hunter's Lash", new Spell(2970, 3, "Tugak Quati", false, 1.2));
            // spells.Add("Burning Curse", new Spell(4716, 7, "Equin Zhapaj", false, 1.6));
            // spells.Add("Exsanguinating Wave", new Spell(3940, 8, "Tugak Zhapaj", false, 1.6));
            // spells.Add("Fellowship Heal I", new Spell(2981, 1, false));
            #endregion

            return spells;
        }
    }
}