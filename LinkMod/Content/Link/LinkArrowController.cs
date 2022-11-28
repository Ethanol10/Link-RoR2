using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace LinkMod.Content.Link
{
    //Controls the label and the state of the arrows that have been selected.
    internal class LinkArrowController : MonoBehaviour
    {
        internal LinkController linkController;
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
        internal HGTextMeshProUGUI ArrowFireTypeLabel;
        internal HGTextMeshProUGUI ArrowTypeLabel;
        internal bool labelsCreated;

        public void Start() 
        {
            //Get Controller to determine if we should even show the label
            linkController = gameObject.GetComponent<LinkController>();

            //Setup initial state for skills when they use it.
            arrowFireType = ArrowFireType.SINGLE;
            arrowTypeEquipped = ArrowTypeEquipped.NORMAL;
            labelsCreated = false;

            Hook();
        }

        private void Hook() 
        {
            On.RoR2.UI.SkillIcon.Update += SkillIcon_Update;
        }

        private void Unhook() 
        {
            On.RoR2.UI.SkillIcon.Update -= SkillIcon_Update;
        }

        public void SetupArrowLabels(RoR2.UI.SkillIcon icon) 
        {
            Vector3 position;
            position = LinkPlugin.riskUIEnabled ? icon.stockText.transform.position : icon.stockText.transform.parent.parent.position;
            Transform childTransform = LinkPlugin.riskUIEnabled ? icon.stockText.transform.parent : icon.stockText.transform.parent.parent.GetChild(1);
            //Alignment only for riskUI
            if (LinkPlugin.riskUIEnabled)
            {
                HGTextMeshProUGUI KeyUI = icon.stockText.transform.parent.parent.GetChild(1).GetComponentInChildren<HGTextMeshProUGUI>();
                KeyUI.alignment = TextAlignmentOptions.Center;
            }

            switch (icon.targetSkillSlot)
            {
                case RoR2.SkillSlot.Secondary:
                    if (!ArrowFireTypeLabel)
                    {
                        //Arrow Firing Type 
                        this.ArrowFireTypeLabel = this.CreateLabel(childTransform, "ArrowFireTypeLabel", "Single", new Vector2(position.x - 7.5f, position.y + 20f), 15f);
                        this.ArrowFireTypeLabel.transform.SetSiblingIndex(0);
                        this.ArrowFireTypeLabel.transform.rotation = icon.stockText.transform.rotation;
                        this.ArrowFireTypeLabel.color = icon.stockText.color;
                    }
                    break;
                case RoR2.SkillSlot.Utility:
                    if (!ArrowTypeLabel)
                    {
                        //Arrow Type 
                        this.ArrowTypeLabel = this.CreateLabel(childTransform, "ArrowTypeLabel", "Normal", new Vector2(position.x - 7.5f, position.y + 20f), 15f);
                        this.ArrowTypeLabel.transform.SetSiblingIndex(0);
                        this.ArrowTypeLabel.transform.rotation = icon.stockText.transform.rotation;
                        this.ArrowTypeLabel.color = icon.stockText.color;
                    }
                    break;
            }

        }

        public void SetArrowFireType(int arrowTypeIndex) 
        {
            switch (arrowTypeIndex) 
            {
                case 1:
                    arrowFireType = ArrowFireType.SINGLE;
                    break;
                case 2:
                    arrowFireType = ArrowFireType.TRIPLE;
                    break;
                case 3:
                    arrowFireType = ArrowFireType.QUINT;
                    break;
                default:
                    arrowFireType = ArrowFireType.SINGLE;
                    break;
            }
        }


        public void SetArrowEquippedType(int arrowTypeIndex)
        {
            switch (arrowTypeIndex)
            {
                case 1:
                    arrowTypeEquipped = ArrowTypeEquipped.NORMAL;
                    break;
                case 2:
                    arrowTypeEquipped = ArrowTypeEquipped.FIRE;
                    break;
                case 3:
                    arrowTypeEquipped = ArrowTypeEquipped.ICE;
                    break;
                case 4:
                    arrowTypeEquipped = ArrowTypeEquipped.LIGHT;
                    break;
                case 5:
                    arrowTypeEquipped = ArrowTypeEquipped.ANCIENT;
                    break;
                case 6:
                    arrowTypeEquipped = ArrowTypeEquipped.BOMB;
                    break;
                default:
                    arrowTypeEquipped = ArrowTypeEquipped.NORMAL;
                    break;
            }
        }

        //Creates the label.
        private HGTextMeshProUGUI CreateLabel(Transform parent, string name, string text, Vector2 position, float textScale)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.parent = parent;
            gameObject.AddComponent<CanvasRenderer>();
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            HGTextMeshProUGUI hgtextMeshProUGUI = gameObject.AddComponent<HGTextMeshProUGUI>();
            hgtextMeshProUGUI.text = text;
            hgtextMeshProUGUI.fontSize = textScale;
            hgtextMeshProUGUI.color = Color.white;
            hgtextMeshProUGUI.alignment = TextAlignmentOptions.Center;
            hgtextMeshProUGUI.enableWordWrapping = false;
            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = position;
            return hgtextMeshProUGUI;
        }

        public void SkillIcon_Update(On.RoR2.UI.SkillIcon.orig_Update orig, RoR2.UI.SkillIcon self)
        {
            orig.Invoke(self);

            //check if we are link first

            if (self.targetSkill?.characterBody.baseNameToken == LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_NAME") 
            {
                //Check if we are in arrow loadout.
                if (linkController?.selectedLoadout == LinkController.SelectedLoadout.ARROW)
                {
                    //Check if the labels are created
                    if (!labelsCreated)
                    {
                        //Create labels
                        SetupArrowLabels(self);
                        if (ArrowFireTypeLabel && ArrowTypeLabel)
                        {
                            labelsCreated = true;
                        }
                    }
                    else
                    {
                        //Reenable the labels.
                        if (self.targetSkillSlot == RoR2.SkillSlot.Secondary) ArrowFireTypeLabel.enabled = true;
                        if (self.targetSkillSlot == RoR2.SkillSlot.Utility) ArrowTypeLabel.enabled = true;

                        string textToSet = "";
                        //Check the type and update accordingly.
                        switch (self.targetSkillSlot)
                        {
                            case RoR2.SkillSlot.Secondary:
                                //Arrow Firing Type
                                switch (arrowFireType)
                                {
                                    case ArrowFireType.SINGLE:
                                        textToSet = "Single";
                                        break;
                                    case ArrowFireType.TRIPLE:
                                        textToSet = "Triple";
                                        break;
                                    case ArrowFireType.QUINT:
                                        textToSet = "Quintuple";
                                        break;
                                }
                                ArrowFireTypeLabel.SetText(textToSet);
                                break;
                            case RoR2.SkillSlot.Utility:
                                //Arrow Type

                                switch (arrowTypeEquipped)
                                {
                                    case ArrowTypeEquipped.NORMAL:
                                        textToSet = "Normal";
                                        break;
                                    case ArrowTypeEquipped.FIRE:
                                        textToSet = "Fire";
                                        break;
                                    case ArrowTypeEquipped.ICE:
                                        textToSet = "Ice";
                                        break;
                                    case ArrowTypeEquipped.LIGHT:
                                        textToSet = "Light";
                                        break;
                                    case ArrowTypeEquipped.ANCIENT:
                                        textToSet = "Ancient";
                                        break;
                                    case ArrowTypeEquipped.BOMB:
                                        textToSet = "Bomb";
                                        break;
                                }
                                ArrowTypeLabel.SetText(textToSet);
                                break;
                        }
                    }
                }
                //Disable the labels.
                else
                {
                    ArrowFireTypeLabel.enabled = false;
                    ArrowTypeLabel.enabled = false;
                }
            }
    
        }

        public void FixedUpdate() 
        {
            
        }

        public void Destroy() 
        {
            Destroy(ArrowFireTypeLabel);
            Destroy(ArrowTypeLabel);
            Unhook();
        }
    }
}
