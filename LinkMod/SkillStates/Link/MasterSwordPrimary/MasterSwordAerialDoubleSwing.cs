﻿using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordAerialDoubleSwing : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Chat.AddMessage("Aerial Double swing");
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
