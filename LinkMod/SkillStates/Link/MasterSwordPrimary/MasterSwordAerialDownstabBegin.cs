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
    internal class MasterSwordAerialDownstabBegin : BaseSkillState
    {
        internal static float baseDuration = 0.25f;
        internal float shieldAwayFraction = 0.15f;
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
            base.PlayAnimation("FullBody, Override", $"AerialDownstabBegin", "Swing.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration * shieldAwayFraction) 
            {
                linkCon.SetSwordOnlyUnsheathed();
            }
            if (base.fixedAge > duration && base.isAuthority) 
            {
                base.outer.SetState(new MasterSwordAerialDownstab { });
            }
            if (base.isAuthority && base.isGrounded) 
            {

            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
