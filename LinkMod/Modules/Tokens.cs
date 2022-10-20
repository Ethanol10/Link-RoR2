using R2API;
using System;

namespace LinkMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region Link
            string prefix = LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_";

            string desc = "Link is a multi-purpose survivor that can adapt to situations as they unfold.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use the Master Sword to quickly dispatch foes at close range. Keep your health high and shoot beams!" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use your Hylian Shield to block damage from the front." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Roll to quickly manouver around enemies, a perfectly timed dodge will render you invulnerable!" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Spin attack can be charged on the ground to deal massive damage!" + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, with the courage to continue forwards.";
            string outroFailure = "..and so he vanished, with no courage left to burn.";

            LanguageAPI.Add(prefix + "NAME", "Link");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Hero of Hyrule");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Triforce of Courage");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Ammo and Energy have a chance to drop from enemies on death.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_MASTER_SWORD_NAME", "Master Sword");
            LanguageAPI.Add(prefix + "PRIMARY_MASTER_SWORD_DESCRIPTION", $"{Helpers.LinkSpecificDescription("Wield the Master Sword.")} Different moves will be performed depending on the circumstance.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_HYLIAN_SHIELD_NAME", "Hylian Shield");
            LanguageAPI.Add(prefix + "SECONDARY_HYLIAN_SHIELD_DESCRIPTION", $"{Helpers.LinkSpecificDescription("Steady.")} Raise your Hylian Shield to block damage from the front.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_ROLL_NAME", "Roll");
            LanguageAPI.Add(prefix + "UTILITY_ROLL_DESCRIPTION", "Roll a short distance <style=cIsUtility>You are invincible for a short time at the start of the roll.</style>");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SPIN_ATTACK_NAME", "Spin Attack");
            LanguageAPI.Add(prefix + "SPECIAL_SPIN_ATTACK_DESCRIPTION", $"{Helpers.LinkSpecificDescription("Focused.")} " +
                $"When grounded, charge your sword, multiplying your damage up to {Helpers.DamageDescription($"{StaticValues.spinAttackMaxMultiplier}x")}, and release to do at base {StaticValues.spinAttackBaseMinorHit}x {Helpers.DamageDescription($"{StaticValues.spinAttackMinorBlastDamageCoefficient * 100}%")}, " +
                $"with a final hit dealing {Helpers.DamageDescription($"{StaticValues.spinAttackMajorBlastDamageCoefficientBase * 100f}%")}. " + Environment.NewLine +
                $"When airborne, rapidly spin upwards dealing damage.");
            #endregion

            #region Sword Loadout
            LanguageAPI.Add(prefix + "SWORD_LOADOUT_NAME", "Sword and Shield Loadout");
            LanguageAPI.Add(prefix + "SWORD_LOADOUT_DESCRIPTION", "Swap to your Master Sword and Shield Loadout.");
            #endregion
            #region Arrow Loadout
            LanguageAPI.Add(prefix + "ARROW_LOADOUT_NAME", "Bow and Arrow Loadout");
            LanguageAPI.Add(prefix + "ARROW_LOADOUT_DESCRIPTION", "Swap to your Bow and Arrow Loadout.");
            #endregion
            #region Bomb Loadout
            LanguageAPI.Add(prefix + "BOMB_LOADOUT_NAME", "Bomb Loadout");
            LanguageAPI.Add(prefix + "BOMB_LOADOUT_DESCRIPTION", "Swap to your Bomb Loadout.");
            #endregion
            #region Miscellaneous Loadout
            LanguageAPI.Add(prefix + "MISCELLANEOUS_LOADOUT_NAME", "Special Loadout");
            LanguageAPI.Add(prefix + "MISCELLANEOUS_LOADOUT_DESCRIPTION", "Swap to the Extra Items Loadout.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Link: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Link, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Link: Mastery");
            #endregion

            #region Keyword Tokens
            //Master Sword
            LanguageAPI.Add(prefix + "PRIMARY_MASTER_SWORD_BEAM_KEYWORD", $"[ Sword Beam ]" + Environment.NewLine +
                $"When your health is above {Helpers.DamageDescription($"{StaticValues.healthRequiredToFirePercentage * 100f}%")}, {Helpers.LinkSpecificDescription("[ Grounded Swing ]")} and {Helpers.LinkSpecificDescription("[ Aerial Double Swing ]")} will fire a beam.");
            LanguageAPI.Add(prefix + "PRIMARY_MASTER_SWORD_GROUNDED_SWING_KEYWORD", $"[ Grounded Swing ]" + Environment.NewLine +
                $"On the ground while not sprinting, Swing your Master Sword twice for {Helpers.DamageDescription($"{StaticValues.msGroundedBasicSwing * 100f}% damage")}, " +
                $"and once for {Helpers.DamageDescription($"{StaticValues.msGroundedFinalSwing}% damage")}.");
            LanguageAPI.Add(prefix + "PRIMARY_MASTER_SWORD_GROUNDED_DASH_KEYWORD", $"[ Dash Attack ]" + Environment.NewLine +
                $"While sprinting, leap forwards and slam your sword down, dealing {Helpers.DamageDescription($"{StaticValues.msGroundedDashAttack * 100f}% damage")}.");
            LanguageAPI.Add(prefix + "PRIMARY_MASTER_SWORD_AERIAL_SWING_KEYWORD", $"[ Aerial Double Swing ]" + Environment.NewLine +
                $"While airborne, swing twice dealing {Helpers.DamageDescription($"{StaticValues.msAerialDoubleSwingFirst * 100f}% and {StaticValues.msAerialDoubleSwingSecond * 100f}% damage respectively")}.");
            LanguageAPI.Add(prefix + "PRIMARY_MASTER_SWORD_AERIAL_DOWNSTAB_KEYWORD", $"[ Downwards Stab ]" + Environment.NewLine +
                $"While airbone and looking down, stab downwards dealing {Helpers.DamageDescription($"{StaticValues.msAerialDownstab * 100}% damage")}.");

            //Hylian Shield
            LanguageAPI.Add(prefix + "STEADY_KEYWORD", $"[ Steady ]" + Environment.NewLine +
                $"Move speed is reduced by {Helpers.DownsideDescription($"{StaticValues.hylianShieldReducedMoveSpeed * 100f}%.")} " + Environment.NewLine +
                $"Armor is increased by {Helpers.LinkSpecificDescription($"{StaticValues.hylianShieldArmor}.")}" + Environment.NewLine +
                $"Jump power reduced by {Helpers.DownsideDescription($"{StaticValues.jumpPowerReduced * 100f}%.")}" + Environment.NewLine +
                $"Max Jump Count reduced to {Helpers.DownsideDescription($"{StaticValues.maxJumpCount}.")}");

            LanguageAPI.Add(prefix + "FOCUSED_KEYWORD", $"[ Focused ]" + Environment.NewLine +
                $"Move speed is reduced by {Helpers.DownsideDescription($"{StaticValues.spinAttackMoveSpeedReduction * 100f}%.")} " + Environment.NewLine +
                $"Armor is increased by {Helpers.LinkSpecificDescription($"{StaticValues.spinAttackArmourIncrease}.")}");

            #endregion

            #endregion
        }
    }
}