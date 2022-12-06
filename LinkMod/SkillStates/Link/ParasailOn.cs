using EntityStates;
using LinkMod.Content.Link;
using RoR2;
using UnityEngine;

namespace LinkMod.SkillStates.Link
{
    public class ParasailOn : BaseState
    {
        private Transform parasailTransform;
        private Animator parasailAnim;
        private Animator linkAnim;
        private LinkController linkCon;
        private static float slowestDescent = -3f;

        public override void OnEnter()
        {
            base.OnEnter();
            linkCon = gameObject.GetComponent<LinkController>();
            linkCon.SetSheathed();
            this.parasailTransform = base.FindModelChild("ParasailObj");
            parasailAnim = parasailTransform.GetComponent<Animator>();
            linkAnim = base.GetModelAnimator();
            parasailAnim.SetBool("isOpen", true);
            linkAnim.SetBool("isGliding", true);
            base.PlayCrossfade("FullBody, Override", "Gliding", 0.15f);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                float newFallingVelocity = base.characterMotor.velocity.y / 1.5f;
                if (newFallingVelocity > slowestDescent) 
                {
                    newFallingVelocity = slowestDescent;
                }
                newFallingVelocity = Mathf.MoveTowards(newFallingVelocity, Modules.Config.glideSpeed.Value, Modules.Config.glideAcceleration.Value * Time.fixedDeltaTime);
                base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, newFallingVelocity, base.characterMotor.velocity.z);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            parasailAnim.SetBool("isOpen", false);
            linkAnim.SetBool("isGliding", false);
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            linkCon.SetUnsheathed();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
