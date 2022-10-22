using LinkMod.SkillStates;
using LinkMod.SkillStates.Link;
using LinkMod.SkillStates.Link.Boomerang;
using LinkMod.SkillStates.Link.BowAndArrow;
using LinkMod.SkillStates.Link.DekuNut;
using LinkMod.SkillStates.Link.GoddessSpells;
using LinkMod.SkillStates.Link.Hookshot;
using LinkMod.SkillStates.Link.HylianShield;
using LinkMod.SkillStates.Link.MasterSwordPrimary;
using LinkMod.SkillStates.Link.MasterSwordSpinAttack;
using LinkMod.SkillStates.Link.RuneBomb;
using LinkMod.SkillStates.Link.StandardBomb;
using LinkMod.SkillStates.Link.SwapLoadout;

namespace LinkMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            //Spawn State
            Modules.Content.AddEntityState(typeof(LinkSpawnState));

            //Loadout Swap States
            Modules.Content.AddEntityState(typeof(SwapSwordLoadout));
            Modules.Content.AddEntityState(typeof(SwapArrowLoadout));
            Modules.Content.AddEntityState(typeof(SwapBombLoadout));
            Modules.Content.AddEntityState(typeof(SwapMiscellaneousLoadout));

            //Parasail
            Modules.Content.AddEntityState(typeof(ParasailOn));

            //Roll
            Modules.Content.AddEntityState(typeof(Roll));

            //Master Sword
            Modules.Content.AddEntityState(typeof(MasterSword));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDoubleSwing));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDownstabBegin));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDownstab));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDownstabEndAerial));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDownstabRecovery));
            Modules.Content.AddEntityState(typeof(MasterSwordSwing));
            Modules.Content.AddEntityState(typeof(MasterSwordSwingFinalGroundedHit));
            Modules.Content.AddEntityState(typeof(MasterSwordDashAttack));
            Modules.Content.AddEntityState(typeof(MasterSwordLandingExit));

            //Spin Attack
            Modules.Content.AddEntityState(typeof(SpinAttack));
            Modules.Content.AddEntityState(typeof(GroundedSpinAttackStart));
            Modules.Content.AddEntityState(typeof(GroundedSpinAttackCharge));
            Modules.Content.AddEntityState(typeof(GroundedSpinAttackEnd));
            Modules.Content.AddEntityState(typeof(AerialSpinAttack));

            //Hylian Shield
            Modules.Content.AddEntityState(typeof(HylianShieldBlockSuccessful));
            Modules.Content.AddEntityState(typeof(HylianShield));
            Modules.Content.AddEntityState(typeof(HylianShieldStart));
            Modules.Content.AddEntityState(typeof(HylianShieldExit));

            //Rune Bomb
            Modules.Content.AddEntityState(typeof(RuneBombDetonate));
            Modules.Content.AddEntityState(typeof(RuneBombSpawn));
            Modules.Content.AddEntityState(typeof(RuneBombStartHold));
            Modules.Content.AddEntityState(typeof(RuneBombHold));
            Modules.Content.AddEntityState(typeof(RuneBombThrow));
            Modules.Content.AddEntityState(typeof(RuneBombDeathState));

            //Standard Bomb
            Modules.Content.AddEntityState(typeof(StandardBombSpawn));
            Modules.Content.AddEntityState(typeof(StandardBombThrow));
            Modules.Content.AddEntityState(typeof(StandardBombStartHold));
            Modules.Content.AddEntityState(typeof(StandardBombHold));

            //Bow And Arrow
            Modules.Content.AddEntityState(typeof(DrawBow));
            Modules.Content.AddEntityState(typeof(FireBow));
            Modules.Content.AddEntityState(typeof(HoldBow));
            Modules.Content.AddEntityState(typeof(SwapArrowFireType));
            Modules.Content.AddEntityState(typeof(SwapArrowType));
            Modules.Content.AddEntityState(typeof(ComboShotFire));
            Modules.Content.AddEntityState(typeof(ComboShotStart));

            //Goddess Spells
            Modules.Content.AddEntityState(typeof(DinsFire));
            Modules.Content.AddEntityState(typeof(FaroresWind));
            Modules.Content.AddEntityState(typeof(NayrusLove));
            Modules.Content.AddEntityState(typeof(GoddessSpellEntry));

            //Deku Nut
            Modules.Content.AddEntityState(typeof(DekuNut));

            //Hookshot
            Modules.Content.AddEntityState(typeof(Hookshot));

            //Boomerang
            Modules.Content.AddEntityState(typeof(BoomerangStart));
            Modules.Content.AddEntityState(typeof(BoomerangHold));
            Modules.Content.AddEntityState(typeof(BoomerangThrow));
        }
    }
}