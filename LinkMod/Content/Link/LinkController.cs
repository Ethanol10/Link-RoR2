using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EmotesAPI;
using RoR2;

namespace LinkMod.Content.Link
{
    internal class LinkController : MonoBehaviour
    {
        internal SkinnedMeshRenderer[] smrs;
        internal CharacterBody body;
        internal Animator anim;
        internal ChildLocator childLocator;
        internal Animator parasailAnimator;

        //Sword and shield objects
        internal Transform shieldSheathed;
        internal Transform swordSheathed;
        internal Transform shieldUnsheathed;
        internal Transform swordUnsheathed;

        public void Awake()
        {
            Hook();
            //Get The modelloc for the hookEmoteEvent
            body = gameObject.GetComponent<CharacterBody>();
            childLocator = GetComponentInChildren<ChildLocator>();

            shieldSheathed = childLocator.FindChild("SheathedShieldObj");
            swordSheathed = childLocator.FindChild("SheathedSwordObj");
            shieldUnsheathed = childLocator.FindChild("ShieldObj");
            swordUnsheathed = childLocator.FindChild("SwordObj");

            if (LinkPlugin.emotesAvailable) 
            {
                HookEmoteEvent();
            }
        }

        public void Hook() 
        {
            
        }

        public void HookEmoteEvent() 
        {
            EmotesAPI.CustomEmotesAPI.animChanged += HandleCustomEmotesAPIAnimationEnd;
        }

        public void UnHookEmoteEvent() 
        {
            EmotesAPI.CustomEmotesAPI.animChanged -= HandleCustomEmotesAPIAnimationEnd;
        }

        public void Start()
        {

            if (LinkPlugin.emotesAvailable) 
            {
                smrs = gameObject.GetComponent<ModelLocator>().modelTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
                anim = body.hurtBoxGroup.gameObject.GetComponent<Animator>();
                parasailAnimator = childLocator.FindChild("ParasailSpawn").GetComponent<Animator>();
            }
        }

        public void FixedUpdate()
        {

        }

        public void Update()
        {

        }

        public void OnDestroy()
        {
            if (LinkPlugin.emotesAvailable) 
            {
                UnHookEmoteEvent();
            }
        }

        public void SetSheathed() 
        {
            swordSheathed.gameObject.SetActive(true);
            shieldSheathed.gameObject.SetActive(true);
            swordUnsheathed.gameObject.SetActive(false);
            shieldUnsheathed.gameObject.SetActive(false);
        }

        public void SetUnsheathed()
        {
            swordSheathed.gameObject.SetActive(false);
            shieldSheathed.gameObject.SetActive(false);
            swordUnsheathed.gameObject.SetActive(true);
            shieldUnsheathed.gameObject.SetActive(true);
        }

        public void SetSwordOnlyUnsheathed() 
        {
            swordSheathed.gameObject.SetActive(false);
            shieldSheathed.gameObject.SetActive(true);
            swordUnsheathed.gameObject.SetActive(true);
            shieldUnsheathed.gameObject.SetActive(false);
        }

        public void HandleCustomEmotesAPIAnimationEnd(string newAnim, BoneMapper mapper) 
        {
            if (newAnim == "none")
            {
                foreach (SkinnedMeshRenderer smr in smrs)
                {
                    if (smr.transform.parent.gameObject.name == "mdlLink")
                    {
                        smr.transform.parent.gameObject.SetActive(false);
                        smr.transform.parent.gameObject.SetActive(true);
                    }
                }
                anim.SetBool("isIdle", true);
                parasailAnimator.SetBool("enableSpawn", false);
                SetUnsheathed();
            }
            else 
            {
                SetSheathed();
            }
        }
    }
}
