using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using LinkMod.Content.Link;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordAerialDownstabEndAerial : BaseSkillState
    {
        internal static float baseDuration = 0.25f;
        internal float revertShieldFraction = 0.4f;
        internal float duration;
        internal Animator animator;
        internal LinkController linkCon;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            duration = baseDuration / this.attackSpeedStat;
            base.StartAimMode(0.5f + this.duration, false);
            linkCon = base.gameObject.GetComponent<LinkController>();

            animator.SetFloat("Swing.playbackRate", this.attackSpeedStat);
            PlayAttackAnimation();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void PlayAttackAnimation()
        {
            base.PlayAnimation("FullBody, Override", $"AerialDownstabEnd", "Swing.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration * revertShieldFraction)
            {
                linkCon.SetUnsheathed();
            }
            if (base.fixedAge > duration && base.isAuthority)
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
