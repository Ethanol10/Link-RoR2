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
        internal static float earlyExitTime = 0.65f;
        internal float duration;
        internal bool hasFired;
        internal OverlapAttack attack;
        internal Animator animator;
        internal HitStopCachedState hitstopCache;
        internal int index;

        public override void OnEnter()
        {
            base.OnEnter();
            hasFired = false;
            this.animator = base.GetModelAnimator();
            duration = baseDuration / this.attackSpeedStat;
            base.StartAimMode(0.5f + this.duration, false);
            SetupOverlapAttack();

            animator.SetFloat("Swing.playbackRate", this.attackSpeedStat); 
            PlayAttackAnimation();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void OnHitEnemyAuthority() 
        {
            
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
                //fire attack
                if (base.fixedAge >= duration * hurtBoxFractionStart
                && base.fixedAge <= duration * hurtboxFractionEnd)
                {
                    hasFired = true;
                    if (this.attack.Fire()) 
                    {
                        this.OnHitEnemyAuthority();
                    }
                    //fire
                }

                //End move
                if (base.fixedAge >= duration * earlyExitTime)
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
                if (base.fixedAge >= duration) 
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
