using EntityStates;
using LinkMod.Content.Link;
using LinkMod.SkillStates.Link.GenericItemStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.StandardBomb
{
    internal class SuperBombSpawn : BaseSkillState
    {
        internal static float baseDuration = 0.6f;
        internal static float sheatheFraction = 0.1f;
        internal static float unsheatheSwordFraction = 0.68f;
        internal static float bombSpawnFraction = 0.21f;
        internal bool sheathe;
        internal bool unsheatheSword;
        internal bool bombEnabled;

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
            linkController.itemInHand = LinkController.ItemInHand.SUPER;
            sheathe = false;
            unsheatheSword = false;
            bombEnabled = false;
            linkController.isHolding = true;

            linkController.DisableFakeSuperBombInHand();
            linkController.DisableSuperBombInHand();

            //assume we're in bomb loadout.
            // Set all skills regarding hylian shield to throw.
            // Set all skills regarding spawning a bomb to throw.
        }

        public override void OnExit()
        {
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
            base.OnExit();
            linkController.itemInHand = LinkController.ItemInHand.SUPER;
            linkController.isHolding = true;
            linkController.DisableFakeSuperBombInHand();
            linkController.EnableSuperBombInHand();
            linkController.SetSwordOnlyUnsheathed();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration * sheatheFraction && !sheathe)
            {
                linkController.SetSheathed();
                sheathe = true;
            }
            if (base.fixedAge >= duration * bombSpawnFraction && !bombEnabled)
            {
                bombEnabled = true;
                linkController.EnableFakeSuperBombInHand();
            }
            if (base.fixedAge >= duration * unsheatheSwordFraction && !unsheatheSword)
            {
                linkController.SetSwordOnlyUnsheathed();
                unsheatheSword = true;
                if (!inputBank.skill4.down)
                {
                    this.outer.SetNextState(new ItemThrow { totalDuration = 0f });
                    return;
                }
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                linkController.DisableFakeSuperBombInHand();
                linkController.EnableSuperBombInHand();
                this.outer.SetNextState(new ItemStartHold { });
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}