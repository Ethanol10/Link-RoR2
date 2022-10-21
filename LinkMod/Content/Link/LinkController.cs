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
        internal bool isShielding;

        //Sword and shield objects
        internal Transform shieldSheathed;
        internal Transform swordSheathed;
        internal Transform shieldUnsheathed;
        internal Transform swordUnsheathed;
        internal Transform runeBombLocation;
        internal Transform runeBombFakeLocation;

        //Bomb Types
        internal enum BombState 
        {
            NOTSPAWNED,
            INHAND,
            THROWN
        }
        internal BombState bombState;
        internal enum BombTypeInHand : uint 
        {
            RUNE, 
            NORMAL, 
            SUPER,
            BOMBCHU
        }
        internal BombTypeInHand bombTypeInHand;

        //Arrow Types
        internal enum ArrowTypeEquipped : uint
        {
            NORMAL = 1,
            FIRE = 2,
            ICE = 3,
            LIGHT = 4,
            ANCIENT = 5,
            BOMB = 6
        };
        internal enum ArrowFireType : uint 
        {
            SINGLE = 1,
            TRIPLE = 2,
            QUINT = 3
        }
        internal ArrowFireType arrowFireType;
        internal ArrowTypeEquipped arrowTypeEquipped;

        internal enum GoddessSpellSelected : uint 
        {
            DIN = 1,
            NAYRU = 2,
            FARORE = 3
        }
        internal GoddessSpellSelected goddessSpellSelected;

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
            runeBombLocation = childLocator.FindChild("runeBombHand");
            runeBombFakeLocation = childLocator.FindChild("runeBombTempHand");

            if (LinkPlugin.emotesAvailable) 
            {
                HookEmoteEvent();
            }

            //Setup initial state for skills when they use it.
            bombState = BombState.NOTSPAWNED;
            arrowFireType = ArrowFireType.SINGLE;
            arrowTypeEquipped = ArrowTypeEquipped.NORMAL;
            goddessSpellSelected = GoddessSpellSelected.DIN;
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

        public void EnableRuneBombInHand() 
        {
            runeBombLocation.gameObject.SetActive(true);
        }

        public void DisableRuneBombInHand()
        {
            runeBombLocation.gameObject.SetActive(false);
        }

        public void EnableFakeRuneBombInHand()
        {
            runeBombFakeLocation.gameObject.SetActive(true);
        }

        public void DisableFakeRuneBombInHand()
        {
            runeBombFakeLocation.gameObject.SetActive(false);
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
