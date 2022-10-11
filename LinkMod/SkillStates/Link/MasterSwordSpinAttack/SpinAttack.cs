using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.MasterSwordSpinAttack
{
    internal class SpinAttack : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            //Entry point for all swings.

            //Target is Grounded
            if (base.isGrounded)
            {
                //Do Default swing
                this.outer.SetState(new GroundedSpinAttackStart { });
                return;
            }

            //Target is in the air
            this.outer.SetState(new AerialSpinAttack { });
       }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}
