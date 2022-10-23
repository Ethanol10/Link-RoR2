using EntityStates;
using RoR2.Skills;
using LinkMod.Content.Link;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.SwapLoadout
{
    internal class SwapSwordLoadout : BaseSkillState
    {
        internal LinkController linkController;

        public override void OnEnter()
        {
            base.OnEnter();
            linkController = base.gameObject.GetComponent<LinkController>();
            //Check loadout
            //if current loadout matches this one, do nothing.
            if (linkController.selectedLoadout == LinkController.SelectedLoadout.SWORD) 
            {
                linkController.selectedLoadout = LinkController.SelectedLoadout.SWORD;
                this.outer.SetNextStateToMain();
                return;
            }

            //Convert to Sword. We assume this is the base state and we unset towards this.
            //We need to know what skill has been selected to unset.
            SkillDef primarySkillDef = characterBody.skillLocator.primary.skillDef;
            SkillDef secondarySkillDef = characterBody.skillLocator.secondary.skillDef;
            SkillDef utilitySkillDef = characterBody.skillLocator.utility.skillDef;
            SkillDef specialSkillDef = characterBody.skillLocator.special.skillDef;

            characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.skillLocator.primary, primarySkillDef, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.secondary.UnsetSkillOverride(characterBody.skillLocator.secondary, secondarySkillDef, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.utility.UnsetSkillOverride(characterBody.skillLocator.utility, utilitySkillDef, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.special.UnsetSkillOverride(characterBody.skillLocator.special, specialSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (linkController.handState == LinkController.HandState.INHAND) 
            {
                characterBody.skillLocator.secondary.SetSkillOverride(characterBody.skillLocator.secondary, LinkMod.Content.Link.Link.ItemHold, GenericSkill.SkillOverridePriority.Contextual);
            }
            linkController.selectedLoadout = LinkController.SelectedLoadout.SWORD;
            this.outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return base.GetMinimumInterruptPriority();
        }
    }
}

