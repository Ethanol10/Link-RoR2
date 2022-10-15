using EntityStates;
using RoR2;

namespace LinkMod.SkillStates.Link
{
    internal class LinkCharacterMain : GenericCharacterMain
    {
        private EntityStateMachine weaponStateMachine; 

        //SHAMELESS RIPPED FROM MAGE LMAO
        public override void OnEnter()
        {
            base.OnEnter();
            this.weaponStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Weapon");
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
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
