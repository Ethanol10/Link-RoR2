using EntityStates;
using LinkMod.Content.Link;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.SwapLoadout
{
    internal class SwapArrowLoadout : BaseSkillState
    {
        internal LinkController linkController;
        public override void OnEnter()
        {
            base.OnEnter();
            linkController = base.gameObject.GetComponent<LinkController>();
            //Check loadout
            //if current loadout matches this one, do nothing.
            if (linkController.selectedLoadout == LinkController.SelectedLoadout.ARROW)
            {
                linkController.selectedLoadout = LinkController.SelectedLoadout.ARROW;
                this.outer.SetNextStateToMain();
                return;
            }

            //Convert to Arrow. Sword is base and we need to check if we are converting from NOT sword to unset everything before setting a skill override.
            if (linkController.selectedLoadout != LinkController.SelectedLoadout.SWORD) 
            {
                //Unset everything

                SkillDef primarySkillDef = characterBody.skillLocator.primary.skillDef;
                SkillDef secondarySkillDef = characterBody.skillLocator.secondary.skillDef;
                SkillDef utilitySkillDef = characterBody.skillLocator.utility.skillDef;
                SkillDef specialSkillDef = characterBody.skillLocator.special.skillDef;

                characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.skillLocator.primary, primarySkillDef, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.secondary.UnsetSkillOverride(characterBody.skillLocator.secondary, secondarySkillDef, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.utility.UnsetSkillOverride(characterBody.skillLocator.utility, utilitySkillDef, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.special.UnsetSkillOverride(characterBody.skillLocator.special, specialSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }

            //Now set to Arrow.
            characterBody.skillLocator.primary.SetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.arrowEntryPoint, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.secondary.SetSkillOverride(characterBody.skillLocator.secondary, LinkMod.Content.Link.Link.swapArrowFiringType, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.utility.SetSkillOverride(characterBody.skillLocator.utility, LinkMod.Content.Link.Link.swapArrowEquipped, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.special.SetSkillOverride(characterBody.skillLocator.special, LinkMod.Content.Link.Link.activateComboShot, GenericSkill.SkillOverridePriority.Contextual);

            linkController.selectedLoadout = LinkController.SelectedLoadout.ARROW;
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
