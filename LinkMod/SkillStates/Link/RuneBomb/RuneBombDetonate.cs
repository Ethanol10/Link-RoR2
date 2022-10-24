using EntityStates;
using LinkMod.Content.Link;
using LinkMod.Modules.Networking.Miscellaneous;
using R2API.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombDetonate : BaseSkillState
    {
        internal static float baseDuration = 1.5f;
        internal float duration;
        internal float detonateFraction = 0.13f;
        internal float takeShieldFraction = 0.58f;
        internal bool detonated;
        internal bool shieldtaken;

        internal LinkController linkController;
        internal Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            linkController = base.gameObject.GetComponent<LinkController>();
            duration = baseDuration / base.attackSpeedStat;
            linkController.SetSwordOnlyUnsheathed();

            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            base.PlayAnimation("UpperBody, Override", "TriggerRuneBomb", "Swing.playbackRate", duration);
            detonated = false;
            shieldtaken = false;

            characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombDetonate, RoR2.GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.primary.SetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombSpawn, RoR2.GenericSkill.SkillOverridePriority.Contextual);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration * detonateFraction && !detonated) 
            {
                detonated = true;
                if (base.isAuthority) 
                {
                    new RuneBombDestroyNetworkRequest(characterBody.masterObjectId).Send(R2API.Networking.NetworkDestination.Clients);
                }   
            }
            if (base.fixedAge > duration * takeShieldFraction && !shieldtaken) 
            {
                linkController.SetUnsheathed();
            }

            if (base.fixedAge > duration && base.isAuthority) 
            {
                base.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}