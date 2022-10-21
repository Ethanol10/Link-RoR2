using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LinkMod.Content.Link
{
    internal class RuneBombController : MonoBehaviour
    {
        public GameObject bomb;
        public CharacterBody body;
        public CharacterMaster master;
        public float stopwatch;

        public void Awake() 
        {
            bomb = this.gameObject;
        }

        public void Start() 
        {
            stopwatch = 0f;
            body = gameObject.GetComponent<CharacterBody>();
            master = body.master;
            master.teamIndex = TeamIndex.Neutral;
            Destroy(master.GetComponent<BaseAI>());

        }

        public void Update() 
        {
            stopwatch += Time.deltaTime;
            if (stopwatch >= Modules.Config.runeBombSelfDestructTimer.Value) 
            {
                master.TrueKill();
            }
        }
    }
}
