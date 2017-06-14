using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;

public class Sleep : State
{
    //Nothing Happens
}

public class Combo : State
{
    internal override void Update()
    {
        if (Helper.CanUseSpell(Q, "Combo"))
        {
            var target = Q.GetTarget();
            if (Helper.SkillShotCheck(target, Q))
            {
                Q.Cast(Q.GetPrediction(target).CastPosition);
            }
        }
        if (Helper.CanUseSpell(W, "Combo"))
        {
            var target = W.GetTarget();
            if (!Helper.MenuCheckBox("Jew", W) || Me.Level > 10 || target.HealthPercent < 30)
            {
                if (Helper.SkillShotCheck(target, W))
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }
            }
        }
        if (Helper.CanUseSpell(E, "Finisher") && Me.HealthPercent > 40)
        {
            var target = TargetSelector.GetTarget(1250, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget() || target.Distance(Me) < Me.GetRealAutoAttackRange()) return;

            var dashPos = Me.Position.Extend(Game.CursorPosition, E.Range);
            if (target.Distance(dashPos) > Me.GetRealAutoAttackRange() || dashPos.CountEnemiesInRange(850) > 2) return;

            var comboDamage = Me.GetAutoAttackDamage(target) * 2 + E.GetDamage(target);
            if (Helper.CanUseSpell(Q, "Combo", true)) comboDamage += Q.GetDamage(target);
            if (Helper.CanUseSpell(W, "Combo", true)) comboDamage += W.GetDamage(target);
            if (target.Health > comboDamage) return;

            E.Cast(dashPos);
        }
        base.Update();
    }
}

public class Harass : State
{
    internal override void Update()
    {
        if (Helper.CanUseSpell(Q, "Harass"))
        {
            var target = Q.GetTarget();
            if (Helper.SkillShotCheck(target, Q))
            {
                Q.Cast(Q.GetPrediction(target).CastPosition);
            }
        }
        if (Helper.CanUseSpell(W, "Harass"))
        {
            var target = W.GetTarget();
            if (!Helper.MenuCheckBox("Jew", W) || Me.Level > 10 || target.HealthPercent < 30)
            {
                if (Helper.SkillShotCheck(target, W))
                {
                    W.Cast(W.GetPrediction(target).CastPosition);
                }
            }
        }
        base.Update();
    }
}

public class Waveclear : State
{
    internal override void Update()
    {
        if (Helper.CanUseSpell(Q, "Waveclear") && Me.ManaPercent > Helper.MenuSlider("WaveclearMana", Q))
        {
            var targets = MinionManager.GetMinions(Q.Range).Where(m => m.Health < Me.GetSpellDamage(m, SpellSlot.Q));
            targets = targets.Where(m => Helper.SkillShotCheck(m, Q)).OrderBy(m=>m.Health);
            Q.Cast(targets.First());
        }
        if (Helper.CanUseSpell(Q, "Harass"))
        {
            var target = Q.GetTarget();
            if (Helper.SkillShotCheck(target, Q))
            {
                Q.Cast(Q.GetPrediction(target).CastPosition);
            }
        }
        base.Update();
    }
}

public class Jungleclear : State
{
    internal override void Update()
    {
        if (Helper.CanUseSpell(Q, "Waveclear") && Me.ManaPercent > Helper.MenuSlider("WaveclearMana", Q))
        {
            var targets = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral);
            targets = targets.Where(m => Helper.SkillShotCheck(m, Q)).OrderByDescending(m => m.MinionLevel).ToList();
            Q.Cast(targets.First());
        }
        base.Update();
    }
}

public class Lasthit : State
{
    internal override void Update()
    {
        if (Helper.CanUseSpell(Q, "Lasthit"))
        {
            var targets = MinionManager.GetMinions(Q.Range).Where(m => m.Distance(Me) > Me.GetRealAutoAttackRange() && m.Health < Me.GetSpellDamage(m, SpellSlot.Q));
            targets = targets.Where(m => Helper.SkillShotCheck(m, Q)).OrderBy(m => m.Health);
            Q.Cast(targets.First());
        }
        base.Update();
    }
}