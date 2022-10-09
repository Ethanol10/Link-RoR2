using EntityStates;
using LinkMod.Content.Link;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link
{
    internal class LinkSpawnState : BaseSkillState
    {
        public float spawnDuration = 4.0f;
        public float unsheatheTime = 0.63f;
        public bool unsheathed;
        internal LinkController linkController;

        //Play the animation.
        public override void OnEnter()
        {
            base.OnEnter();
            unsheathed = false;
            ChildLocator childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            linkController = this.gameObject.GetComponent<LinkController>();
            linkController.SetSheathed();

            //Get the parasail spawn
            Transform parasailObj = childLocator.FindChild("ParasailSpawn");
            Animator parasailAnimator = parasailObj.GetComponent<Animator>();

            base.GetModelAnimator().SetBool("isSpawn", true);
            parasailAnimator.SetBool("enableSpawn", true);
            
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge > spawnDuration * unsheatheTime && !unsheathed) 
            {
                linkController.SetUnsheathed();
                unsheathed = true;
            }
            if (base.fixedAge > spawnDuration) 
            {
                base.outer.SetNextStateToMain();
            }
        }
    }
}
