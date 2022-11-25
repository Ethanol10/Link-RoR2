using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EmotesAPI;
using RoR2;
using static LinkMod.Content.Link.LinkController;

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
        internal bool isHolding;
        internal bool isCharging;
        internal bool isCharged;

        //Sword and shield objects
        internal Transform shieldSheathed;
        internal Transform swordSheathed;
        internal Transform shieldUnsheathed;
        internal Transform swordUnsheathed;
        internal Transform runeBombLocation;
        internal Transform runeBombFakeLocation;
        internal Transform bombThrowPosition;
        internal Transform standardBombLocation;
        internal Transform standardBombFakeLocation;
        internal Transform superBombLocation;
        internal Transform superBombFakeLocation;


        //Loadout stuff
        internal enum SelectedLoadout : uint
        {
            SWORD = 1,
            ARROW = 2,
            BOMB = 3,
            MISC = 4
        }
        internal SelectedLoadout selectedLoadout;

        internal List<List<float>> cooldown;
        internal List<List<int>> stock;

        //Bomb Types
        internal bool runeBombThrown;

        internal enum ItemInHand : uint 
        {
            RUNE, 
            NORMAL, 
            SUPER,
            BOMBCHU
        }
        internal ItemInHand itemInHand;

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
            bombThrowPosition = childLocator.FindChild("ThrowPosition");
            standardBombLocation = childLocator.FindChild("standardBomb");
            standardBombFakeLocation = childLocator.FindChild("standardBombTemp");
            superBombLocation = childLocator.FindChild("superBomb");
            superBombFakeLocation = childLocator.FindChild("superBombTemp");

            if (LinkPlugin.emotesAvailable) 
            {
                HookEmoteEvent();
            }

            //Setup initial state for skills when they use it.
            goddessSpellSelected = GoddessSpellSelected.DIN;
            selectedLoadout = SelectedLoadout.SWORD;
            itemInHand = ItemInHand.RUNE;
            runeBombThrown = false;

            //Charging Variables
            isCharging = false;
            isCharged = false;
            isHolding = false;

            //List of Lists
            cooldown = new List<List<float>>();
            for (int i = 0; i < 4; i++) 
            {
                cooldown.Add(new List<float>());
            }
            foreach (List<float> list in cooldown) 
            {
                for (int i = 0; i < 4; i++) 
                {
                    list.Add(0.0f);
                }
            }

            stock = new List<List<int>>();
            for (int i = 0; i < 4; i++)
            {
                stock.Add(new List<int>());
            }
            foreach (List<int> list in stock)
            {
                for (int i = 0; i < 4; i++)
                {
                    list.Add(1);
                }
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

        public void EnableStandardBombInHand()
        {
            standardBombLocation.gameObject.SetActive(true);
        }

        public void DisableStandardBombInHand()
        {
            standardBombLocation.gameObject.SetActive(false);
        }

        public void EnableFakeStandardBombInHand()
        {
            standardBombFakeLocation.gameObject.SetActive(true);
        }

        public void DisableFakeStandardBombInHand()
        {
            standardBombFakeLocation.gameObject.SetActive(false);
        }

        public void EnableSuperBombInHand()
        {
            superBombLocation.gameObject.SetActive(true);
        }

        public void DisableSuperBombInHand()
        {
            superBombLocation.gameObject.SetActive(false);
        }

        public void EnableFakeSuperBombInHand()
        {
            superBombFakeLocation.gameObject.SetActive(true);
        }

        public void DisableFakeSuperBombInHand()
        {
            superBombFakeLocation.gameObject.SetActive(false);
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
