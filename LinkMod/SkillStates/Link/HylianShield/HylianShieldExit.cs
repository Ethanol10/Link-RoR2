using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.HylianShield
{
    internal class HylianShieldExit : BaseSkillState
    {
        public static float baseDuration = 0.4f;
        public float duration;
        public Animator animator;
        internal ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            childLocator = base.GetModelChildLocator();
            this.childLocator.FindChild("ShieldHurtboxParent").gameObject.SetActive(false);
            LinkController linkcon = gameObject.GetComponent<LinkController>();
            linkcon.isShielding = false;

            base.PlayAnimation("UpperBody, Override", "ShieldBlockEnd", "Swing.playbackRate", duration);

            if (NetworkServer.active)
            {
                base.characterBody.SetBuffCount(Modules.Buffs.HylianShieldBuff.buffIndex, 0);
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
                    base.outer.SetNextStateToMain();
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
