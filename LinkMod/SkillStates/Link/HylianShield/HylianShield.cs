using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.HylianShield
{
    internal class HylianShield : BaseSkillState
    {
        internal float baseDuration = 0.1f;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("UpperBody, Override", "ShieldBlockHold");
        }

        public override void OnExit()
        {
            base.OnExit();
            LinkController linkcon = gameObject.GetComponent<LinkController>();
            linkcon.isShielding = false;
            ChildLocator childLocator = base.GetModelChildLocator();
            childLocator.FindChild("ShieldHurtboxParent").gameObject.SetActive(false);

            if (NetworkServer.active)
            {
                base.characterBody.SetBuffCount(Modules.Buffs.HylianShieldBuff.buffIndex, 0);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(baseDuration * 2.0f, false);
            //Disallow user from exiting state until user presses to get out of the state.
            if (base.isAuthority) 
            {
                if (!base.inputBank.skill2.down) 
                {
                    this.outer.SetNextState(new HylianShieldExit());
                }
            }
            if (base.fixedAge >= baseDuration) 
            {
                base.fixedAge = 0f;
                base.PlayCrossfade("UpperBody, Override", "ShieldBlockHold", 0.06f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
