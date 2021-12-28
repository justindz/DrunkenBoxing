using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public enum State
    {
        Disabled,
        Ready,
        Unwielding,
        WieldingCaster,
        SwitchingToMagicMode,
        Casting,
    }
    class Character {
        public static Character instance = new Character();
        public int id;
        public Dictionary<string, Spell> spells;
        public List<Caster> casters;
        public Position position;
        public State state;
        public Queue<Enemy> combatants;
        public List<Enemy> combatantsRingRange;
        public Spell lastSpellCast;
        public DateTime lastSpellCastShouldBeDoneAfter;

        private Character() {
            instance = this;
            state = State.Disabled;
            spells = Spell.BuildSpellTable();            
            combatants = new Queue<Enemy>();
            combatantsRingRange = new List<Enemy>();
            lastSpellCast = null;
            lastSpellCastShouldBeDoneAfter = DateTime.MinValue;
            state = State.Ready;
        }

        public void Update(double deltaTime) {
            try {
                if (state == State.Disabled) return;

                if (state == State.Casting && (DateTime.UtcNow.CompareTo(lastSpellCastShouldBeDoneAfter) > 0))
                    state = State.Ready;
                
                if (combatants.Count == 0) return;
                if (CoreManager.Current.Actions.BusyState != 0) return;

                if (state == State.Ready) {
                    int casterId = SelectCasterByTarget(combatants.Peek()).id;
                    int wieldedId = GetWieldedCasterId();

                    if (wieldedId == -1) {
                        CoreManager.Current.Actions.AutoWield(casterId);
                        state = State.WieldingCaster;
                        // Logger.LogMessage("I need to equip the caster for this target.");
                    }
                    else if (wieldedId != -1 && wieldedId != casterId) {
                        CoreManager.Current.Actions.MoveItem(wieldedId, id, 0, false);
                        state = State.Unwielding;
                        // Logger.LogMessage("I need to unequip the current caster for this target.");
                    }
                    else if (CoreManager.Current.Actions.CombatMode != CombatState.Magic) {
                        state = State.SwitchingToMagicMode;
                        Decal.Adapter.CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                        // Logger.LogMessage("I need to get into magic combat mode.");
                    }
                    else {
                        state = State.Casting;
                        Spell toCast = spells["Incantation of Bloodstone Bolt"];

                        if (combatantsRingRange.Count >= Settings.instance.ringMinimumCount) {
                            toCast = spells["Ring of Death"];
                            Logger.LogMessage("There are " + combatantsRingRange.Count.ToString() + " enemies in ring range, so we're switching to a ring spell.");
                        }

                        lastSpellCast = toCast;
                        lastSpellCastShouldBeDoneAfter = DateTime.UtcNow.AddSeconds(toCast.animationSeconds);
                        CoreManager.Current.Actions.CastSpell(toCast.id, combatants.Peek().id);
                        // Logger.LogMessage("I'm casting " + lastSpellCast.id.ToString() + ".");
                    }
                }
                else if (state == State.WieldingCaster) {
                    int casterId = SelectCasterByTarget(combatants.Peek()).id;

                    if (GetWieldedCasterId() == casterId) {
                        state = State.Ready;
                        // Logger.LogMessage("I've equipped the right caster for the target and I'm ready to cast.");
                    }
                }
                else if (state == State.Unwielding) {
                    if (GetWieldedCasterId() == -1) {
                        state = State.Ready;
                        // Logger.LogMessage("I've unequipped an incorrect caster for the target and I'm ready to equip the right one.");
                    }
                }
                else if (state == State.SwitchingToMagicMode && CoreManager.Current.Actions.CombatMode == CombatState.Magic) {
                    state = State.Ready;
                    // Logger.LogMessage("I've switched to magic combat mode.");
                }
            } catch (Exception ex) { Logger.LogError("Character.Update=" + state.ToString(), ex); }
        }

        public void Core_ChatBoxMessage(object sender, ChatTextInterceptEventArgs e) {
            try {
                if (state == State.Casting && e.Color == 17 && e.Text.StartsWith("You say, \"" + lastSpellCast.text)) {
                    lastSpellCastShouldBeDoneAfter = DateTime.UtcNow.AddSeconds(lastSpellCast.animationSeconds); // Set a more accurate wait time in case of input lag
                    // Logger.LogMessage("Confirmed spell cast for " + lastSpellCast.id + ".");
                }
            } catch (Exception ex) {
                Logger.LogError("Character.Core_ChatBoxMessage=" + e.Text, ex);
            }
        }

        public Caster SelectCasterByTarget(Enemy e) {
            List<Caster> slayers = casters.FindAll(x => x.slayer == e.race);

            if (slayers.Count > 0) {
                Caster best = slayers[0];

                foreach (Caster c in slayers) {
                    if (c.slayerMulti > best.slayerMulti)
                        best = c;
                }

                return best;
            }
            else {
                Caster best = casters[0];

                if (e.boss) {
                    foreach (Caster c in casters) {
                        if (c.critMulti > best.critMulti)
                            best = c;
                    }
                }
                else {
                    foreach (Caster c in casters) {
                        if (c.critChance > best.critChance)
                            best = c;
                    }
                }

                return best;
            }
        }

        public int GetWieldedCasterId() {
            WorldObjectCollection inv = CoreManager.Current.WorldFilter.GetInventory();

            foreach (WorldObject item in inv) {
                if (item.ObjectClass == ObjectClass.WandStaffOrb) {
                    if (item.Values(LongValueKey.Wielder) == id) {
                        return item.Id;
                    }
                }
            }

            return -1;
        }

        public void UpdateCombatant(Enemy enemy) {
            if (!combatantsRingRange.Contains(enemy) && enemy.distanceFromPlayer <= Settings.instance.ringDistance)
                combatantsRingRange.Add(enemy);
            else if (combatantsRingRange.Contains(enemy) && enemy.distanceFromPlayer > Settings.instance.ringDistance)
                combatantsRingRange.Remove(enemy);

            if (!combatants.Contains(enemy) && enemy.distanceFromPlayer <= Settings.instance.fightDistance)
                combatants.Enqueue(enemy);
            else if (combatants.Contains(enemy) && enemy.distanceFromPlayer > Settings.instance.fightDistance)
                RemoveCombatant(enemy);
        }

        public void RemoveCombatant(Enemy enemy) {
            if (combatants.Contains(enemy)) {
                Queue<Enemy> newCombatants = new Queue<Enemy>();

                while (combatants.Count > 0) {
                    Enemy e = combatants.Dequeue();

                    if (e.id != enemy.id)
                        newCombatants.Enqueue(e);
                }

                combatants = newCombatants;
            }

            combatantsRingRange.Remove(enemy);
        }
    }
}