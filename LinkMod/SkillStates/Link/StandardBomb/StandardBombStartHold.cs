using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.StandardBomb
{
    internal class StandardBombStartHold : BaseSkillState
    {
        internal static float baseDuration = 0.5f;

        internal float duration;
        internal Animator animator;
        internal LinkController linkController;

        public override void OnEnter() 
        {
            base.OnEnter();
            linkController = base.gameObject.GetComponent<LinkController>();
            animator = base.GetModelAnimator();

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
            return InterruptPriority.Frozen;
        }
    }
}