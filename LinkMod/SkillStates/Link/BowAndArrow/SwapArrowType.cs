using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link.BowAndArrow
{
    internal class SwapArrowType : BaseSkillState
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
            if (arrowFireType > 6)
            {
                arrowFireType = 1;
            }
            arrowController.SetArrowEquippedType(arrowFireType);

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
