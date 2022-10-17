using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.HylianShield
{
    internal class HylianShieldStart : BaseSkillState
    {
        public static float baseDuration = 0.4f;
        public float duration;
        public Animator animator;
        public ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            base.StartAimMode(baseDuration * 1.5f, false);
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            childLocator = base.GetModelChildLocator();
            this.childLocator.FindChild("ShieldHurtboxParent").gameObject.SetActive(true);
            LinkController linkcon = gameObject.GetComponent<LinkController>();
            linkcon.isShielding = true;

            base.PlayAnimation("UpperBody, Override", "ShieldBlockStart", "Swing.playbackRate", duration);

            if (NetworkServer.active) 
            {
                base.characterBody.SetBuffCount(Modules.Buffs.HylianShieldBuff.buffIndex, 1);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority) 
            {
                if (base.fixedAge > duration)
                {
                    base.outer.SetNextState(new HylianShield { });
                    return;
                } 
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
