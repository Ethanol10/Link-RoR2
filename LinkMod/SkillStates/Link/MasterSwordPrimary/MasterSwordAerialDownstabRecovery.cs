using EntityStates;
using LinkMod.Content.Link;
using UnityEngine;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordAerialDownstabRecovery : BaseSkillState
    {
        internal static float baseDuration = 1.1f;
        internal float retrieveShieldFraction = 0.77f;
        internal float duration;
        internal Animator anim;
        internal LinkController linkcon;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            anim = base.GetModelAnimator();
            linkcon = this.gameObject.GetComponent<LinkController>();

            anim.SetFloat("Swing.playbackRate", base.attackSpeedStat);

            base.PlayAnimation("FullBody, Override", "AerialDownstabLanding", "Swing.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration * retrieveShieldFraction) 
            {
                linkcon.SetUnsheathed();
            }

            if (base.fixedAge > duration) 
            {
                this.outer.SetNextStateToMain();
            }
        }
    }
}
