using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.BowAndArrow
{
    internal class HoldBow : BaseSkillState
    {
        //Client has held down the button and is still holding it down.
        //We will have a critical charge, given the player has held the button down for longer than the initial animation.

        public static float baseCriticalCharge = 0.2f;
        public static float baseDuration = 0.65f;
        internal Animator anim;
        internal LinkController linkController;
        internal float stopwatch = 0f;
        internal float totalDuration = 0f;
        internal bool alreadyPlayingAnim = false;

        public override void OnEnter()
        {
            base.OnEnter();
            base.StartAimMode(0.5f + baseDuration, false);

            anim = base.GetModelAnimator();
            linkController = gameObject.GetComponent<LinkController>();

            //In case the player has not somehow set these correctly.
            linkController.SetSheathed();
            linkController.EnableBowInHand();
            linkController.EnableArrowInHand();
            if (totalDuration >= baseCriticalCharge * base.attackSpeedStat)
            {
                linkController.isCharging = false;
                linkController.isCharged = true;
            }
            else 
            {
                linkController.isCharging = true;
                linkController.isCharged = false;
            }

            if (!alreadyPlayingAnim) 
            {
                base.PlayAnimation("UpperBody, Override", "BowHold");
                alreadyPlayingAnim = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            linkController.SetSheathed();
            linkController.EnableBowInHand();
            linkController.EnableArrowInHand();
        }

        public override void Update()
        {
            base.Update();

            if (base.isAuthority) 
            {
                stopwatch += Time.fixedDeltaTime;
                //Input sensitive. needs to be in update.
                if (base.inputBank.skill1.down)
                {
                    //Reset the skill and add to the duration.
                    if (stopwatch > baseDuration)
                    {
                        base.outer.SetNextState(new HoldBow
                        {
                            totalDuration = totalDuration + stopwatch,
                            alreadyPlayingAnim = true
                        }); ;
                    }
                }
                else 
                {
                    bool criticallyCharged = false;
                    if(totalDuration > baseCriticalCharge * attackSpeedStat) 
                    {
                        criticallyCharged = true;
                    }
                    //Let go, fire.
                    base.outer.SetState(new FireBow 
                    {
                        isCriticallyCharged = criticallyCharged, 
                        totalDurationHeld = totalDuration 
                    });
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return base.GetMinimumInterruptPriority();
        }
    }
}
