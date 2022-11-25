using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
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
        }
    }
}
