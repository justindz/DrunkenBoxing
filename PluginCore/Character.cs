using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace DrunkenBoxing {
    public enum State
    {
        Unready,
        Ready,
        ChangingCaster,
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
        public Spell lastSpellCast;
        public DateTime lastSpellCastShouldBeDoneAfter;

        private Character() {
            instance = this;
            state = State.Unready;
            spells = Spell.BuildSpellTable();            
            combatants = new Queue<Enemy>();
            lastSpellCast = null;
            lastSpellCastShouldBeDoneAfter = DateTime.MinValue;
            state = State.Ready;
        }

        public void Update(double deltaTime) {
            try {
                if (state == State.Unready) return;

                if (state == State.Casting && DateTime.UtcNow > lastSpellCastShouldBeDoneAfter)
                    state = State.Ready;
                else
                    return;
                
                if (combatants.Count == 0) return;
                if (Decal.Adapter.CoreManager.Current.Actions.BusyState != 0) return;

                if (state == State.Ready) {
                    int casterId = SelectCasterByTarget(combatants.Peek()).id;
                    
                    if (false) { // if casterId is not the one curently being wielded
                        Decal.Adapter.CoreManager.Current.Actions.AutoWield(casterId);
                        state = State.ChangingCaster;
                        Logger.LogMessage("I need to change casters for this target.");
                    }
                    else if (Decal.Adapter.CoreManager.Current.Actions.CombatMode != CombatState.Magic) {
                        state = State.SwitchingToMagicMode;
                        Decal.Adapter.CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                        Logger.LogMessage("I need to get into magic combat mode.");
                    }
                    else {
                        lastSpellCast = spells["Incantation of Bloodstone Bolt"];
                        state = State.Casting;
                        Decal.Adapter.CoreManager.Current.Actions.CastSpell(lastSpellCast.id, combatants.Peek().id);
                        Logger.LogMessage("I'm casting " + lastSpellCast.id.ToString() + ".");
                    }
                }
                else if (state == State.ChangingCaster) {
                    int casterId = SelectCasterByTarget(combatants.Peek()).id;
                    // If casterId is the one currently being wielded
                        // state = State.Ready;
                    Logger.LogMessage("I've equipped the right caster for the target and I'm ready to cast.");
                }
                else if (state == State.SwitchingToMagicMode && Decal.Adapter.CoreManager.Current.Actions.CombatMode == CombatState.Magic) {
                    state = State.Ready;
                    Logger.LogMessage("I've switched to magic combat mode.");
                }
            } catch (Exception ex) { Logger.LogError("Character.Update=" + state.ToString(), ex); }
        }
        
        public void CharacterFilter_SpellCast(object sender, SpellCastEventArgs e) {
            if (e.SpellId == lastSpellCast.id)
                lastSpellCastShouldBeDoneAfter = DateTime.UtcNow.Add(TimeSpan.FromSeconds(30.0));
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

        public void AddCombatant(Enemy enemy) {
            if (!combatants.Contains(enemy)) {
                combatants.Enqueue(enemy);
            }
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
        }
    }
}