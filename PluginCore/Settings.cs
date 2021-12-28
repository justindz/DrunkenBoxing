using System;

namespace DrunkenBoxing {
    public class Settings {
        public static Settings instance = new Settings();

        public double fightDistance;
        public double ringDistance;
        public int ringMinimumCount;

        private Settings() {
            fightDistance = 6.0;
            ringDistance = 4.0;
            ringMinimumCount = 3;
        }
    }
}