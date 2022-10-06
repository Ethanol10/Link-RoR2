using LinkMod.SkillStates;
using LinkMod.SkillStates.BaseStates;
using LinkMod.SkillStates.Link;
using LinkMod.SkillStates.Link.MasterSwordPrimary;

namespace LinkMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {

            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(SlashCombo));
            Modules.Content.AddEntityState(typeof(Shoot));
            Modules.Content.AddEntityState(typeof(ThrowBomb));


            //Spawn State
            Modules.Content.AddEntityState(typeof(LinkSpawnState));

            //Roll
            Modules.Content.AddEntityState(typeof(Roll));

            //Master Sword
            Modules.Content.AddEntityState(typeof(MasterSword));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDoubleSwing));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDownstab));
            Modules.Content.AddEntityState(typeof(MasterSwordAerialDownstabRecovery));
            Modules.Content.AddEntityState(typeof(MasterSwordSwing));
            Modules.Content.AddEntityState(typeof(MasterSwordSwingFinalGroundedHit));
            Modules.Content.AddEntityState(typeof(MasterSwordDashAttack));


        }
    }
}