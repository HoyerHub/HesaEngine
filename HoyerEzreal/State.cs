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

            var killableEnemies = ObjectManager.Heroes.Enemies.Where(e => e.IsValidTarget(Q.Range) && e.Health < Me.GetSpellDamage(e, SpellSlot.Q) + Me.GetAutoAttackDamage(e, true) * 2).ToList();
            if (killableEnemies.Count > 0)
            {
                R.Cast(killableEnemies.OrderByDescending(e => R.GetPrediction(e).Hitchance).First());
            }
        }
        if (Helper.CanUseSpell(R, "Finisher"))
        {
            var killableEnemies = ObjectManager.Heroes.Enemies.Where(e => e.IsValidTarget(3000) && e.Health + 30 < Me.GetSpellDamage(e, SpellSlot.R)).ToList();
            if (killableEnemies.Count > 0 && !ObjectManager.Heroes.Enemies.Any(e=>e.Distance(Me) < 900))
            {
                R.Cast(killableEnemies.OrderByDescending(e => R.GetPrediction(e).Hitchance).First());
            }
        }
    }
}