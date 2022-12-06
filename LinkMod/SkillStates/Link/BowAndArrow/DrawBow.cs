using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.BowAndArrow
{
    internal class DrawBow : BaseSkillState
    {
        internal LinkController linkController;
        internal Animator anim;
        internal static float removeSwordShieldFrac;
        internal static float showBowFrac;
        internal static float showArrowFrac;

        public override void OnEnter()
        {
            base.OnEnter();
            linkController = gameObject.GetComponent<LinkController>();

            //Show the draw animation.
            //after set duration transition to Hold
            //stay in hold
            
            //terminate if the player lets go of m1.
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
