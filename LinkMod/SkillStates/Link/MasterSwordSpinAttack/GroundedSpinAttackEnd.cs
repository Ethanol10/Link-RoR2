using EntityStates;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.MasterSwordSpinAttack
{
    internal class GroundedSpinAttackEnd : BaseSkillState
    {
        internal float totalDurationHeld;
        internal static float baseDuration = 1.5f;
        internal float boostedDamage;
        internal float duration;
        internal float excessAmount;
        internal float lengthBetweenTicks;
        internal BlastAttack suckingBlastAttack;
        internal BlastAttack finalBlastAttack;
        internal float stopwatch;
        internal Animator animator;
        internal int maxNoOfBlasts;
        internal int noOfBlasts;
        internal float blastTimer;
        internal float startBlasting = 0.0f;
        internal float endBlasting = 0.4f;
        internal float MajorBlast = 0.50f;
        internal bool hasFired;

        //Hitstop stuff
        internal HitStopCachedState hitstopCache;
        internal float lightHitStopDuration = 0.10f;
        internal float heavyHitStopDuration = 0.3f;
        internal float hitHopVelocity = 5f;
        internal bool inHitStop;
        internal Vector3 storedVelocity;
        internal float hitPauseTimer;
        internal bool hasHopped;

        public override void OnEnter()
        {
            base.OnEnter();
            stopwatch = 0f;
            duration = baseDuration;
            excessAmount = 0f;
            blastTimer = 0f;
            hasFired = false;
            noOfBlasts = 0;
            maxNoOfBlasts = (int)Modules.StaticValues.spinAttackBaseMinorHit * (base.attackSpeedStat >= 1 ? (int)base.attackSpeedStat : 1);
            lengthBetweenTicks = (float)((duration * endBlasting) / maxNoOfBlasts);
            animator = base.GetModelAnimator();

            animator.SetFloat("Swing.playbackRate", 1.0f);
            inHitStop = false;

            if (totalDurationHeld > 1.0f)
            {
                //Do scaled Damage.
                boostedDamage = totalDurationHeld;
                if (boostedDamage > Modules.StaticValues.spinAttackMaxMultiplier) 
                {
                    boostedDamage = Modules.StaticValues.spinAttackMaxMultiplier;
                    //allow them to boost damage further if they hold it, but they will get diminishing boosts
                    excessAmount = boostedDamage - Modules.StaticValues.spinAttackMaxMultiplier;
                    if (excessAmount < 0)
                    {
                        excessAmount = 0;
                    }
                    else 
                    {
                        boostedDamage += excessAmount / 2.0f;
                    }
                }
            }
            else 
            {
                boostedDamage = 1f;
            }

            SetupBlastAttacks();

            //Don't scale the speed of the attack because we want it to last the entire baseDuration
            base.PlayAnimation("FullBody, Override", "GroundedSpinAttack", "Swing.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                base.characterBody.SetBuffCount(Modules.Buffs.SpinAttackSlowDebuff.buffIndex, 0);
            }
        }

        public void OnHitEnemyAuthority(bool isLightHit)
        {
            if (!hasHopped)
            {
                if (base.characterMotor && !base.characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    base.SmallHop(base.characterMotor, hitHopVelocity);
                }

                hasHopped = true;
            }

            if (isLightHit)
            {
                if (!this.inHitStop && this.lightHitStopDuration > 0f)
                {
                    storedVelocity = base.characterMotor.velocity;
                    hitstopCache = base.CreateHitStopCachedState(base.characterMotor, animator, "Swing.playbackRate");
                    hitPauseTimer = lightHitStopDuration;
                    inHitStop = true;
                }
            }
            else 
            {
                if (!this.inHitStop && this.heavyHitStopDuration > 0f)
                {
                    storedVelocity = base.characterMotor.velocity;
                    hitstopCache = base.CreateHitStopCachedState(base.characterMotor, animator, "Swing.playbackRate");
                    hitPauseTimer = heavyHitStopDuration;
                    inHitStop = true;
                }
            }   
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority) 
            {
                Debug.Log($"stopwatch: {stopwatch} blastTimer: {blastTimer} tickRate: {lengthBetweenTicks} maxNoBlast: {maxNoOfBlasts}");
                //Increment stopwatch if we are in hitpause
                if (!inHitStop)
                {
                    this.stopwatch += Time.fixedDeltaTime;
                    this.blastTimer += Time.fixedDeltaTime;
                }
                else
                {
                    //Increment Timers for hitstun
                    this.hitPauseTimer -= Time.fixedDeltaTime;
                    //Set velocity to zero, and punch playback rate to 0 to simulate a strong hit
                    if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                    if (this.animator) this.animator.SetFloat("Swing.playbackRate", 0f);
                }

                if (hitPauseTimer <= 0f && inHitStop)
                {
                    ConsumeHitStopCachedState(hitstopCache, characterMotor, animator);
                    inHitStop = false;
                    characterMotor.velocity = storedVelocity;
                }
                if (noOfBlasts <= maxNoOfBlasts)
                {
                    if (blastTimer > lengthBetweenTicks)
                    {
                        noOfBlasts++;
                        suckingBlastAttack.position = base.gameObject.transform.position;
                        BlastAttack.Result result = suckingBlastAttack.Fire();
                        if (result.hitCount > 0)
                        {
                            OnHitEnemyAuthority(true);
                        }
                        blastTimer = 0f;
                    }
                }
                else 
                {
                    if (stopwatch > duration * MajorBlast && !hasFired) 
                    {
                        hasFired = true;
                        finalBlastAttack.position = base.gameObject.transform.position;
                        BlastAttack.Result result = finalBlastAttack.Fire();
                        if (result.hitCount > 0) 
                        {
                            OnHitEnemyAuthority(false);
                        }
                    }
                }

                if (stopwatch > duration) 
                {
                    base.outer.SetNextStateToMain();
                }
            }
        }

        public void SetupBlastAttacks() 
        {
            suckingBlastAttack = new BlastAttack 
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                baseDamage = Modules.StaticValues.spinAttackMinorBlastDamageCoefficient * base.damageStat * boostedDamage,
                procCoefficient = 1.0f,
                baseForce = -500f,
                radius = Modules.StaticValues.spinAttackMinorRadius,
                crit = base.RollCrit()
            };

            finalBlastAttack = new BlastAttack
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                baseDamage = Modules.StaticValues.spinAttackMajorBlastDamageCoefficientBase * base.damageStat * boostedDamage,
                procCoefficient = 1.0f,
                baseForce = 1000f,
                radius = Modules.StaticValues.spinAttackMajorRadius,
                crit = base.RollCrit()
            };
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.totalDurationHeld);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.totalDurationHeld = reader.ReadSingle();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
