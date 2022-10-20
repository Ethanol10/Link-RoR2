using System.Collections;
using UnityEngine;

namespace LinkMod.Content.Link
{
    public class LinkRandomIdleAnimController : MonoBehaviour
    {
        public Animator anim;
        // Use this for initialization
        void Start()
        {
            //randomly select the first bool on select.
            anim = GetComponent<Animator>();

            //Generate number check between thirds
            int randomNum = Random.Range(1, 11);

            if (randomNum <= 3) 
            {
                anim.SetBool("trigger1", true);
                anim.SetBool("trigger2", false);
                anim.SetBool("trigger3", false);
            }
            if (randomNum > 3 && randomNum <= 6) 
            {
                anim.SetBool("trigger1", false);
                anim.SetBool("trigger2", true);
                anim.SetBool("trigger3", false);
            }
            if (randomNum > 6 && randomNum <= 10)
            {
                anim.SetBool("trigger1", false);
                anim.SetBool("trigger2", false);
                anim.SetBool("trigger3", true);
            }
        }

        public void rerollAnim() 
        {
            //Generate number check between thirds
            int randomNum = Random.Range(1, 11);
            
            if (randomNum <= 3)
            {
                anim.SetBool("trigger1", true);
                anim.SetBool("trigger2", false);
                anim.SetBool("trigger3", false);
            }
            if (randomNum > 3 && randomNum <= 6)
            {
                anim.SetBool("trigger1", false);
                anim.SetBool("trigger2", true);
                anim.SetBool("trigger3", false);
            }
            if (randomNum > 6 && randomNum <= 10)
            {
                anim.SetBool("trigger1", false);
                anim.SetBool("trigger2", false);
                anim.SetBool("trigger3", true);
            }
        }
    }
}