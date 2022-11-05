using EntityStates;
using LinkMod.Content.Link;
using LinkMod.SkillStates.Link.GenericItemStates;
using LinkMod.SkillStates.Link.RuneBomb;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace LinkMod.SkillStates.Link.SuperBomb
{
    internal class SuperBombHold : BaseSkillState
    {
        internal static float baseDuration = 1.0f;

        internal float duration;
        internal Animator animator;
        internal LinkController linkController;
        internal float totalDuration;

        public override void OnEnter()
        {
            base.OnEnter();
            linkController = base.gameObject.GetComponent<LinkController>();
            animator = base.GetModelAnimator();

            //Don't scale by attackSpeed
            duration = baseDuration;
            animator.SetFloat("Swing.playbackRate", 1f);

            base.PlayAnimation("UpperBody, Override", "ItemThrowHold", "Swing.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
            linkController.isCharged = false;
            linkController.isCharging = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (totalDuration <= Modules.Config.bombTimerToMaxCharge.Value)
            {
                linkController.isCharged = false;
                linkController.isCharging = true;
            }
            if (totalDuration >= Modules.Config.bombTimerToMaxCharge.Value)
            {
                linkController.isCharging = false;
                linkController.isCharged = true;
            }
            if (base.isAuthority)
            {
                if (!inputBank.skill4.down)
                {
                    totalDuration += duration;

                    linkController.itemInHand = LinkController.ItemInHand.SUPER;
                    base.outer.SetState(
                        new ItemThrow
                        {
                            totalDuration = this.totalDuration
                        }
                    );
                    return;
                }
                if (base.fixedAge >= duration)
                {
                    //check if still holding skill down.
                    if (inputBank.skill4.down)
                    {
                        totalDuration += duration;
                        base.outer.SetState(
                            new SuperBombHold
                            {
                                totalDuration = this.totalDuration
                            }
                        );
                        return;
                    }
                    else
                    {
                        totalDuration += duration;

                        linkController.itemInHand = LinkController.ItemInHand.SUPER;
                        base.outer.SetState(
                            new ItemThrow
                            {
                                totalDuration = this.totalDuration
                            }
                        );
                        return;
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.totalDuration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.totalDuration = reader.ReadSingle();
        }
    }
}