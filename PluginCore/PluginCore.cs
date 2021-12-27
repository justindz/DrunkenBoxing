using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public class PluginCore : PluginBase {
        public void Chat(string str) {
            Host.Actions.AddChatText("[DB] " + str, 1);
            Logger.LogMessage(str);
        }

        protected override void Startup() {
            Core.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;
            Core.WorldFilter.CreateObject += World.instance.WorldFilter_CreateObject;
            Core.WorldFilter.ChangeObject += World.instance.WorldFilter_ChangeObject;
            Core.WorldFilter.ReleaseObject += World.instance.WorldFilter_ReleaseOject;
            Core.CommandLineText += Core_CommandLineText;
        }

        protected override void Shutdown()
        {
            Core.CharacterFilter.LoginComplete -= CharacterFilter_LoginComplete;
            Core.WorldFilter.CreateObject -= World.instance.WorldFilter_CreateObject;
            Core.WorldFilter.ChangeObject -= World.instance.WorldFilter_ChangeObject;
            Core.WorldFilter.ReleaseObject -= World.instance.WorldFilter_ReleaseOject;
            Core.CommandLineText -= Core_CommandLineText;
            Logger.LogMessage("DrunkenBoxing time has ended. For now.\n");
        }

        private void Core_CommandLineText(object sender, ChatParserInterceptEventArgs e) {
            try {
                if (e.Text.StartsWith("/db ")) {
                    e.Eat = true;
                    string command = e.Text.Substring(4);

                    if (command.StartsWith("update")) {
                        string noun = command.Split(' ')[1];

                        if (noun.StartsWith("spells"))
                            DetectMountainRetreatSpells();
                        else if (noun.StartsWith("casters"))
                            DetectCasters();
                    }
                    else if (command.StartsWith("test")) {
                        string noun = command.Split(' ')[1];

                        if (noun.StartsWith("caster")) {
                            WorldObject sel = Core.WorldFilter[Host.Actions.CurrentSelection];
                            
                            if (sel == null || sel.ObjectClass != ObjectClass.Monster) {
                                Chat("Select a monster to test caster selection.");
                                return;
                            }

                            Chat("I would use " + Character.instance.SelectCasterByTarget(World.instance.enemies[sel.Id]).name + " on that target.");
                        }
                    }
                    else
                        Chat("Unrecognized command: " + command);
                }
            } catch (Exception ex) {
                e.Eat = true;
                Logger.LogError(ex);
            }
        }

        private void CharacterFilter_LoginComplete(object sender, EventArgs e) {
            Chat("It's time for DrunkenBoxing!");
            
            try {
                Character.instance.id = Core.CharacterFilter.Id;
                Character.instance.coords = Core.WorldFilter[Character.instance.id].Coordinates();
                DetectMountainRetreatSpells();
                DetectCasters();
            }
            catch (Exception ex) { Logger.LogError(ex); }
        }

        private void DetectMountainRetreatSpells() {
            Logger.LogMessage("Updating Mountain Retreat Life spells list...");
            List<Spell> newSpells = new List<Spell>();

            Character.instance.spells.ForEach(
                delegate(Spell sp) {
                    if (Core.CharacterFilter.IsSpellKnown(sp.id)) {
                        newSpells.Add(new Spell(sp.name, sp.id, true));
                        Chat("Enabled: " + sp.name);
                    }
                    else {
                        newSpells.Add(sp);
                        Chat("Skipped: " + sp.name);
                    }
                }
            );

            Character.instance.spells = newSpells;
        }

        private void DetectCasters() {
            Logger.LogMessage("Updating Life magic casters...");
            WorldObjectCollection inv = Core.WorldFilter.GetInventory();
            Character.instance.casters = new List<Caster>();

            foreach (WorldObject item in inv) {
                if (item.ObjectClass == ObjectClass.WandStaffOrb) {
                    if (!item.LongKeys.Contains(159) || item.LongKeys[159] == 33) {
                        Caster c = Caster.BuildFromWorldObject(item);
                        Character.instance.casters.Add(c);
                        Chat("Added life caster: " + c.ToString());
                    }
                    else
                        Chat("Skipped caster: " + item.Name);
                }
            }
        }
    }
}
