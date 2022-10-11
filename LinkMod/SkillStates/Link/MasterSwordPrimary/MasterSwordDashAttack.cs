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
        internal static float baseDuration = 1.5f;
        internal static float hurtBoxFractionStart = 0.28f;
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
        internal float hitStopDuration = 0.25f;
        internal bool inHitStop;
        internal Vector3 storedVelocity;
        internal float hitPauseTimer;
        internal bool hasHopped;


        //"Roll" part
        public float rollDuration = 0.3f;
        public static float initialSpeedCoefficient = 4f;
        public static float finalSpeedCoefficient = 0f;

        private float rollSpeed;
        private Vector3 forwardDirection;
        private Vector3 previousPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            base.characterBody.isSprinting = false;
            this.animator = base.GetModelAnimator();
            duration = baseDuration / this.attackSpeedStat;
            hitHopVelocity = 10f / this.attackSpeedStat;
            rollDuration = duration * rollDuration;
            SetupOverlapAttack();

            animator.SetFloat("Swing.playbackRate", this.attackSpeedStat);
            PlayAttackAnimation();

            //We need a separate stopwatch to let the hitpause timer tick down.
            stopwatch = 0f;

            //The roll part, we want it to roll irrespective of the move length.
            if (base.isAuthority && base.inputBank && base.characterDirection)
            {
                this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            }

            Vector3 rhs = base.characterDirection ? base.characterDirection.forward : this.forwardDirection;
            Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);

            float num = Vector3.Dot(this.forwardDirection, rhs);
            float num2 = Vector3.Dot(this.forwardDirection, rhs2);

            this.RecalculateRollSpeed();

            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity = this.forwardDirection * this.rollSpeed;
                base.characterMotor.velocity.y = 0f;
            }

            Vector3 b = base.characterMotor ? base.characterMotor.velocity : Vector3.zero;
            this.previousPosition = base.transform.position - b;
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        private void RecalculateRollSpeed()
        {
            this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, stopwatch / rollDuration);
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
                        this.outer.SetState(new MasterSword { });
                    }
                }

                //Otherwise exit out completely back to main state.
                if (stopwatch >= duration)
                {
                    this.outer.SetNextStateToMain();
                }
            }

            if (stopwatch < rollDuration)
            {
                //Roll doesn't need to be in authority.
                this.RecalculateRollSpeed();

                if (base.characterDirection) base.characterDirection.forward = this.forwardDirection;
                if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = Mathf.Lerp(Roll.dodgeFOV, 60f, stopwatch / rollDuration);

                Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
                if (base.characterMotor && base.characterDirection && normalized != Vector3.zero)
                {
                    Vector3 vector = normalized * this.rollSpeed;
                    float d = Mathf.Max(Vector3.Dot(vector, this.forwardDirection), 0f);
                    vector = this.forwardDirection * d;

                    base.characterMotor.velocity = vector;
                }
                this.previousPosition = base.transform.position;
            }
            else 
            {
                base.characterMotor.velocity = Vector3.zero;
            }
        }

        public void SetupOverlapAttack()
        {
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = System.Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(),
                    (HitBoxGroup element) => element.groupName == "GroundedDashAttack");
            }

            this.attack = new OverlapAttack
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                damage = Modules.StaticValues.msGroundedDashAttack * this.damageStat,
                procCoefficient = 1.5f,
                forceVector = Vector3.forward,
                pushAwayForce = 500f,
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
