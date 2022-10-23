using LinkMod.Modules.Networking.Miscellaneous;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.Content.Link
{
    internal class RuneBombController : MonoBehaviour
    {
        public GameObject bomb;
        public NetworkInstanceId ownerNetID;
        public CharacterBody body;
        public CharacterMaster master;
        public float stopwatch;
        public bool isDead;

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

            foreach (BaseAI obj in master.aiComponents)
            {
                UnityEngine.Object.Destroy(obj);
            }

        }

        public void Update() 
        {
            stopwatch += Time.deltaTime;
            if (stopwatch >= Modules.Config.runeBombSelfDestructTimer.Value && !isDead) 
            {
                isDead = true;
                if (NetworkServer.active) 
                {
                    new RuneBombDestroyNetworkRequest(ownerNetID).Send(R2API.Networking.NetworkDestination.Clients);
                }
            }
        }

        public void OnDestroy() 
        {
            if (!isDead) 
            {
                new RuneBombDestroyNetworkRequest(ownerNetID).Send(R2API.Networking.NetworkDestination.Clients);
            }
        }
    }
}
