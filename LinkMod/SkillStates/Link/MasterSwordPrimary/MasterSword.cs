using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSword : BaseSkillState
    {
        public static float baseDuration = 0.4f;
        public float duration;
        public Animator animator;
        internal ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();

            //Entry point for all swings.

            //Turn off shielding if it's on.
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

            //Target is Grounded
            if (base.isGrounded) 
            {
                //Check if dashing
                if (base.characterBody.isSprinting) 
                {
                    this.outer.SetState(new MasterSwordDashAttack { });
                    return;
                }

                //Do Default swing
                this.outer.SetState(new MasterSwordSwing { });
                return;
            }

            //Continue under the assumption that the character is in the air
            if (CheckLookingDown()) 
            {
                this.outer.SetState(new MasterSwordAerialDownstabBegin { });
                return;
            }

            //otherwise just default to aerial attack
            this.outer.SetState(new MasterSwordAerialDoubleSwing { });
            return;
       }

        private bool CheckLookingDown()
        {
            if (Vector3.Dot(base.GetAimRay().direction, Vector3.down) > 0.8f)
            {
                return true;
            }
            return false;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}
