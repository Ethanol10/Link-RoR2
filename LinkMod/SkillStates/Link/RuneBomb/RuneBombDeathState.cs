using System;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.SkillStates.Link.RuneBomb
{
    public class RuneBombDeathState : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            //Spawn explosion effect
            EffectData effect = new EffectData
            {
                rotation = Quaternion.identity,
                origin = this.gameObject.transform.position,
                scale = 1f,
                color = Color.white,
            };
            EffectManager.SpawnEffect(Modules.Assets.runeBombExplosion, effect, true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority) 
            {
                EntityState.Destroy(base.gameObject);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}