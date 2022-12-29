using EntityStates;
using LinkMod.Content.Link;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace LinkMod.SkillStates.Link.BowAndArrow
{
    internal class FireBow : BaseSkillState
    {
        internal LinkController linkController;
        internal Animator anim;
        internal LinkArrowController arrowController;
        internal bool hasFired;
        internal GameObject selectedArrowPrefab;
        internal float totalDurationHeld;

        internal static float baseDuration = 0.8f;
        internal static float disableArrowFrac = 0f;
        internal static float fireArrow = 0.02f;
        internal static float disableBowFrac = 0.5f;
        internal static float unsheatheFrac = 0.5f;
        internal static float earlyExitFrac = 0.3f;
        internal float stopwatch = 0f;
        internal float duration;
        internal bool isCriticallyCharged;
        public override void OnEnter()
        {
            base.OnEnter();
            linkController = gameObject.GetComponent<LinkController>();
            arrowController = gameObject.GetComponent<LinkArrowController>();
            duration = baseDuration / base.attackSpeedStat;
            anim = base.GetModelAnimator();
            anim.SetFloat("Swing.playbackRate", base.attackSpeedStat);
            linkController.isCharged = false;
            linkController.isCharging = false;

            base.PlayAnimation("UpperBody, Override", "BowFire", "Swing.playbackRate", duration);
            hasFired = false;
            
            //Choose arrow type, arrow firing type
            //FUCK We'll need a prefab for each type of arrow
            selectedArrowPrefab = GetSelectedArrowType();
        }

        public override void OnExit()
        {
            base.OnExit();
            linkController.SetUnsheathed();
            linkController.DisableArrowInHand();
            linkController.DisableBowInHand();
        }

        //Return the right prefab for the given situation.
        public GameObject GetSelectedArrowType() 
        {
            switch (arrowController.arrowTypeEquipped) 
            {
                case LinkArrowController.ArrowTypeEquipped.NORMAL:
                    return Modules.Projectiles.basicArrowPrefab;
                case LinkArrowController.ArrowTypeEquipped.FIRE:
                    return Modules.Projectiles.basicArrowPrefab;
                case LinkArrowController.ArrowTypeEquipped.ICE:
                    return Modules.Projectiles.basicArrowPrefab;
                case LinkArrowController.ArrowTypeEquipped.LIGHT:
                    return Modules.Projectiles.basicArrowPrefab;
                case LinkArrowController.ArrowTypeEquipped.ANCIENT:
                    return Modules.Projectiles.basicArrowPrefab;
                case LinkArrowController.ArrowTypeEquipped.BOMB:
                    return Modules.Projectiles.basicArrowPrefab;
                default:
                    return Modules.Projectiles.basicArrowPrefab;
            }
        }

        public override void Update()
        {
            base.Update();
            stopwatch += Time.deltaTime;
            if (base.isAuthority)
            {
                if (!hasFired) 
                {
                    float speed = Modules.Config.arrowMaxSpeed.Value * Mathf.Clamp(totalDurationHeld / HoldBow.baseCriticalCharge, 0.6f, 1f);
                    if (stopwatch >= duration * fireArrow) 
                    {
                        hasFired = true;
                        //Fire the arrow.
                        ProjectileManager.instance.FireProjectile(selectedArrowPrefab,
                            GetAimRay().origin,
                            Util.QuaternionSafeLookRotation(GetAimRay().direction),
                            base.gameObject,
                            Modules.StaticValues.arrowDamageCoefficient * this.damageStat,
                            300f,
                            base.RollCrit(),
                            DamageColorIndex.Default,
                            null,
                            speed);
                    }
                }

                //check if we should transition back into arrow fire.
                if (stopwatch >= duration * earlyExitFrac)
                {
                    //check if we are in arrow mode and set them back to drawbow.
                    //if not we don't do squat.
                    if (linkController.selectedLoadout == LinkController.SelectedLoadout.ARROW)
                    {
                        if (base.inputBank.skill1.down)
                        {
                            //go back to draw.
                            base.outer.SetState(new DrawBow { });
                        }
                    }
                    else 
                    {
                        if (base.inputBank.skill1.down) 
                        {
                            base.outer.SetNextStateToMain();
                        }
                    }
                }
            }

            //handle animation outside independent of client.
            if (stopwatch >= duration * disableArrowFrac)
            {
                linkController.DisableArrowInHand();
            }
            if (stopwatch >= duration * disableBowFrac)
            {
                linkController.DisableBowInHand();
            }
            if (stopwatch >= duration * unsheatheFrac)
            {
                linkController.SetUnsheathed();
            }

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
