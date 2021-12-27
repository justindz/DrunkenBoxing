using Decal.Adapter.Wrappers;
using System;

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

    public class Enemy {
        public int id;
        public string name;
        public Race race;
        public bool boss;
        public CoordsObject coords;
        public double distanceFromPlayer;

        public Enemy(WorldObject e) {
            this.id = e.Id;
            this.name = e.Name;
            this.race = (Enum.IsDefined(typeof(Race), e.LongKeys[2]) ? (Race)e.LongKeys[2] : Race.None);
            this.boss = false;
            this.coords = e.Coordinates();
        }
    }
}