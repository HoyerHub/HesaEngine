using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;

public abstract class State
{
    private static Ezreal Ref => Helper.Ref;

    internal AIHeroClient Me => ObjectManager.Me;
    internal Spell Q => Ref.Q;
    internal Spell W => Ref.W;
    internal Spell E => Ref.E;
    internal Spell R => Ref.R;

    internal virtual void Update()
    {
        if (Helper.CanUseSpell(Q, "Auto"))
        {
            var target = TargetSelector.GetTarget(Q.Range);
            var pred = Q.GetPrediction(target);
            if (pred.Hitchance >= HitChance.VeryHigh)
            {
                Q.Cast(pred.CastPosition);
            }
        }
        if (Helper.CanUseSpell(Q))
        {
            var killableEnemies = ObjectManager.Heroes.Enemies.Where(e => Helper.SkillShotCheck(e, Q) && e.Health < Me.GetSpellDamage(e, SpellSlot.Q) + Me.GetAutoAttackDamage(e)).ToList();
            if (killableEnemies.Count > 0)
            {
                Q.Cast(killableEnemies.OrderByDescending(e => Q.GetPrediction(e).Hitchance).First());
            }
        }
        if (Me.ManaPercent > Helper.MenuSlider("Stack", Q) && Helper.MenuSlider("Stack", Q) > 0 && Helper.CanUseSpell(Q) && Me.Position.CountEnemiesInRange(1500) == 0 && Me.HasItem((int)ItemId.Tear_of_the_Goddess))
        {
            Q.Cast(Game.CursorPosition);
        }
        if (Helper.CanUseSpell(R, "Finisher"))
        {
            var killableEnemies = ObjectManager.Heroes.Enemies.Where(e => Helper.SkillShotCheck(e, R) && e.Health + 30 < Me.GetSpellDamage(e, SpellSlot.R)).ToList();
            if (killableEnemies.Count > 0 && !ObjectManager.Heroes.Enemies.Any(e=>e.Distance(Me) < 900))
            {
                R.Cast(killableEnemies.OrderByDescending(e => R.GetPrediction(e).Hitchance).First());
            }
        }
        if (Helper.MenuSlider("Multi", R) > 0 && Helper.CanUseSpell(R) && Me.Position.CountEnemiesInRange(900) == 0)
        {
            foreach (var enemy in ObjectManager.Heroes.Enemies.Where(e=> e.Distance(Me) > 900 && e.Distance(Me) < 3000 && Helper.SkillShotCheck(e, R)))
            {
                R.CastIfWillHit(enemy, Helper.MenuSlider("Multi", R));
            }
        }
    }
}