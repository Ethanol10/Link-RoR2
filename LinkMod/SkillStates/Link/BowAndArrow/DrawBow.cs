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
        internal static float removeSwordShieldFrac = 0.01f;
        internal static float showBowFrac = 0.01f;
        internal static float showArrowFrac = 0.01f;
        internal static float baseDuration = 0.865f;
        internal float duration;
        internal float stopwatch;

        public override void OnEnter()
        {
            base.OnEnter();
            linkController = gameObject.GetComponent<LinkController>();
            anim = base.GetModelAnimator();
            stopwatch = 0f;

            base.StartAimMode(0.5f + this.duration, false);
            duration = baseDuration / base.attackSpeedStat;
            anim.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            base.PlayAnimation("UpperBody, Override", "BowDraw", "Swing.playbackRate", duration);
            linkController.isCharging = true;
            //Show the draw animation.
            //after set duration transition to Hold
            //stay in hold
            
            //terminate if the player lets go of m1.
        }

        public override void OnExit()
        {
            base.OnExit();
            //This should be set as default for this state and will transition back once the player is done shooting.
            linkController.SetSheathed();
            linkController.EnableBowInHand();
            linkController.EnableArrowInHand();
            linkController.isCharging = true;
        }

        public override void Update()
        {
            base.Update();
            stopwatch += Time.fixedDeltaTime;

            if (base.isAuthority) 
            {
                //Check input. needs to be responsive to input.
                if (base.inputBank.skill1.down)
                {
                    if (stopwatch >= duration) 
                    {
                        //move to hold.
                        base.outer.SetState(new HoldBow { });
                    }

                }
                else 
                {
                    //Exit
                    base.outer.SetState(new FireBow { });
                }
            }

            //Visible stuff.
            if (stopwatch >= duration * removeSwordShieldFrac)
            {
                linkController.SetSheathed();
            }
            if (stopwatch >= duration * showBowFrac)
            {
                linkController.EnableBowInHand();
            }
            if (stopwatch >= duration * showArrowFrac)
            {
                linkController.EnableArrowInHand();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return base.GetMinimumInterruptPriority();
        }
    }
}
