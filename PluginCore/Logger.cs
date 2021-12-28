using System;
using System.IO;

namespace DrunkenBoxing {
    class Logger {
        public static void LogMessage(string message) {
            StreamWriter sw = new StreamWriter("DrunkenBoxing_" + DateTime.Today.ToString("D").Replace(", ", "_") + ".log", true);
            sw.Write(DateTime.Now.ToString());
            sw.Write("[MESSAGE]: ");
            sw.Write(message + "\n");
            sw.Close();
        }

        public static void LogError(string context, Exception ex) {
            StreamWriter sw = new StreamWriter("DrunkenBoxing_" + DateTime.Today.ToString("D").Replace(", ", "_") + ".log", true);
            sw.Write(DateTime.Now.ToString());
            sw.Write("[ERROR - " + context + "]: ");
            sw.Write(ex.Message + "\n");
            sw.Close();
        }
    }
}