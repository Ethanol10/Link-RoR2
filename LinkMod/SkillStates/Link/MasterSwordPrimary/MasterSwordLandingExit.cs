using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordLandingExit : BaseSkillState
    {
        public static float baseDuration = 0.2f;
        public float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            base.GetModelAnimator().SetFloat("Landing.playbackRate", base.attackSpeedStat);
            base.PlayAnimation("FullBody, Override", "LandingAnim", "Landing.playbackRate", duration);
            base.outer.SetNextStateToMain();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
