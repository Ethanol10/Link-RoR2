using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordAerialDownstab : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("Aerial Downstab");
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
