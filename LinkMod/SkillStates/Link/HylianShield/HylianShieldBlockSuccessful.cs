using EntityStates;

namespace LinkMod.SkillStates.Link.HylianShield
{
    internal class HylianShieldBlockSuccessful : BaseSkillState
    {
        internal float baseDuration = 0.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("UpperBody, Override", "ShieldBlockSuccess");
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority) 
            {
                if (base.fixedAge > baseDuration) 
                {
                    base.outer.SetNextStateToMain();
                    return;
                }
            }
            //Should be played on the 
        }
    }
}
