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