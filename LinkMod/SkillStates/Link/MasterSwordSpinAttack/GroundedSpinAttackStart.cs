using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.MasterSwordSpinAttack
{
    internal class GroundedSpinAttackStart : BaseSkillState
    {
        internal static float baseDuration = 0.05f;
        internal float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            base.GetModelAnimator().SetFloat("Swing.playbackRate", base.attackSpeedStat);
            base.PlayAnimation("FullBody, Override", "GroundedSpinAttackStart", "Swing.playbackRate", duration);

            if (NetworkServer.active) 
            {
                base.characterBody.SetBuffCount(Modules.Buffs.SpinAttackSlowDebuff.buffIndex, 1);
            }
       }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge > duration) 
            {
                base.outer.SetState(new GroundedSpinAttackCharge { });
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
