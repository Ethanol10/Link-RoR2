using EntityStates;
using LinkMod.Modules.Networking.Miscellaneous;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;

namespace LinkMod.SkillStates.Link.MasterSwordSpinAttack
{
    internal class AerialSpinAttack : BaseSkillState
    {
        public static float baseDuration = 1.5f;
        public static float initialSpeedCoefficient = 5f;
        public static float finalSpeedCoefficient = 0f;
        public float duration;

        private float rollSpeed;
        private Vector3 movementVector;
        private Animator animator;
        private Vector3 previousPosition;

        public float stopwatch;
        internal int maxNoOfBlasts;
        internal int noOfBlasts;
        internal float blastTimer;
        internal float startBlasting = 0.0f;
        internal float endBlasting = 0.4f;
        internal float MajorBlast = 0.50f;
        internal bool hasFired;
        internal BlastAttack suckingBlastAttack;
        internal BlastAttack finalBlastAttack;
        internal float lengthBetweenTicks;

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
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", 1.0f);
            duration = baseDuration;
            stopwatch = 0f;

            base.PlayAnimation("FullBody, Override", "SpinAttackAir", "Swing.playbackRate", duration);

            RecalculateSpeed();

            //roll code
            Vector3 forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
            forwardDirection = forwardDirection * 0.8f;
            movementVector = Vector3.up + forwardDirection;
            movementVector = movementVector.normalized;

            if (base.characterMotor && base.characterDirection) 
            {
                base.characterMotor.velocity = this.movementVector * this.rollSpeed;
            }

            Vector3 b = base.characterMotor ? base.characterMotor.velocity : Vector3.zero;
            this.previousPosition = base.transform.position - b;

            //Attack code.
            SetupBlastAttacks();
            blastTimer = 0f;
            hasFired = false;
            noOfBlasts = 0;
            maxNoOfBlasts = (int)Modules.StaticValues.spinAttackBaseMinorHit * (base.attackSpeedStat >= 1 ? (int)base.attackSpeedStat : 1);
            lengthBetweenTicks = (float)((duration * endBlasting) / maxNoOfBlasts);
            inHitStop = false;
        }

        private void RecalculateSpeed()
        {
            if (stopwatch / duration < 0.35f)
            {
                this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(initialSpeedCoefficient, finalSpeedCoefficient, stopwatch / duration);
            }
            else 
            {
                this.rollSpeed = this.moveSpeedStat * Mathf.SmoothStep(initialSpeedCoefficient, finalSpeedCoefficient, stopwatch / duration);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
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
            if(stopwatch < duration * MajorBlast) 
            {
                RecalculateSpeed();
                if (base.characterDirection) base.characterDirection.forward = this.movementVector;
                if (base.cameraTargetParams) base.cameraTargetParams.fovOverride = Mathf.Lerp(Roll.dodgeFOV, 60f, base.fixedAge / Roll.duration);

                Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
                if (base.characterMotor && base.characterDirection && normalized != Vector3.zero)
                {
                    Vector3 vector = normalized * this.rollSpeed;
                    float d = Mathf.Max(Vector3.Dot(vector, this.movementVector), 0f);
                    vector = this.movementVector * d;

                    base.characterMotor.velocity = vector;
                }
                this.previousPosition = base.transform.position;
            }
            
            // Attack code.
            if (base.isAuthority)
            {
                if (isGrounded) 
                {
                    new ServerForceFallStateNetworkRequest(base.characterBody.masterObjectId).Send(R2API.Networking.NetworkDestination.Clients);
                    this.outer.SetNextStateToMain();
                    return;
                }
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
                baseDamage = Modules.StaticValues.spinAttackMinorBlastDamageCoefficient * base.damageStat,
                procCoefficient = 1.0f,
                baseForce = -1000f,
                radius = Modules.StaticValues.spinAttackMinorRadius,
                crit = base.RollCrit()
            };

            finalBlastAttack = new BlastAttack
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                baseDamage = Modules.StaticValues.spinAttackMajorBlastDamageCoefficientBase * base.damageStat,
                procCoefficient = 1.0f,
                baseForce = 1000f,
                radius = Modules.StaticValues.spinAttackMajorRadius,
                crit = base.RollCrit()
            };
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
