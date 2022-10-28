using EntityStates;
using RoR2.Skills;
using LinkMod.Content.Link;
using LinkMod.Modules.Networking.Miscellaneous;
using R2API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using RoR2;
using RoR2.Projectile;

namespace LinkMod.SkillStates.Link.GenericItemStates
{
    internal class ItemThrow : BaseSkillState
    {
        internal static float baseDuration = 0.96f;

        internal Animator animator;
        internal LinkController linkController;
        internal float throwItemFraction = 0.2f;
        internal float shieldTakeOutFraction = 0.73f;
        internal bool itemThrown;
        internal bool shieldTaken;

        internal float duration;
        internal float totalDuration;
        internal bool wasGrounded;

        internal Transform throwPosition;

        internal float force;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            animator = GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", attackSpeedStat);
            linkController = gameObject.GetComponent<LinkController>();

            itemThrown = false;
            shieldTaken = false;
            linkController.isHolding = false;

            if (isGrounded)
            {
                PlayCrossfade("UpperBody, Override", "GroundedItemThrow", "Swing.playbackRate", duration, 0.02f);
                wasGrounded = true;
            }
            else
            {
                PlayAnimation("FullBody, Override", "AirItemThrow", "Swing.playbackRate", duration);
                wasGrounded = false;
            }

            force = Mathf.Max(0.5f, Mathf.Clamp(totalDuration / Modules.Config.bombTimerToMaxCharge.Value, 0f, 1.0f));

            switch (linkController.itemInHand)
            {
                case LinkController.ItemInHand.RUNE:
                    linkController.runeBombThrown = true;
                    characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.skillLocator.primary, characterBody.skillLocator.primary.skillDef, RoR2.GenericSkill.SkillOverridePriority.Contextual);
                    characterBody.skillLocator.primary.SetSkillOverride(characterBody.skillLocator.primary, Content.Link.Link.runeBombDetonate, RoR2.GenericSkill.SkillOverridePriority.Contextual);
                    break;
                case LinkController.ItemInHand.NORMAL:
                    break;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            linkController.SetUnsheathed();
            linkController.isHolding = false;
            switch (linkController.itemInHand)
            {
                case LinkController.ItemInHand.RUNE:
                    linkController.runeBombThrown = true;
                    break;
                case LinkController.ItemInHand.NORMAL:
                    break;
            }
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
            if (fixedAge >= duration * throwItemFraction && !itemThrown)
            {
                switch (linkController.itemInHand) 
                {
                    case LinkController.ItemInHand.RUNE:
                        RuneBombSpecificFunction();
                        break;
                    case LinkController.ItemInHand.NORMAL:
                        NormalBombSpecificFunction();
                        break;
                    default:
                        break;
                }
            }
            if (fixedAge >= duration * shieldTakeOutFraction && !shieldTaken)
            {
                linkController.SetUnsheathed();
                shieldTaken = true;
            }
            if (fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public void NormalBombSpecificFunction() 
        {
            itemThrown = true;
            linkController.DisableStandardBombInHand();

            if (base.isAuthority) 
            {
                //Throw the projectile.
                ProjectileManager.instance.FireProjectile(Modules.Projectiles.standardBombPrefab,
                    GetAimRay().origin,
                    Util.QuaternionSafeLookRotation(GetAimRay().direction),
                    base.gameObject,
                    Modules.StaticValues.standardBombBlastDamageCoefficient * this.damageStat,
                    4000f,
                    base.RollCrit(),
                    DamageColorIndex.Default,
                    null,
                    force * Modules.Config.bombMaxThrowPower.Value);
            }
        }

        public void RuneBombSpecificFunction() 
        {
            linkController.DisableRuneBombInHand();
            itemThrown = true;
            //Fire using network request
            if (isAuthority)
            {
                new RuneBombSpawnNetworkRequest(characterBody.masterObjectId, GetAimRay().origin, GetAimRay().direction, force * Modules.Config.bombMaxThrowPower.Value).Send(R2API.Networking.NetworkDestination.Clients);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (fixedAge >= duration * throwItemFraction && itemThrown)
            {
                return InterruptPriority.Skill;
            }
            return InterruptPriority.Frozen;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(totalDuration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            totalDuration = reader.ReadSingle();
        }
    }
}