using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.BowAndArrow
{
    internal class SwapArrowFireType : BaseSkillState
    {
        internal LinkController linkController;
        internal LinkArrowController arrowController;

        public override void OnEnter()
        {
            base.OnEnter();
            linkController = gameObject.GetComponent<LinkController>();
            arrowController = gameObject.GetComponent<LinkArrowController>();

            //switch arrow type.
            int arrowFireType = (int)arrowController.arrowFireType;
            arrowFireType += 1;
            if (arrowFireType > 3) 
            {
                arrowFireType = 1;
            }
            arrowController.SetArrowFireType(arrowFireType);
            
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            //State should play an animation, then cycle the type
            //If we are only rolling one type of arrow at every stage clear, we then have to
            //skip over the right arrow types and assign appropriately.
            //Show UI element too?
            
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}