using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSword : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            //Entry point for all swings.

            //Handle Grounded swings 
                //Provide entry point into grounded swing
                //let the grounded swing handle the swing progression if the player is holding the button down.

            //Handle Aerial swing
                //Check if pointing down
                    //Do downstab
                        //If the player hits the ground before they hit the ground, play the recovery state. 
                        //Otherwise allow them to transition out of the state.
                //otherwise do double swing.
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
