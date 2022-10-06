using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordSwing : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("Swing");
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
