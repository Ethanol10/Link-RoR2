using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordDashAttack : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("Dash Attack");
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}
