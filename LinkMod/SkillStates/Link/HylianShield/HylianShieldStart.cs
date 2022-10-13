using EntityStates;
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
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            childLocator = base.GetModelChildLocator();
            this.childLocator.FindChild("ShieldHurtboxParent").gameObject.SetActive(true);

            base.PlayAnimation("UpperBody, Override", "ShieldBlockStart", "Swing.playbackRate", duration);

            if (NetworkServer.active) 
            {
                base.characterBody.SetBuffCount(Modules.Buffs.HylianShieldSlowDebuff.buffIndex, 1);
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
                    base.outer.SetState(new HylianShield { });
                    return;
                } 
            }
        }
    }
}
