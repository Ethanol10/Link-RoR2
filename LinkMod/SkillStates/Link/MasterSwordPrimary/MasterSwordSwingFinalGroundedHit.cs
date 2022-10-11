using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using static LinkMod.Modules.Projectiles;
using RoR2.Projectile;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordSwingFinalGroundedHit : BaseSkillState
    {
        internal static float baseDuration = 0.9f;
        internal static float hurtBoxFractionStart = 0.1f;
        internal static float hurtboxFractionEnd = 0.25f;
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
            base.PlayAnimation("UpperBody, Override", $"SwordSwing2", "Swing.playbackRate", this.duration);
        }
        
        public void FireBeam() 
        {
            //I suspect this is not working in a non-networked sense. Will need to get the server request up.
            //I dunno might need the network request in the future.
            Modules.Projectiles.swordBeamPrefab.GetComponent<SwordbeamOnHit>().netID = base.characterBody.masterObjectId;
            Ray ray = GetAimRay();
            ProjectileManager.instance.FireProjectile(Modules.Projectiles.swordBeamPrefab,
                ray.origin,
                Util.QuaternionSafeLookRotation(ray.direction),
                base.gameObject,
                Modules.StaticValues.swordBeamDamageCoefficientBase * this.damageStat,
                0f,
                base.RollCrit(),
                DamageColorIndex.Default,
                null,
                Modules.StaticValues.swordBeamForce);
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
                    if (!hasFired) 
                    {
                        FireBeam();
                    }
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
                procCoefficient = 1f,
                forceVector = Vector3.up,
                pushAwayForce = 300f,
                hitBoxGroup = hitBoxGroup,
                isCrit = base.RollCrit(),

                //swingSoundString = 
                //impactSound =
                //hitEffectPrefab = 
            };
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
