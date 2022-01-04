using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public class Caster {
        public int id;
        public string name;
        public bool cs;
        public bool cb;
        public Race slayer;
        public float slayerMulti;

        public Caster(int id, string name, bool cs = false, bool cb = false, int slayer = -1, float slayerBonus = 1.0f) {
            this.id = id;
            this.name = name;
            this.cs = cs;
            this.cb = cb;
            this.slayer = (Race)slayer;
            this.slayerMulti = slayerBonus;
        }

        public static Caster BuildFromWorldObject(WorldObject item) {
            Caster c = new Caster(item.Id, item.Name);
            int underlay = item.Values(LongValueKey.IconUnderlay, 0);

            if (underlay == 13144)
                c.cs = true;
            else if (underlay == 13143)
                c.cb = true;

            c.slayer = (item.LongKeys.Contains(166) ? (Race)item.LongKeys[166] : Race.None);
            c.slayerMulti = (item.DoubleKeys.Contains(138) ? item.DoubleKeys[138] : 1.0f);
            return c;
        }

        public override string ToString() {
            return name + " CS=" + cs.ToString() + " CB=" + cb.ToString() + " Slayer=" + slayer.ToString() + " SlayerMulti=" + slayerMulti.ToString();
        }
    }
}