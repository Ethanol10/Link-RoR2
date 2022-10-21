using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombThrow : BaseSkillState
    {
        internal static float baseDuration = 0.96f;

        internal Animator animator;
        internal LinkController linkController;
        internal float throwBombFraction = 0.2f;
        internal float shieldTakeOutFraction = 0.73f;
        internal bool bombThrown;
        internal bool shieldTaken;

        internal float duration;
        internal float totalDuration;
        internal bool wasGrounded;
        

        public override void OnEnter() 
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            linkController = base.gameObject.GetComponent<LinkController>();
            linkController.bombState = LinkController.BombState.THROWN;

            bombThrown = false;
            shieldTaken = false;

            if (isGrounded)
            {
                base.PlayCrossfade("UpperBody, Override", "GroundedItemThrow", "Swing.playbackRate", duration, 0.02f);
                wasGrounded = true;
            }
            else 
            {
                base.PlayAnimation("FullBody, Override", "AirItemThrow", "Swing.playbackRate", duration);
                wasGrounded = false;
            }
            
        }

        public override void OnExit()
        {
            base.OnExit();
            if (wasGrounded)
            {
                base.PlayAnimation("UpperBody, Override", "BufferEmpty");
            }
            else 
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration * throwBombFraction && !bombThrown) 
            {
                linkController.DisableRuneBombInHand();
                bombThrown = true;
                //Fire using network request
            }
            if(base.fixedAge >= duration * shieldTakeOutFraction && !shieldTaken) 
            {
                linkController.SetUnsheathed();
                shieldTaken = true;
            }
            if (base.fixedAge >= duration) 
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}