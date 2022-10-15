using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using LinkMod.Modules.Networking.Miscellaneous;
using R2API.Networking.Interfaces;
using static LinkMod.Modules.Projectiles;
using RoR2.Projectile;

namespace LinkMod.SkillStates.Link.MasterSwordPrimary
{
    internal class MasterSwordAerialDoubleSwing : BaseSkillState
    {
        internal static float baseDuration = 1.5f;
        internal static float firstSwingFractionStart = 0.18f;
        internal static float firstSwingFractionEnd = 0.22f;
        internal static float secondSwingFractionStart = 0.33f;
        internal static float secondSwingFractionEnd = 0.4f;
        internal static float earlyExitTime = 0.58f;
        internal float hitHopVelocity = 13f;
        internal float duration;
        internal bool hasFired;
        internal OverlapAttack firstAttack;
        internal OverlapAttack secondAttack;
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
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
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
            base.PlayAnimation("FullBody, Override", $"AerialAttack", "Swing.playbackRate", this.duration);
        }

        public void FireBeam()
        {
            //I dunno might need the network request in the future.
            Modules.Projectiles.swordBeamPrefab.GetComponent<SwordbeamOnHit>().netID = base.characterBody.masterObjectId;
            Ray ray = GetAimRay();

            ProjectileSimple simple = Modules.Projectiles.swordBeamPrefab.GetComponent<ProjectileSimple>();
            simple.desiredForwardSpeed = Modules.StaticValues.swordBeamProjectileSpeed;
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
                    if (base.isGrounded) 
                    {
                        new ServerForceFallStateNetworkRequest(base.characterBody.masterObjectId).Send(R2API.Networking.NetworkDestination.Clients);
                        this.outer.SetNextStateToMain();
                        return;
                    }
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

                //fire first attack
                if (stopwatch >= duration * firstSwingFractionStart
                && stopwatch <= duration * firstSwingFractionEnd)
                {
                    if (this.firstAttack.Fire())
                    {
                        this.OnHitEnemyAuthority();
                    }
                    //fire
                }

                if (stopwatch >= duration * secondSwingFractionStart
                && stopwatch <= duration * secondSwingFractionEnd)
                {
                    if (!hasFired && (healthComponent.health / characterBody.maxHealth >= Modules.StaticValues.healthRequiredToFirePercentage)) 
                    {
                        FireBeam();
                    }
                    hasFired = true;
                    if (this.secondAttack.Fire())
                    {
                        this.OnHitEnemyAuthority();
                    }
                    //fire
                }

                //End move
                if (stopwatch >= duration * earlyExitTime)
                {

                    //Check if we should continue onwards.
                    if (this.inputBank.skill1.down)
                    {
                        //EEEeeeh let the MasterSword class handle it
                        if (!base.isGrounded)
                        {
                            this.outer.SetState(new MasterSword { });
                            return;
                        }

                        this.outer.SetState(new MasterSwordAerialDoubleSwing());
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
                    (HitBoxGroup element) => element.groupName == "AerialSwingHitbox");
            }

            this.firstAttack = new OverlapAttack
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                damage = Modules.StaticValues.msAerialDoubleSwingFirst * this.damageStat,
                procCoefficient = 1f,
                forceVector = Vector3.zero,
                pushAwayForce = 0f,
                hitBoxGroup = hitBoxGroup,
                isCrit = base.RollCrit(),

                //swingSoundString = 
                //impactSound =
                //hitEffectPrefab = 
            };

            this.secondAttack = new OverlapAttack
            {
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                damage = Modules.StaticValues.msAerialDoubleSwingSecond * this.damageStat,
                procCoefficient = 1f,
                forceVector = Vector3.zero,
                pushAwayForce = 0f,
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
