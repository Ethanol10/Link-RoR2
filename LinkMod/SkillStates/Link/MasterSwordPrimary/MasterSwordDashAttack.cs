using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordDashAttack : BaseSkillState
    {
        internal static float baseDuration = 2.0f;
        internal static float hurtBoxFractionStart = 0.28f;
        internal static float hurtboxFractionEnd = 0.34f;
        internal static float earlyExitTime = 0.8f;
        internal float hitHopVelocity = 10f;
        internal float duration;
        internal bool hasFired;
        internal OverlapAttack attack;
        internal Animator animator;
        internal int index;
        internal float stopwatch;

        //Hitstop stuff
        internal HitStopCachedState hitstopCache;
        internal float hitStopDuration = 0.25f;
        internal bool inHitStop;
        internal Vector3 storedVelocity;
        internal float hitPauseTimer;
        internal bool hasHopped;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            base.characterBody.isSprinting = false;
            this.animator = base.GetModelAnimator();
            duration = baseDuration / this.attackSpeedStat;
            hitHopVelocity = 10f / this.attackSpeedStat;
            SetupOverlapAttack();

            animator.SetFloat("Swing.playbackRate", this.attackSpeedStat);
            PlayAttackAnimation();

            //We need a separate stopwatch to let the hitpause timer tick down.
            stopwatch = 0f;
        }

        public override void OnExit()
        {
            base.OnExit();
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
            base.PlayAnimation("FullBody, Override", "GroundedDashAttack", "Swing.playbackRate", this.duration);
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

                        //Go back to the beginning.
                        this.outer.SetState(new MasterSwordSwing { index = 0 });
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
                    (HitBoxGroup element) => element.groupName == "GroundedFinalSwingHitbox");
            }

            this.attack = new OverlapAttack
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                damage = Modules.StaticValues.msGroundedFinalSwing * this.damageStat,
                procCoefficient = 0.75f,
                forceVector = Vector3.zero,
                pushAwayForce = 0f,
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
