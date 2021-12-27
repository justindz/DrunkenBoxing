using Decal.Adapter.Wrappers;

namespace DrunkenBoxing {
    public class Caster {
        public int id;
        public string name;
        public float critChance;
        public float critMulti;
        public Race slayer;
        public float slayerMulti;

        public Caster(int id, string name, float critChance = 0, float critMulti = 1.0f, int slayer = -1, float slayerBonus = 1.0f) {
            this.id = id;
            this.name = name;
            this.critChance = critChance;
            this.critMulti = critMulti;
            this.slayer = (Race)slayer;
            this.slayerMulti = slayerBonus;
        }

        public static Caster BuildFromWorldObject(WorldObject item) {
            Caster c = new Caster(item.Id, item.Name);
            c.critChance = (item.DoubleKeys.Contains(147) ? item.DoubleKeys[147] : 0f);
            c.critMulti = (item.DoubleKeys.Contains(136) ? item.DoubleKeys[136] : 1.0f);
            c.slayer = (item.LongKeys.Contains(166) ? (Race)item.LongKeys[166] : Race.None);
            c.slayerMulti = (item.DoubleKeys.Contains(138) ? item.DoubleKeys[138] : 1.0f);
            return c;
        }

        public override string ToString() {
            return name + " Crit%=" + critChance.ToString() + " CritMulti=" + critMulti.ToString() + " Slayer=" + slayer.ToString() + " SlayerMulti=" + slayerMulti.ToString();
        }
    }
}