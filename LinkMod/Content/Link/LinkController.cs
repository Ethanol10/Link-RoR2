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

        public void Awake()
        {
            Hook();

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
            //Get The modelloc for the hookEmoteEvent
            body = gameObject.GetComponent<CharacterBody>();

            if (LinkPlugin.emotesAvailable) 
            {
                smrs = gameObject.GetComponent<ModelLocator>().modelTransform.GetComponentsInChildren<SkinnedMeshRenderer>();
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
            }
        }
    }
}
