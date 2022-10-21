using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombHold : BaseSkillState
    {
        internal static float baseDuration = 0.5f;

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
            animator.SetFloat("Swing.playbackRate", 1.0f);

            base.PlayAnimation("UpperBody, Override", "ItemThrowHold", "Swing.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority) 
            {
                if (!base.IsKeyDownAuthority())
                {
                    totalDuration += duration;
                    base.outer.SetState(
                        new RuneBombThrow
                        {
                            totalDuration = this.totalDuration
                        }
                    );
                    return;
                }
                if (base.fixedAge >= duration) 
                {
                    //check if still holding skill down.
                    if (base.IsKeyDownAuthority()) 
                    {
                        totalDuration += duration;
                        base.outer.SetNextState(
                            new RuneBombHold 
                            {
                                totalDuration = this.totalDuration
                            }
                        );
                        return;
                    }
                    else 
                    {
                        totalDuration += duration;
                        base.outer.SetNextState(
                            new RuneBombThrow
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