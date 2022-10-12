using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.SkillStates.Link.MasterSwordSpinAttack
{
    internal class AerialSpinAttack : BaseSkillState
    {
        public static float duration = 1.5f;
        public static float initialSpeedCoefficient = 5f;
        public static float finalSpeedCoefficient = 0f;

        private float rollSpeed;
        private Vector3 upwardDirection;
        private Animator animator;
        private Vector3 previousPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
            animator.SetFloat("Swing.playbackRate", 1.0f);

            base.PlayAnimation("FullBody, Override", "SpinAttackAir", "Swing.playbackRate", duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}
