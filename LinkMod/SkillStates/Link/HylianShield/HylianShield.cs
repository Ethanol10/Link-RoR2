using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.HylianShield
{
    internal class HylianShield : BaseSkillState
    {
        internal float baseDuration = 0.1f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("UpperBody, Override", "ShieldBlockHold");
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(baseDuration * 2.0f, false);
            //Disallow user from exiting state until user presses to get out of the state.
            if (base.isAuthority) 
            {
                if (!base.inputBank.skill2.down) 
                {
                    this.outer.SetState(new HylianShieldExit());
                }
            }
            if (base.fixedAge >= baseDuration) 
            {
                base.fixedAge = 0f;
                base.PlayCrossfade("UpperBody, Override", "ShieldBlockHold", 0.04f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
