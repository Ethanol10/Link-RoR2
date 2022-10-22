using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombStartHold : BaseSkillState
    {
        internal static float baseDuration = 0.2f;

        internal float duration;
        internal Animator animator;
        internal LinkController linkController;

        public override void OnEnter() 
        {
            base.OnEnter();
            linkController = base.gameObject.GetComponent<LinkController>();
            animator = base.GetModelAnimator();
            duration = baseDuration / base.attackSpeedStat;

            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            base.PlayAnimation("UpperBody, Override", "ItemStartThrow", "Swing.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration && base.isAuthority) 
            {
                base.outer.SetNextState(new RuneBombHold { });
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}