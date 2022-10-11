using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordSwing : BaseSkillState
    {
        internal static float baseDuration = 0.7f;
        internal static float hurtBoxFractionStart = 0.2f;
        internal static float hurtboxFractionEnd = 0.4f;
        internal static float earlyExitTime = 0.45f;
        internal float hitHopVelocity = 10f;
        internal float duration;
        internal bool hasFired;
        internal OverlapAttack attack;
        internal Animator animator;
        internal int index;
        internal float stopwatch;

        //Hitstop stuff
        internal HitStopCachedState hitstopCache;
        internal float hitStopDuration = 0.10f;
        internal bool inHitStop;
        internal Vector3 storedVelocity;
        internal float hitPauseTimer;
        internal bool hasHopped;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            this.animator = base.GetModelAnimator();
            duration = baseDuration / this.attackSpeedStat;
            hitHopVelocity = 10f / this.attackSpeedStat;
            base.StartAimMode(0.5f + this.duration, false);
            SetupOverlapAttack();

            animator.SetFloat("Swing.playbackRate", this.attackSpeedStat); 
            PlayAttackAnimation();

            //We need a separate stopwatch to let the hitpause timer tick down.
            stopwatch = 0f;
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("UpperBody, Override", "BufferEmpty");
        }

        public void OnHitEnemyAuthority() 
        {
            if (!hasHopped)
            {
                if (base.characterMotor && !base.characterMotor.isGrounded && hitHopVelocity > 0f)
                {
                    base.SmallHop(base.characterMotor, hitHopVelocity);
                }

                hasHopped = true;
            }


            if (!this.inHitStop && this.hitStopDuration > 0f)
            {
                storedVelocity = base.characterMotor.velocity;
                hitstopCache = base.CreateHitStopCachedState(base.characterMotor, animator, "Swing.playbackRate");
                hitPauseTimer = hitStopDuration / attackSpeedStat;
                inHitStop = true;
            }
        }

        public void PlayAttackAnimation() 
        {
            base.PlayAnimation("UpperBody, Override", $"SwordSwing{index}", "Swing.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority) 
            {

                //Increment stopwatch if we are in hitpause
                if (!inHitStop)
                {
                    this.stopwatch += Time.fixedDeltaTime;
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

                //fire attack
                if (stopwatch >= duration * hurtBoxFractionStart
                && stopwatch <= duration * hurtboxFractionEnd)
                {
                    hasFired = true;
                    if (this.attack.Fire()) 
                    {
                        this.OnHitEnemyAuthority();
                    }
                    //fire
                }

                //End move
                if (stopwatch >= duration * earlyExitTime)
                {
                    if (!hasFired)
                    {
                        //fire if for some reason it didn't fire.
                        if (this.attack.Fire())
                        {
                            this.OnHitEnemyAuthority();
                        }
                    }

                    //Check if we should continue onwards.
                    if (this.inputBank.skill1.down)
                    {
                        //EEEeeeh let the MasterSword class handle it
                        if (!base.isGrounded) 
                        {
                            this.outer.SetState(new MasterSword { });
                            return;
                        }

                        if (index >= 1) 
                        {
                            this.outer.SetState(new MasterSwordSwingFinalGroundedHit { });
                            return;
                        }

                        this.outer.SetState(new MasterSwordSwing { index = 1 });
                        //go to same state but increase index.
                        //If index is 1 then transition to final grounded hit.
                    }
                }

                //Otherwise exit out completely back to main state.
                if (stopwatch >= duration) 
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public void SetupOverlapAttack()
        {
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = System.Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(),
                    (HitBoxGroup element) => element.groupName == "GroundedSwingHitbox");
            }

            this.attack = new OverlapAttack
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                damage = Modules.StaticValues.msGroundedBasicSwing * this.damageStat,
                procCoefficient = 1f,
                forceVector = Vector3.zero,
                pushAwayForce = 10f,
                hitBoxGroup = hitBoxGroup,
                isCrit = base.RollCrit(),
                
                //swingSoundString = 
                //impactSound =
                //hitEffectPrefab = 
            };
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.index);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.index = reader.ReadInt32();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
