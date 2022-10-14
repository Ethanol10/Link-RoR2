using LinkMod.SkillStates.Link.HylianShield;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.Modules.Networking.Miscellaneous
{
    internal class ServerForceShieldBlockSuccessAnimNetworkRequest : INetMessage
    {
        NetworkInstanceId netID;

        public ServerForceShieldBlockSuccessAnimNetworkRequest()
        {

        }

        public ServerForceShieldBlockSuccessAnimNetworkRequest(NetworkInstanceId netID)
        {
            this.netID = netID;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
        }

        public void OnReceived()
        {
            ForceFallState();
        }

        //Lots of shit checks in here.
        public void ForceFallState()
        {
            GameObject masterobject = Util.FindNetworkObject(netID);

            if (!masterobject)
            {
                Debug.Log("Specified GameObject not found! Fix your shit Ethanol 10.");
                return;
            }
            CharacterMaster charMaster = masterobject.GetComponent<CharacterMaster>();
            if (!charMaster)
            {
                Debug.Log("charMaster failed to locate");
                return;
            }

            if (!charMaster.hasEffectiveAuthority)
            {
                return;
            }

            GameObject bodyObject = charMaster.GetBodyObject();

            EntityStateMachine[] stateMachines = bodyObject.GetComponents<EntityStateMachine>();
            //"No statemachines?"
            if (!stateMachines[0])
            {
                Debug.LogWarning("StateMachine search failed! Wrong object?");
                return;
            }

            foreach (EntityStateMachine stateMachine in stateMachines)
            {
                if (stateMachine.customName == "Slide")
                {
                    stateMachine.SetState(new HylianShieldBlockSuccessful());
                    return;
                }
            }
        }
    }
}
