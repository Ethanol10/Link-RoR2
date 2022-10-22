using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    internal class RuneBombSpawn : BaseSkillState
    {
        internal static float baseDuration = 1.5f;
        internal static float sheatheFraction = 0.1f;
        internal static float unsheatheSwordFraction = 0.68f;
        internal static float bombSpawnFraction = 0.21f;
        internal bool sheathe;
        internal bool unsheatheSword;
        internal bool bombEnabled;

        internal float duration;
        internal Animator animator;
        internal LinkController linkController;

        public override void OnEnter() 
        {
            base.OnEnter();
            duration = baseDuration / base.attackSpeedStat;
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            linkController = base.gameObject.GetComponent<LinkController>();

            base.PlayAnimation("UpperBody, Override", "DeployBomb", "Swing.playbackRate", duration);
            linkController.bombState = LinkController.BombState.INHAND;
            linkController.bombTypeInHand = LinkController.BombTypeInHand.RUNE;
            sheathe = false;
            unsheatheSword = false;
            bombEnabled = false;

            linkController.DisableRuneBombInHand();
            linkController.DisableFakeRuneBombInHand();

            //assume we're in bomb loadout.
            // Set all skills regarding hylian shield to throw.
            // Set all skills regarding spawning a bomb to throw.

            characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombSpawn, RoR2.GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.primary.SetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombHold, RoR2.GenericSkill.SkillOverridePriority.Contextual);
        }

        public override void OnExit()
        {
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
            base.OnExit();
            linkController.bombState = LinkController.BombState.INHAND;
            linkController.bombTypeInHand = LinkController.BombTypeInHand.RUNE;
            linkController.DisableFakeRuneBombInHand();
            linkController.EnableRuneBombInHand();
            linkController.SetSwordOnlyUnsheathed();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= duration * sheatheFraction && !sheathe)
            {
                linkController.SetSheathed();
                sheathe = true;
            }
            if (base.fixedAge >= duration * bombSpawnFraction && !bombEnabled) 
            {
                bombEnabled = true;
                linkController.EnableFakeRuneBombInHand();
            }
            if (base.fixedAge >= duration * unsheatheSwordFraction && !unsheatheSword) 
            {
                linkController.SetSwordOnlyUnsheathed();
                unsheatheSword = true;
            }
            if (base.fixedAge >= duration) 
            {
                linkController.DisableFakeRuneBombInHand();
                linkController.EnableRuneBombInHand();
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}