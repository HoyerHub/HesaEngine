using System.Linq;
using HesaEngine.SDK;
using HesaEngine.SDK.Enums;
using HesaEngine.SDK.GameObjects;

public class Helper
{
    public static Ezreal Ref;

    public static bool IsBasicAttacking;

    public static bool CanUseSpell(Spell spell, string menuItem = "", bool ignoreBasicAttack = false)
    {
        return (!IsBasicAttacking || ignoreBasicAttack) && spell.IsLearned && ObjectManager.Me.CanCast && spell.IsReady() &&
               (menuItem == "" || MenuCheckBox(menuItem, spell)) && spell.ManaCost < ObjectManager.Me.Mana;
    }

    public static bool SkillShotCheck(Obj_AI_Base target, Spell spell, HitChance hitChance = HitChance.Medium)
    {
        return target != null && target.IsValidTarget(spell.Range) && spell.GetPrediction(target).Hitchance >= hitChance;
    }

    public static bool MenuCheckBox(string query, Spell spell = null)
    {
        string subMenu;
        if (spell != null)
        {
            if (spell.Slot == SpellSlot.Q) subMenu = "Q Settings";
            else if (spell.Slot == SpellSlot.W) subMenu = "W Settings";
            else if (spell.Slot == SpellSlot.E) subMenu = "E Settings";
            else subMenu = "R Settings";
            return Ref.Main.SubMenu(subMenu).Get<MenuCheckbox>(query).Checked;
        }
        subMenu = query.Split('.').First();
        var menuItem = query.Split('.').Last();
        return Ref.Main.SubMenu(subMenu).Get<MenuCheckbox>(menuItem).Checked;
    }

    public static int MenuSlider(string query, Spell spell = null)
    {
        string subMenu;
        if (spell != null)
        {
            if (spell.Slot == SpellSlot.Q) subMenu = "Q Settings";
            else if (spell.Slot == SpellSlot.W) subMenu = "W Settings";
            else if (spell.Slot == SpellSlot.E) subMenu = "E Settings";
            else subMenu = "R Settings";
            return Ref.Main.SubMenu(subMenu).Get<MenuSlider>(query).CurrentValue;
        }
        subMenu = query.Split('.').First();
        var menuItem = query.Split('.').Last();
        return Ref.Main.SubMenu(subMenu).Get<MenuSlider>(menuItem).CurrentValue;
    }

    public static bool IsNeutralObjective(string name)
    {
        var bigMobs = new[]
        {
            "SRU_Dragon_Water", "SRU_Dragon_Fire", "SRU_Dragon_Earth", "SRU_Dragon_Air",
            "SRU_Dragon_Elder", "SRU_Baron", "SRU_RiftHerald", "TT_Spiderboss"
        };
        return bigMobs.Contains(name);
    }
}