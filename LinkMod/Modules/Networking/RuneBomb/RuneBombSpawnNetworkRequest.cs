using LinkMod.SkillStates.Link.HylianShield;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.Modules.Networking.Miscellaneous
{
    internal class RuneBombSpawnNetworkRequest : INetMessage
    {
        NetworkInstanceId netID;
        Vector3 throwDirection;
        float throwForce;

        public RuneBombSpawnNetworkRequest()
        {

        }

        public RuneBombSpawnNetworkRequest(NetworkInstanceId netID, Vector3 throwDirection, float throwForce)
        {
            this.netID = netID;
            this.throwDirection = throwDirection;
            this.throwForce = throwForce;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
            throwDirection = reader.ReadVector3();
            throwForce = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
            writer.Write(throwDirection);
            writer.Write(throwForce);
        }

        public void OnReceived()
        {
            //Master Summon.
            if (NetworkServer.active) 
            {
                MasterSummonForBombBody();
                ApplyForceToBomb();
            }
        }

        public void MasterSummonForBombBody() 
        {
            
        }

        public void ApplyForceToBomb() 
        {
        
        }
    }
}
