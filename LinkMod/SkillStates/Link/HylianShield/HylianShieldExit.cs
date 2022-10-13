using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.HylianShield
{
    internal class HylianShieldExit : BaseSkillState
    {
        public static float baseDuration = 0.4f;
        public float duration;
        public Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            animator = base.GetModelAnimator();

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
