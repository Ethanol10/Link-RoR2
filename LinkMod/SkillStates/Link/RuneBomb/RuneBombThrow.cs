using EntityStates;
using LinkMod.Content.Link;
using LinkMod.Modules.Networking.Miscellaneous;
using R2API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombThrow : BaseSkillState
    {
        internal static float baseDuration = 0.96f;

        internal Animator animator;
        internal LinkController linkController;
        internal float throwBombFraction = 0.2f;
        internal float shieldTakeOutFraction = 0.73f;
        internal bool bombThrown;
        internal bool shieldTaken;

        internal float duration;
        internal float totalDuration;
        internal bool wasGrounded;

        internal Transform throwPosition;

        internal float force;

        public override void OnEnter() 
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            linkController = base.gameObject.GetComponent<LinkController>();
            linkController.bombState = LinkController.BombState.THROWN;
            throwPosition = linkController.bombThrowPosition;

            bombThrown = false;
            shieldTaken = false;

            if (isGrounded)
            {
                base.PlayCrossfade("UpperBody, Override", "GroundedItemThrow", "Swing.playbackRate", duration, 0.02f);
                wasGrounded = true;
            }
            else 
            {
                base.PlayAnimation("FullBody, Override", "AirItemThrow", "Swing.playbackRate", duration);
                wasGrounded = false;
            }

            force = Mathf.Max(0.5f, Mathf.Clamp(totalDuration / Modules.Config.bombTimerToMaxCharge.Value, 0f, 1.0f));

            characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombHold, RoR2.GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.primary.SetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombDetonate, RoR2.GenericSkill.SkillOverridePriority.Contextual);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (wasGrounded)
            {
                base.PlayAnimation("UpperBody, Override", "BufferEmpty");
            }
            else 
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration * throwBombFraction && !bombThrown) 
            {
                linkController.DisableRuneBombInHand();
                bombThrown = true;
                //Fire using network request
                if (base.isAuthority) 
                {
                    new RuneBombSpawnNetworkRequest(characterBody.masterObjectId, GetAimRay().origin, GetAimRay().direction, force * Modules.Config.bombMaxThrowPower.Value).Send(R2API.Networking.NetworkDestination.Clients);
                }
            }
            if(base.fixedAge >= duration * shieldTakeOutFraction && !shieldTaken) 
            {
                linkController.SetUnsheathed();
                shieldTaken = true;
            }
            if (base.fixedAge >= duration) 
            {
                base.outer.SetNextStateToMain();
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