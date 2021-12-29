using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public enum Race
    {
        None = -1,
        Olthoi = 1,
        Banderling = 2,
        Drudge = 3,
        Lugian = 5,
        Tumerok = 6,
        Golem = 13,
        Undead = 14,
        Tusker = 16,
        Virindi = 19,
        Wisp = 20,
        Shadow = 22,
        Zefir = 29,
        Skeleton = 30,
        Human = 31,
        Niffis = 45,
        Elemental = 61,
        Burun = 75,
        Ghost = 77,
        Viamontian = 83,
        Mukkir = 89,
        Anekshay = 101,
    }
    
    public enum Priority {
        Rage,
        Focus,
        Normal,
        Last,
        Never,
    }

    public class Enemy {
        public int id;
        public string name;
        public Race race;
        public Priority priority;
        public bool boss;
        public Position position;
        public double distanceFromPlayer;

        public Enemy(WorldObject e) {
            this.id = e.Id;
            this.name = e.Name;
            this.race = (Enum.IsDefined(typeof(Race), e.LongKeys[2]) ? (Race)e.LongKeys[2] : Race.None);
            this.priority = Priority.Normal;
            this.boss = Settings.instance.bosses.Contains(this.name);
        }
    }
}