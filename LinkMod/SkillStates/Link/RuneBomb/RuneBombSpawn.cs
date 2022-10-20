using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombSpawn : BaseSkillState
    {
        internal static float baseDuration = 1.5f;
        internal static float sheatheFraction = 0.1f;
        internal static float unsheatheSwordFraction = 0.68f;
        internal bool sheathe;
        internal bool unsheatheSword;

        internal float duration;
        internal Animator animator;
        internal LinkController linkController;

        public override void OnEnter() 
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            linkController = base.gameObject.GetComponent<LinkController>();

            base.PlayAnimation("UpperBody, Override", "DeployBomb", "Swing.playbackRate", duration);
            linkController.runeBombState = LinkController.RuneBombState.INHAND;
            sheathe = false;
            unsheatheSword = false;

            // Set all skills regarding hylian shield to throw.
            // Set all skills regarding spawning a bomb to throw.
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration * sheatheFraction && !sheathe) 
            {
                linkController.SetSheathed();
            }
            if (base.fixedAge >= duration * unsheatheSwordFraction && !unsheatheSword) 
            {
                linkController.SetSwordOnlyUnsheathed();
            }
            if (base.fixedAge >= duration) 
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}