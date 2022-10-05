using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinkMod.SkillStates.Link
{
    internal class LinkSpawnState : BaseSkillState
    {
        public float spawnDuration = 4.0f;

        //Play the animation.
        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Body", "SpawnAnim", "Spawn.playbackRate", spawnDuration);

        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > spawnDuration) 
            {
                base.outer.SetNextStateToMain();
            }
        }
    }
}
