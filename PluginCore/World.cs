using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public struct Position {
        public int landcell;
        public float x;
        public float y;
        public float z;

        public Position(int landcell, float x, float y, float z) {
            this.landcell = landcell;
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return "[" + landcell + "] " + x + "|" + y + "|" + z;
        }
    }
    public class World {
        public static World instance = new World();
        public Dictionary<int, Enemy> enemies;

        private World() {
            enemies = new Dictionary<int, Enemy>();
        }

        public void WorldFilter_CreateObject(object sender, CreateObjectEventArgs e) {
            try {
                if (e.New.ObjectClass == ObjectClass.Monster) {
                    if (enemies.ContainsKey(e.New.Id)) return;

                    Enemy enemy = new Enemy(e.New);
                    enemy.position = new Position(0, (float)e.New.Offset().X, (float)e.New.Offset().Y, (float)e.New.Offset().Z);
                    enemy.distanceFromPlayer = World.DistanceBetween(enemy.position, Character.instance.position);
                    enemies.Add(e.New.Id, enemy);

                    if (enemy.distanceFromPlayer <= Settings.instance.fightDistance)
                        Character.instance.UpdateCombatant(enemy);
                }
                else if (e.New.ObjectClass == ObjectClass.Player && e.New.Id == Character.instance.id) {
                    Character.instance.position = new Position(0, (float)e.New.Offset().X, (float)e.New.Offset().Y, (float)e.New.Offset().Z);;
                    Logger.LogMessage("Character added at " + Character.instance.position.ToString() + ".");
                }
            } catch (Exception ex) { Logger.LogError("World.WorldFilter_Create_Object=" + e.New.Name, ex); }
        }

        public void EchoFilter_ServerDispatch(object sender, NetworkMessageEventArgs e) {
            try {
                if (e.Message.Type == 0xF748) { // Set Position and Motion
                    int id = e.Message.Value<int>("object");
                    Position pos = PositionStructToPosition(e.Message.Struct("position"));

                    if (Character.instance.id == id) {
                        Character.instance.position = pos;

                        foreach (KeyValuePair<int, Enemy> en in World.instance.enemies) {
                            Enemy enemy = en.Value;
                            enemy.distanceFromPlayer = World.DistanceBetween(Character.instance.position, enemy.position);
                            Character.instance.UpdateCombatant(enemy);
                        }
                    }
                    else if (enemies.ContainsKey(id)) {
                        enemies[id].position = pos;
                        enemies[id].distanceFromPlayer = World.DistanceBetween(Character.instance.position, pos);
                        Character.instance.UpdateCombatant(enemies[id]);
                    } else return;
                }
            } catch (Exception ex) { Logger.LogError("World.EchoFilter_ServerDispatch=" + e.Message.Type.ToString(), ex); }
        }

        public void WorldFilter_ReleaseOject(object sender, ReleaseObjectEventArgs e) {
            if (enemies.ContainsKey(e.Released.Id)) {
                Character.instance.RemoveCombatant(enemies[e.Released.Id]);
                enemies.Remove(e.Released.Id);
            }
        }

        public static Position PositionStructToPosition(MessageStruct position) {
            return new Position(position.Value<int>("landcell"), position.Value<float>("x"), position.Value<float>("y"), position.Value<float>("z"));
        }

        public static double DistanceBetween(Position a, Position b) {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y) + Math.Abs(a.z - b.z);
        }
    }
}