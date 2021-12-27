using System;

namespace DrunkenBoxing {
    public class Spell {
        public string name;
        public int id;
        public bool has;

        public Spell(string name, int id, bool has) {
            this.name = name;
            this.id = id;
            this.has = has;
        }
    }
}