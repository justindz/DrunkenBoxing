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
        public string name;
        public Dictionary<string, Spell> spells;
        public Position position;
        public State state;
        public Dictionary<Priority, Queue<Enemy>> combatants;
        public List<Enemy> combatantsRingRange;
        private Spell lastSpellCast;
        private DateTime lastSpellCastShouldBeDoneAfter;
        private Dictionary<int, DateTime> corruptedTouches;
        private DateTime wardOfRebirthExpires;
        private double timeInterval;
        private int lastWieldedCasterId;
        private bool updateLastWieldedCasterId;

        private Character() {
            instance = this;
            state = State.Disabled;
            spells = Spell.BuildSpellTable();
            combatants = new Dictionary<Priority, Queue<Enemy>>(5);
            combatants.Add(Priority.Rage, new Queue<Enemy>());
            combatants.Add(Priority.Focus, new Queue<Enemy>());
            combatants.Add(Priority.Normal, new Queue<Enemy>());
            combatants.Add(Priority.Last, new Queue<Enemy>());
            combatants.Add(Priority.Never, new Queue<Enemy>());
            combatantsRingRange = new List<Enemy>();
            lastSpellCast = null;
            lastSpellCastShouldBeDoneAfter = DateTime.MinValue;
            corruptedTouches = new Dictionary<int, DateTime>();
            wardOfRebirthExpires = DateTime.MinValue;
            timeInterval = 0.0;
            lastWieldedCasterId = 0;
            updateLastWieldedCasterId = true;
        }

        public void Update(double deltaTime) {
            try {
                if (state == State.Disabled) return;

                timeInterval += deltaTime;

                if (timeInterval >= 1000.0) {
                    if (corruptedTouches.Count > 0) {
                        List<int> toRemove = new List<int>();

                        foreach (KeyValuePair<int, DateTime> kvp in corruptedTouches) {
                            if (DateTime.UtcNow.CompareTo(kvp.Value) > 0)
                                toRemove.Add(kvp.Key);
                        }

                        foreach (int trk in toRemove) {
                            corruptedTouches.Remove(trk);
                            // Logger.LogMessage("Corrupted Touch expired on " + trk + ".");
                        }
                    }

                    timeInterval = 0.0;
                }

                if (state == State.Casting && (DateTime.UtcNow.CompareTo(lastSpellCastShouldBeDoneAfter) > 0))
                    state = State.Ready;
                
                if (CoreManager.Current.Actions.BusyState != 0) return;

                if (state == State.Ready) {
                    Enemy nextTarget = GetNextTarget();
                    int wieldedId = GetWieldedCasterId();

                    if (nextTarget != null && wieldedId == -1) {
                        CoreManager.Current.Actions.AutoWield(SelectCasterIdByTarget(nextTarget));
                        state = State.WieldingCaster;
                        updateLastWieldedCasterId = true;
                        // Logger.LogMessage("I need to equip the caster for this target.");
                    }
                    else if (nextTarget != null && wieldedId != -1 && wieldedId != SelectCasterIdByTarget(nextTarget)) {
                        CoreManager.Current.Actions.MoveItem(wieldedId, id, 0, false);
                        state = State.Unwielding;
                        updateLastWieldedCasterId = true;
                        // Logger.LogMessage("I need to unequip the current caster for this target.");
                    }
                    else if (WeHaveSomethingToCast() && CoreManager.Current.Actions.CombatMode != CombatState.Magic) {
                        state = State.SwitchingToMagicMode;
                        Decal.Adapter.CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                        // Logger.LogMessage("I need to get into magic combat mode.");
                    }
                    else {
                        if (TimeToCastWardOfRebirth())
                            CastWardOfRebirth();
                        else if (spells["Ring of Death"].has && combatantsRingRange.Count >= Settings.instance.ringMinimumCount) // TODO pull out method
                            CastRingOfDeath(nextTarget);
                        else if (nextTarget != null && spells["Corrupted Touch"].has && Settings.instance.dots.Contains(nextTarget.name) && !corruptedTouches.ContainsKey(nextTarget.id)) // TODO pull out method
                            CastCorruptedTouch(nextTarget);
                        else if (nextTarget != null)
                            CastBolt(nextTarget);
                    }
                }
                else if (state == State.WieldingCaster) {
                    int casterId = SelectCasterIdByTarget(GetNextTarget());

                    if (GetWieldedCasterId() == casterId) {
                        state = State.Ready;
                        updateLastWieldedCasterId = false;
                        // Logger.LogMessage("I've equipped the right caster for the target and I'm ready to cast.");
                    }
                }
                else if (state == State.Unwielding) {
                    if (GetWieldedCasterId() == -1) {
                        state = State.Ready;
                        updateLastWieldedCasterId = true;
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

        public Enemy GetNextTarget() {
            if (combatants[Priority.Rage].Count > 0)
                return combatants[Priority.Rage].Peek();
            else if (combatants[Priority.Focus].Count > 0)
                return combatants[Priority.Focus].Peek();
            else if (combatants[Priority.Normal].Count > 0)
                return combatants[Priority.Normal].Peek();
            else if (combatants[Priority.Last].Count > 0)
                return combatants[Priority.Last].Peek();

            return null;
        }

        public bool WeHaveSomethingToCast() {
            if (GetNextTarget() != null)
                return true;
            else if (TimeToCastWardOfRebirth())
                return true;

            return false;
        }

        public bool TimeToCastWardOfRebirth() {
            if (spells["Ward of Rebirth"].has && Fellowship.instance.inFellowship && Fellowship.instance.members.Count > 1 && (Fellowship.instance.membershipHasChanged || DateTime.UtcNow.CompareTo(wardOfRebirthExpires) > 0))
                return true;

            return false;
        }

        public bool IsTracked(Enemy e) {
            if (combatants[Priority.Rage].Contains(e))
                return true;
            else if (combatants[Priority.Focus].Contains(e))
                return true;
            else if (combatants[Priority.Normal].Contains(e))
                return true;
            else if (combatants[Priority.Last].Contains(e))
                return true;
            else if (combatants[Priority.Never].Contains(e))
                return true;

            return false;
        }

        public int SelectCasterIdByTarget(Enemy e) {
            if (e == null)
                return Settings.instance.pveCaster;

            if (Settings.instance.slayerCasters[e.race] != int.MinValue)
                return Settings.instance.slayerCasters[e.race];
            
            if (e.boss && Settings.instance.bossCaster != int.MinValue)
                return Settings.instance.bossCaster;

            return Settings.instance.pveCaster;
        }

        public int GetWieldedCasterId() {
            if (updateLastWieldedCasterId) {
                WorldObjectCollection inv = CoreManager.Current.WorldFilter.GetInventory();

                foreach (WorldObject item in inv) {
                    if (item.ObjectClass == ObjectClass.WandStaffOrb) {
                        if (item.Values(LongValueKey.Wielder) == id) {
                            lastWieldedCasterId = item.Id;
                            return lastWieldedCasterId;
                        }
                    }
                }

                lastWieldedCasterId = -1;
                return lastWieldedCasterId;
            }
            else
                return lastWieldedCasterId;
        }

        public void UpdateCombatant(Enemy enemy) {
            if (!combatantsRingRange.Contains(enemy) && enemy.distanceFromPlayer <= Settings.instance.ringDistance)
                combatantsRingRange.Add(enemy);
            else if (combatantsRingRange.Contains(enemy) && enemy.distanceFromPlayer > Settings.instance.ringDistance)
                combatantsRingRange.Remove(enemy);

            if (!IsTracked(enemy) && enemy.distanceFromPlayer <= Settings.instance.fightDistance)
                combatants[enemy.priority].Enqueue(enemy);
            else if (IsTracked(enemy) && enemy.distanceFromPlayer > Settings.instance.fightDistance)
                RemoveCombatant(enemy);
        }

        public void RemoveCombatant(Enemy enemy) {
            if (IsTracked(enemy)) {
                Queue<Enemy> newCombatants = new Queue<Enemy>();

                while (combatants[enemy.priority].Count > 0) {
                    Enemy e = combatants[enemy.priority].Dequeue();

                    if (e.id != enemy.id)
                        newCombatants.Enqueue(e);
                }

                combatants[enemy.priority] = newCombatants;
            }

            combatantsRingRange.Remove(enemy);
        }

        private void SetCastTrackingStates(Spell s) {
            state = State.Casting;
            lastSpellCast = s;
            lastSpellCastShouldBeDoneAfter = DateTime.UtcNow.AddSeconds(s.animationSeconds);
        }

        #region Actions
        private void CastBolt(Enemy target) {
            Spell toCast = spells["Incantation of Bloodstone Bolt"]; // TODO select bolt by skill level
            SetCastTrackingStates(toCast);
            CoreManager.Current.Actions.CastSpell(toCast.id, target.id);
            // Logger.LogMessage("I'm casting a Bolt.");
        }

        private void CastRingOfDeath(Enemy target) {
            Spell toCast = spells["Ring of Death"];
            SetCastTrackingStates(toCast);
            CoreManager.Current.Actions.CastSpell(toCast.id, target.id);
            // Logger.LogMessage("There are " + combatantsRingRange.Count.ToString() + " enemies in ring range, so we're using a ring spell.");
        }

        private void CastCorruptedTouch(Enemy target) {
            Spell toCast = spells["Corrupted Touch"];
            SetCastTrackingStates(toCast);
            corruptedTouches.Add(target.id, DateTime.UtcNow.AddSeconds(toCast.animationSeconds + toCast.effectDurationSeconds));
            CoreManager.Current.Actions.CastSpell(toCast.id, target.id);
            // Logger.LogMessage("I'm casting Corrupted Touch.");
        }

        private void CastWardOfRebirth() {
            Spell toCast = spells["Ward of Rebirth"];
            SetCastTrackingStates(toCast);
            wardOfRebirthExpires = DateTime.UtcNow.AddSeconds(toCast.animationSeconds + toCast.effectDurationSeconds);
            CoreManager.Current.Actions.CastSpell(toCast.id, Fellowship.instance.members.Find(x => x != id));
            Fellowship.instance.membershipHasChanged = false;
            // Logger.LogMessage("I'm casting Ward of Rebirth.");
        }
        #endregion
    }
}