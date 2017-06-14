using System;
using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;

public class Ezreal : IScript
{
    public string Name { get; } = "Hoyer's Ezreal";
    public string Version { get; } = "1.0.0";
    public string Author { get; } = "Hoyer";

    public Spell Q, W, E, R;

    public Menu Main;
    private Menu _qMenu, _wMenu, _eMenu, _rMenu;

    private State _currentState = new Sleep();
    private AIHeroClient Me => ObjectManager.Me;

    public void OnInitialize()
    {
        Core.DelayAction(OnLoad, new Random().Next(800, 1200));
    }

    private void OnLoad()
    {
        if (ObjectManager.Me.Hero != Champion.Ezreal) return;

        InitSpells();
        InitMenu();
        Helper.Ref = this;

        Game.OnTick += Update;
        Orbwalker.BeforeAttack += BeforeAttack;
        Orbwalker.AfterAttack += AfterAttack;
        AntiGapcloser.OnEnemyGapcloser += OnEnemyGapcloser;
    }

    private void Update()
    {
        SetState();
        _currentState.Update();
    }

    private void InitSpells()
    {
        Q = new Spell(SpellSlot.Q, 1150);
        Q.SetSkillshot(delay: 250, width: 60, speed: 2000, collision: true, type: SkillshotType.SkillshotLine);

        W = new Spell(SpellSlot.Q, 1000);
        W.SetSkillshot(delay: 250, width: 80, speed: 1600, collision: false, type: SkillshotType.SkillshotLine);

        E = new Spell(SpellSlot.E, 475);

        R = new Spell(SpellSlot.R, 3000);
        R.SetSkillshot(delay: 1000, width: 160, speed: 2000, collision: false, type: SkillshotType.SkillshotLine);
    }

    private void InitMenu()
    {
        Main = Menu.AddMenu("Hoyer's Ezreal");

        _qMenu = Main.AddSubMenu("Q Settings");
        _qMenu.Add(new MenuCheckbox("Combo", "Use Q in Combo", true));
        _qMenu.Add(new MenuCheckbox("Harass", "Use Q in Harass", true));
        _qMenu.Add(new MenuCheckbox("Waveclear", "Use Q to Waveclear", true));
        _qMenu.Add(new MenuSlider("WaveclearMana", "Min. Mana to Waveclear", 0, 100, 50));
        _qMenu.Add(new MenuCheckbox("Lasthit", "Use Q to Lasthit out of attack range", true));
        _qMenu.Add(new MenuCheckbox("Auto", "Always Q if very high hitchance", true));
        _qMenu.Add(new MenuSlider("Stack", "Mana to Stack Tear (0 = Off)", 0, 100, 90));

        _wMenu = Main.AddSubMenu("W Settings");
        _wMenu.Add(new MenuCheckbox("Combo", "Use W in Combo", true));
        _wMenu.Add(new MenuCheckbox("Harass", "Use W in Harass", true));
        _wMenu.Add(new MenuCheckbox("Jew", "Save Mana in early game", true));
        _wMenu.Add(new MenuCheckbox("Buff", "Buff Allies taking objectives", true));

        _eMenu = Main.AddSubMenu("E Settings");
        _eMenu.Add(new MenuCheckbox("Gapcloser", "Use E to avoid gapclosers", true));
        _eMenu.Add(new MenuCheckbox("Finisher", "Use E to finish enemies (aggressive)", true));

        _rMenu = Main.AddSubMenu("R Settings");
        _rMenu.Add(new MenuCheckbox("Finisher", "Use R to finish enemies (safe)", true));
        _rMenu.Add(new MenuSlider("Multi", "Use R to hit multiple enemies (0 = Off)", 0, 5, 3));
    }

    private void SetState()
    {
        if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Combo)
        {
            if (_currentState is Combo) return;
            _currentState = new Combo();
        }
        else if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.Harass)
        {
            if (_currentState is Harass) return;
            _currentState = new Harass();
        }
        else if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.LaneClear)
        {
            if (_currentState is Waveclear) return;
            _currentState = new Waveclear();
        }
        else if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.JungleClear)
        {
            if (_currentState is Jungleclear) return;
            _currentState = new Jungleclear();
        }
        else if (Core.Orbwalker.ActiveMode == Orbwalker.OrbwalkingMode.LastHit)
        {
            if (_currentState is Lasthit) return;
            _currentState = new Lasthit();
        }
        else _currentState = new Sleep();
    }

    private void BeforeAttack(HesaEngine.SDK.Args.BeforeAttackEventArgs args)
    {
        if (!args.Unit.IsMe) return;
        Helper.IsBasicAttacking = true;
    }

    private void AfterAttack(AttackableUnit unit, AttackableUnit target)
    {
        if (!unit.IsMe) return;

        Helper.IsBasicAttacking = false;

        if (!Helper.MenuCheckBox("Buff", W)) return;

        if (target.ObjectType == GameObjectType.obj_AI_Turret || target.ObjectType == GameObjectType.obj_HQ || 
            (target is Obj_AI_Minion && Helper.IsNeutralObjective(((Obj_AI_Minion) target).CharData.BaseSkinName)))
        {
            var nearbyAllies = ObjectManager.Heroes.Allies.Where(a => a.Distance(Me) < 800 && !a.IsMe).ToList();
            if (nearbyAllies.Count != 0) Q.Cast(Q.GetPrediction(nearbyAllies.OrderByDescending(a=> a.TotalAttackDamage).First()).CastPosition);
        }
    }

    private void OnEnemyGapcloser(ActiveGapcloser gapcloser)
    {
        if (Helper.CanUseSpell(E, "Gapcloser", true) && gapcloser.Sender.IsEnemy)
        {
            if (gapcloser.Target.IsMe || gapcloser.Sender.Distance(Me) <= 300 || gapcloser.End.Distance(Me) <= 250)
                E.Cast(Me.Position.Extend(gapcloser.Sender.Position, -E.Range), true);
        }
    }
}