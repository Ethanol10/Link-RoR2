using LinkMod.SkillStates;
using LinkMod.SkillStates.Link;
using LinkMod.SkillStates.Link.HylianShield;
using LinkMod.SkillStates.Link.MasterSwordPrimary;
using LinkMod.SkillStates.Link.MasterSwordSpinAttack;

namespace LinkMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            //Spawn State
            Modules.Content.AddEntityState(typeof(LinkSpawnState));

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
        }
    }
}