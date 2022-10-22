using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.MasterSwordSpinAttack
{
    internal class GroundedSpinAttackCharge : BaseSkillState
    {
        internal static float baseDuration = 1.5f;
        internal float duration;
        internal float totalDuration;
        internal LinkController linkController;

        public override void OnEnter()
        {
            base.OnEnter();
            base.GetModelAnimator().SetFloat("Swing.playbackRate", 1.0f);
            base.PlayAnimation("FullBody, Override", "GroundedSpinAttackHold", "Swing.playbackRate", baseDuration);

            linkController = gameObject.GetComponent<LinkController>();
            if (NetworkServer.active)
            {
                base.characterBody.SetBuffCount(Modules.Buffs.SpinAttackSlowDebuff.buffIndex, 1);
            }
        }   

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (totalDuration <= Modules.Config.multiplierSpinAttack.Value) 
            {
                linkController.isCharged = false;
                linkController.isCharging = true;
            }
            if (totalDuration >= Modules.Config.multiplierSpinAttack.Value)
            {
                linkController.isCharged = true;
                linkController.isCharging = false;
            }
            if (base.isAuthority) 
            {
                
                if (base.fixedAge > duration) 
                {
                    if (!base.inputBank.skill4.down)
                    {
                        totalDuration += base.fixedAge;
                        base.outer.SetState(new GroundedSpinAttackEnd { totalDurationHeld = totalDuration });
                    }
                    else 
                    {
                        totalDuration += base.fixedAge;
                        base.outer.SetState(new GroundedSpinAttackCharge { totalDuration = totalDuration });
                    }
                }

                //If let go
                if (!base.inputBank.skill4.down)
                {
                    totalDuration += base.fixedAge;
                    base.outer.SetState(new GroundedSpinAttackEnd { totalDurationHeld = totalDuration });
                }
            }
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
