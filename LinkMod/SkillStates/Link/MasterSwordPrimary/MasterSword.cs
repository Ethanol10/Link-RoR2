using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSword : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            //Entry point for all swings.
            
            //Target is Grounded
            if (base.isGrounded) 
            {
                //Check if dashing
                if (base.characterBody.isSprinting) 
                {
                    this.outer.SetState(new MasterSwordDashAttack { });
                    return;
                }

                //Do Default swing
                this.outer.SetState(new MasterSwordSwing { });
                return;
            }

            //Continue under the assumption that the character is in the air
            if (CheckLookingDown()) 
            {
                this.outer.SetState(new MasterSwordAerialDownstab { });
                return;
            }

            //otherwise just default to aerial attack
            this.outer.SetState(new MasterSwordAerialDoubleSwing { });
            return;
       }

        private bool CheckLookingDown()
        {
            if (Vector3.Dot(base.GetAimRay().direction, Vector3.down) > 0.8f)
            {
                return true;
            }
            return false;
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
