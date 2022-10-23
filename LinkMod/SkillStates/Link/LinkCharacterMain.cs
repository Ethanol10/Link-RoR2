using EntityStates;
using LinkMod.Content.Link;
using RoR2;
using UnityEngine;

namespace LinkMod.SkillStates.Link
{
    internal class LinkCharacterMain : GenericCharacterMain
    {
        private EntityStateMachine weaponStateMachine; 
        private LinkController linkController;

        //SHAMELESS RIPPED FROM MAGE LMAO
        public override void OnEnter()
        {
            base.OnEnter();
            this.weaponStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
            linkController = gameObject.GetComponent<LinkController>();
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            if(linkController)
            {
                if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority && !linkController.isShielding && linkController.handState != LinkController.HandState.INHAND)
                {
                    bool CheckJumpingHold = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;
                    bool flag = this.weaponStateMachine.state.GetType() == typeof(ParasailOn);
                    
                    if (CheckJumpingHold && !flag)
                    {
                        this.weaponStateMachine.SetNextState(new ParasailOn());
                    }
                    if (!CheckJumpingHold && flag)
                    {
                        this.weaponStateMachine.SetNextState(new Idle());
                    }
                }
            }
            else
            {
                linkController = gameObject.GetComponent<LinkController>();
            }
        }

        public override void OnExit()
        {
            if (base.isAuthority && this.weaponStateMachine)
            {
                this.weaponStateMachine.SetNextState(new Idle());
            }
            base.OnExit();
        }
    }
}
