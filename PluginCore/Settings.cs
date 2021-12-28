using System;

namespace DrunkenBoxing {
    public class Settings {
        public static Settings instance = new Settings();

        public double fightDistance;

        private Settings() {
            fightDistance = 6.0;
        }
    }
}