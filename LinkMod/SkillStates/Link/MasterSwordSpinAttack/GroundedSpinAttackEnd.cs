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

        public override void OnEnter()
        {
            base.OnEnter();
            stopwatch = 0f;
            duration = baseDuration;
            excessAmount = 0f;
            lengthBetweenTicks = (float)(baseDuration / Modules.StaticValues.spinAttackBaseMinorHit);

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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority) 
            {
                
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
                baseDamage = Modules.StaticValues.spinAttackMinorBlastDamageCoefficient * base.damageStat * (boostedDamage / 10.0f),
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
