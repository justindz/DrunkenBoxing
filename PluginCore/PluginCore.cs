using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public class PluginCore : PluginBase {
        private DateTime lastInterval = DateTime.MinValue;

        public void Chat(string str) {
            Host.Actions.AddChatText("[DB] " + str, 1);
            Logger.LogMessage(str);
        }

        protected override void Startup() {
            Core.RenderFrame += Core_RenderFrame;
            Core.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;
            Core.WorldFilter.CreateObject += World.instance.WorldFilter_CreateObject;
            Core.EchoFilter.ServerDispatch += World.instance.EchoFilter_ServerDispatch;
            Core.EchoFilter.ServerDispatch += Fellowship.instance.EchoFilter_ServerDispatch;
            Core.WorldFilter.ReleaseObject += World.instance.WorldFilter_ReleaseOject;
            Core.CommandLineText += Core_CommandLineText;
            Core.ChatBoxMessage += Character.instance.Core_ChatBoxMessage;
        }

        protected override void Shutdown()
        {
            Logger.LogMessage("DrunkenBoxing time has ended. For now.\n");
            Core.CharacterFilter.LoginComplete -= CharacterFilter_LoginComplete;
            Core.WorldFilter.CreateObject -= World.instance.WorldFilter_CreateObject;
            Core.EchoFilter.ServerDispatch -= World.instance.EchoFilter_ServerDispatch;
            Core.EchoFilter.ServerDispatch -= Fellowship.instance.EchoFilter_ServerDispatch;
            Core.WorldFilter.ReleaseObject -= World.instance.WorldFilter_ReleaseOject;
            Core.CommandLineText -= Core_CommandLineText;
            Core.ChatBoxMessage -= Character.instance.Core_ChatBoxMessage;
            Core.RenderFrame -= Core_RenderFrame;
        }

        private void Core_RenderFrame(object sender, EventArgs e) {
            try {
                if (DateTime.UtcNow - lastInterval >= TimeSpan.FromMilliseconds(100.0)) {
                    double deltaTime = DateTime.UtcNow.Subtract(lastInterval).Milliseconds;
                    lastInterval = DateTime.UtcNow;
                    Character.instance.Update(deltaTime);
                }
            } catch (Exception ex) { Logger.LogError("PluginCore.Core_RenderFrame", ex); }
        }

        private void Core_CommandLineText(object sender, ChatParserInterceptEventArgs e) {
            try {
                if (e.Text.StartsWith("/db ")) {
                    e.Eat = true;
                    string command = e.Text.Substring(4);

                    if (command.StartsWith("off")) {
                        Character.instance.state = State.Disabled;
                        Chat("DrunkenBoxing disabled. Use \"/db on\" to re-enable.");
                    }
                    else if (command.StartsWith("on")) {
                        Character.instance.state = State.Ready;
                        Chat("DrunkenBoxing enabled. Use \"/db off\" to disable.");
                    }
                    else if (command.StartsWith("dump")) {
                        Chat("Dumping settings to file.");
                        Settings.instance.Dump(Core.CharacterFilter.Name);
                    }
                    else if (command.StartsWith("load")) {
                        Chat("Loading settings from file.");
                        
                        if (Settings.instance.Load(Core.CharacterFilter.Name)) {
                            Chat("Loading settings succeeded.");

                            if (Settings.instance.pveCaster != int.MinValue)
                                Character.instance.state = State.Ready;
                            else {
                                Character.instance.state = State.Disabled;
                                Chat("DrunkenBoxing disabled. You need to '/db set caster pve' and then '/db on' to enable.");
                            }
                        }
                        else
                            Chat("No settings file found for this character, or settings file format issue (see logs).");
                    }
                    else if (command.StartsWith("update")) {
                        string noun = command.Split(' ')[1];

                        if (noun.StartsWith("spells"))
                            DetectMountainRetreatSpells();
                    }
                    else if (command.StartsWith("set")) {
                        string noun = command.Split(' ')[1];

                        if (noun == "caster") {
                            string subject = command.Split(' ')[2];

                            if (subject == "pve") {
                                WorldObject sel = Core.WorldFilter[Host.Actions.CurrentSelection];

                                if (Caster.IsCaster(sel)) {
                                    Settings.instance.pveCaster = sel.Id;
                                    Chat(sel.Name + " set as PVE caster.");
                                }
                                else
                                    Chat("'" + sel.Name + "' is not a valid caster.");
                            }
                            else if (subject == "boss") {
                                WorldObject sel = Core.WorldFilter[Host.Actions.CurrentSelection];

                                if (Caster.IsCaster(sel)) {
                                    Settings.instance.bossCaster = sel.Id;
                                    Chat(sel.Name + " set as boss caster.");
                                }
                                else
                                    Chat("'" + sel.Name + "' is not a valid caster.");
                            }
                            else {
                                try {
                                    Race race = (Race)Enum.Parse(typeof(Race), noun, true);
                                    WorldObject sel = Core.WorldFilter[Host.Actions.CurrentSelection];

                                    if (Caster.IsCaster(sel)) {
                                        Settings.instance.slayerCasters[race] = sel.Id;
                                        Chat(sel.Name + " set as caster for " + race.ToString() + ".");
                                    }
                                    else
                                        Chat("'" + sel.Name + "' is not a valid caster.");
                                }
                                catch (ArgumentNullException) {
                                    Chat("'" + subject + "' is not a valid race.");
                                }
                            }
                        }
                    }
                    else if (command.StartsWith("test")) {
                        string noun = command.Split(' ')[1];

                        if (noun.StartsWith("distance")) {
                            WorldObject sel = Core.WorldFilter[Host.Actions.CurrentSelection];

                            if (sel == null || sel.ObjectClass != ObjectClass.Monster) {
                                Chat("Select a monster to test distance.");
                                return;
                            }

                            Chat("You are " + World.DistanceBetween(Character.instance.position, World.instance.enemies[sel.Id].position).ToString() + " from that.");
                        }
                        else if (noun.StartsWith("enemies")) {
                            string output = "Current tracked enemies are:";

                            foreach (KeyValuePair<int, Enemy> kvp in World.instance.enemies) {
                                output += " " + kvp.Value.name + ",";
                            }

                            Chat(output.TrimEnd(','));
                        }
                        else if (noun.StartsWith("combatants")) {
                            string output = "Current combatants are:";

                            foreach (KeyValuePair<Priority, Queue<Enemy>> kvp in Character.instance.combatants) {
                                foreach (Enemy combatant in kvp.Value) {
                                    output += " " + combatant.name + ",";
                                }
                            }

                            Chat(output.TrimEnd(','));
                            Chat("The next one I want to eff up is " + Character.instance.GetNextTarget().name);
                            Chat("There are " + Character.instance.combatantsRingRange.Count.ToString() + " enemies in ring range.");
                        }
                        else if (noun.StartsWith("state"))
                            Chat("State=" + Character.instance.state.ToString());
                    }
                    else
                        Chat("Unrecognized command: " + command);
                }
            } catch (Exception ex) {
                e.Eat = true;
                Logger.LogError("PluginCore.CoreCommandLineText=" + e.Text, ex);
            }
        }

        private void CharacterFilter_LoginComplete(object sender, EventArgs e) {
            try {
                Character.instance.id = Core.CharacterFilter.Id;
                Character.instance.name = Core.CharacterFilter.Name;
                
                if (Settings.instance.Load(Core.CharacterFilter.Name))
                    Chat("Loading settings succeeded.");
                else
                    Chat("No settings file found for this character.");

                DetectMountainRetreatSpells();
                
                if (Settings.instance.pveCaster != int.MinValue)
                    Character.instance.state = State.Ready;
                else
                    Chat("DrunkenBoxing disabled. You need to '/db set caster pve' and then '/db on' to enable.");
            }
            catch (Exception ex) { Logger.LogError("PluginCore.CharacterFilter_LoginComplete", ex); }
        }

        private void DetectMountainRetreatSpells() {
            Logger.LogMessage("Updating Mountain Retreat Life spells list...");

            foreach (KeyValuePair<string, Spell> entry in Character.instance.spells) {
                if (Core.CharacterFilter.IsSpellKnown(entry.Value.id)) {
                    Character.instance.spells[entry.Key].has = true;
                    Chat("Enabled: " + entry.Key);
                }
            }
        }
    }
}
