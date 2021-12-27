using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public class World {
        public static World instance = new World();
        public Dictionary<int, Enemy> enemies;

        private World() {
            enemies = new Dictionary<int, Enemy>();
            Logger.LogMessage("World tracker created.");
        }

        public void WorldFilter_CreateObject(object sender, CreateObjectEventArgs e) {
            if (e.New.ObjectClass != ObjectClass.Monster) return;
            if (enemies.ContainsKey(e.New.Id)) return;
            Enemy enemy = new Enemy(e.New);
            enemies.Add(e.New.Id, enemy);
            Caster c = Character.instance.SelectCasterByTarget(enemy);
            Logger.LogMessage("Enemy '" + enemy.name + "' added. Initial distance from player is " + DistanceBetween(e.New.Coordinates(), Character.instance.coords).ToString() + ". Caster = " + c.name + ".");
        }

        public void WorldFilter_ChangeObject(object sender, ChangeObjectEventArgs e) {
            if (enemies.ContainsKey(e.Changed.Id))
                enemies[e.Changed.Id].coords = e.Changed.Coordinates();
            else if (e.Changed.Id == Character.instance.id)
                Character.instance.coords = e.Changed.Coordinates();
            else
                return;
            
            // Update distance from player, trigger events if needed
        }

        public void WorldFilter_ReleaseOject(object sender, ReleaseObjectEventArgs e) {
            if (enemies.ContainsKey(e.Released.Id)) {
                string name = enemies[e.Released.Id].name;
                enemies.Remove(e.Released.Id);
                Logger.LogMessage("Enemy '" + name + "' released.");
            }
        }

        public static double DistanceBetween(CoordsObject a, CoordsObject b) {
            double nsDiff = (((a.NorthSouth * 10) + 1019.5) * 24) - (((b.NorthSouth * 10) + 1019.5) * 24);
            double esDiff = (((a.EastWest * 10) + 1019.5) * 24) - (((b.EastWest * 10) + 1019.5) * 24);
            return Math.Abs(Math.Sqrt(Math.Pow(Math.Abs(nsDiff), 2) + Math.Pow(Math.Abs(esDiff), 2)));
        }
    }
}