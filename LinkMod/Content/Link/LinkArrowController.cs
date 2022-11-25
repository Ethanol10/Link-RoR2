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
            if (icon.targetSkill?.characterBody.baseNameToken == LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_NAME") 
            {
                switch (icon.targetSkillSlot) 
                {
                    case RoR2.SkillSlot.Secondary:
                        //Arrow Type
                        break;
                    case RoR2.SkillSlot.Utility:
                        //Arrow Firing Type
                        break;
                }
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
            //Check if we are in arrow loadout.
            if (linkController?.selectedLoadout == LinkController.SelectedLoadout.ARROW)
            {
                //Check if the labels are created
                if (!labelsCreated) 
                {
                    //Create labels
                    SetupArrowLabels(self);
                    labelsCreated = true;
                }

                //Reenable the labels.
                ArrowFireTypeLabel.enabled = true;
                ArrowTypeLabel.enabled = true;

                //Check the type and update accordingly.

                
            }
            //Disable the labels.
            else 
            {
                ArrowFireTypeLabel.enabled = false;
                ArrowTypeLabel.enabled = false;
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
