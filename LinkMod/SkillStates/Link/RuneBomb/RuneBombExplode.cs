using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombExplode : BaseSkillState
    {
        public override void OnEnter() 
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}