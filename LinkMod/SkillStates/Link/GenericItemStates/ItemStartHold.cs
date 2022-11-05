using EntityStates;
using LinkMod.Content.Link;
using LinkMod.SkillStates.Link.RuneBomb;
using LinkMod.SkillStates.Link.StandardBomb;
using LinkMod.SkillStates.Link.SuperBomb;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.GenericItemStates
{
    internal class ItemStartHold : BaseSkillState
    {
        internal static float baseDuration = 0.2f;

        internal float duration;
        internal Animator animator;
        internal LinkController linkController;

        public override void OnEnter()
        {
            base.OnEnter();
            linkController = gameObject.GetComponent<LinkController>();
            animator = GetModelAnimator();
            duration = baseDuration / attackSpeedStat;

            animator.SetFloat("Swing.playbackRate", attackSpeedStat);
            PlayAnimation("UpperBody, Override", "ItemStartThrow", "Swing.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                switch (linkController.itemInHand)
                {
                    case LinkController.ItemInHand.RUNE:
                        if (!base.inputBank.skill1.down)
                        {
                            outer.SetState(new ItemThrow { totalDuration = 0f });
                        }
                        break;
                    case LinkController.ItemInHand.NORMAL:
                        if (!base.inputBank.skill3.down)
                        {
                            outer.SetState(new ItemThrow { totalDuration = 0f });
                        }
                        break;
                    case LinkController.ItemInHand.SUPER:
                        if (!base.inputBank.skill4.down)
                        {
                            outer.SetState(new ItemThrow { totalDuration = 0f });
                        }
                        break;
                    case LinkController.ItemInHand.BOMBCHU:
                        if (!base.inputBank.skill4.down)
                        {
                            outer.SetState(new ItemThrow { totalDuration = 0f });
                        }
                        break;
                }
            }
            if (fixedAge >= duration && isAuthority)
            {
                switch (linkController.itemInHand)
                {
                    case LinkController.ItemInHand.RUNE:
                        outer.SetState(new RuneBombHold { });
                        break;
                    case LinkController.ItemInHand.NORMAL:
                        outer.SetState(new StandardBombHold { });
                        break;
                    case LinkController.ItemInHand.SUPER:
                        outer.SetState(new SuperBombHold { });
                        break;
                    case LinkController.ItemInHand.BOMBCHU:
                        break;
                }
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}