using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordSwing : BaseSkillState
    {
        internal static float baseDuration = 1.0f;
        internal static float hurtBoxFractionStart;
        internal static float hurtboxFractionEnd;
        internal float duration;
        internal bool hasFired;

        internal Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            this.animator = base.GetModelAnimator();
            duration = baseDuration / this.attackSpeedStat;
            base.StartAimMode(0.5f + this.duration, false);
            Chat.AddMessage("Swing");
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > duration * hurtBoxFractionStart 
                && base.fixedAge < duration * hurtboxFractionEnd) 
            {
                hasFired = true;
                //fire
            }

            //End move
            if (base.fixedAge > duration) 
            {
                if (!hasFired) 
                {
                    //fire
                }

                if (base.isAuthority) 
                {
                    if (this.IsKeyDownAuthority()) 
                    {
                        //go to same state but increase index.
                    }
                }
            }
        }
    }
}
