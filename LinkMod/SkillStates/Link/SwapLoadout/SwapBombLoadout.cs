﻿using EntityStates;
using LinkMod.Content.Link;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.SwapLoadout
{
    internal class SwapBombLoadout : BaseSkillState
    {
        internal LinkController linkController;

        public override void OnEnter()
        {
            base.OnEnter();
            linkController = base.gameObject.GetComponent<LinkController>();

            //Check loadout
            //if current loadout matches this one, do nothing.
            if (linkController.selectedLoadout == LinkController.SelectedLoadout.BOMB)
            {
                this.outer.SetNextStateToMain();
                linkController.selectedLoadout = LinkController.SelectedLoadout.BOMB;
                return;
            }

            //Unset the entire thing
            if (linkController.selectedLoadout != LinkController.SelectedLoadout.SWORD) 
            {
                SkillDef primary = characterBody.skillLocator.primary.skillDef;
                SkillDef secondary = characterBody.skillLocator.secondary.skillDef;
                SkillDef utility = characterBody.skillLocator.utility.skillDef;
                SkillDef special = characterBody.skillLocator.special.skillDef;

                characterBody.skillLocator.primary.UnsetSkillOverride(characterBody.skillLocator.primary, primary, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.secondary.UnsetSkillOverride(characterBody.skillLocator.secondary, secondary, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.utility.UnsetSkillOverride(characterBody.skillLocator.utility, utility, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.special.UnsetSkillOverride(characterBody.skillLocator.special, special, GenericSkill.SkillOverridePriority.Contextual);
            }

            if (linkController.runeBombThrown) 
            {
                //set rune bomb only to detonate.
                characterBody.skillLocator.primary.SetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombDetonate, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.secondary.SetSkillOverride(characterBody.skillLocator.secondary, LinkMod.Content.Link.Link.hylianShieldSkillDef, GenericSkill.SkillOverridePriority.Contextual);

                //Check this separately.
                characterBody.skillLocator.utility.SetSkillOverride(characterBody.skillLocator.utility, LinkMod.Content.Link.Link.standardBombSpawn, GenericSkill.SkillOverridePriority.Contextual);
                characterBody.skillLocator.special.SetSkillOverride(characterBody.skillLocator.special, LinkMod.Content.Link.Link.superBombSpawn, GenericSkill.SkillOverridePriority.Contextual);

                linkController.selectedLoadout = LinkController.SelectedLoadout.BOMB;
                this.outer.SetNextStateToMain();
                return;
            }
            //Check if the bomb has been deployed.
            //Check if the rune bomb has been thrown.

            //Default case
            characterBody.skillLocator.primary.SetSkillOverride(characterBody.skillLocator.primary, LinkMod.Content.Link.Link.runeBombSpawn, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.secondary.SetSkillOverride(characterBody.skillLocator.secondary, LinkMod.Content.Link.Link.hylianShieldSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.utility.SetSkillOverride(characterBody.skillLocator.utility, LinkMod.Content.Link.Link.standardBombSpawn, GenericSkill.SkillOverridePriority.Contextual);
            characterBody.skillLocator.special.SetSkillOverride(characterBody.skillLocator.special, LinkMod.Content.Link.Link.superBombSpawn, GenericSkill.SkillOverridePriority.Contextual);
            linkController.selectedLoadout = LinkController.SelectedLoadout.BOMB;
            this.outer.SetNextStateToMain();
            return;
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
