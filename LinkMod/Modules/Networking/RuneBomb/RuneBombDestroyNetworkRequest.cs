using LinkMod.Content.Link;
using LinkMod.SkillStates.Link.HylianShield;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.Modules.Networking.Miscellaneous
{
    internal class RuneBombDestroyNetworkRequest : INetMessage
    {
        NetworkInstanceId netID;

        GameObject playerObj;
        CharacterMaster playerMaster;
        CharacterBody body;

        public RuneBombDestroyNetworkRequest()
        {

        }

        public RuneBombDestroyNetworkRequest(NetworkInstanceId netID)
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
            playerObj = Util.FindNetworkObject(netID);
            playerMaster = playerObj.GetComponent<CharacterMaster>();
            body = playerMaster.GetBody();
            
            //Master Summon.
            if (NetworkServer.active) 
            {
                Explode();
                DestroyOldBomb(netID);
            }
            if (body.hasEffectiveAuthority)
            {
                ResetOriginalLinkRuneBombSkill();
            }
        }

        public void ResetOriginalLinkRuneBombSkill() 
        {
            //Notify the Original link that his bomb is gone. Set back detonate skills to spawn.
            LinkController linkController = body.GetComponent<LinkController>();

            linkController.runeBombThrown = false;

            if (linkController.selectedLoadout == LinkController.SelectedLoadout.BOMB)
            {
                SkillDef primarySkill = body.skillLocator.primary.skillDef;
                body.skillLocator.primary.UnsetSkillOverride(body.skillLocator.primary, primarySkill, GenericSkill.SkillOverridePriority.Contextual);
                body.skillLocator.primary.SetSkillOverride(body.skillLocator.primary, Link.runeBombSpawn, GenericSkill.SkillOverridePriority.Contextual);
            }
        }

        public void Explode()
        {
            if (LinkPlugin.summonCharacterMaster.ContainsKey(netID.ToString())) 
            {
                CharacterMaster master = LinkPlugin.summonCharacterMaster[netID.ToString()];
                if (master)
                {
                    CharacterBody bombBody = master.GetBody();

                    BlastAttack blastAttack = new BlastAttack
                    {
                        attacker = playerObj,
                        baseDamage = Modules.StaticValues.runeBombBlastDamageCoefficient * body.damage,
                        radius = Modules.StaticValues.runeBombRadius,
                        position = bombBody.transform.position,
                        falloffModel = BlastAttack.FalloffModel.None,
                        crit = body.RollCrit(),
                        teamIndex = TeamIndex.Player,
                        damageType = DamageType.Generic,
                        baseForce = Modules.StaticValues.runeBombBlastForce,
                        bonusForce = Vector3.up
                    };

                    blastAttack.Fire();
                }
                //Play the effect
            }
        }

        //Destroy the clone, using the username as the key.
        public void DestroyOldBomb(NetworkInstanceId instance)
        {
            //Check if the key exists to remove.
            if (LinkPlugin.summonCharacterMaster.ContainsKey(instance.Value.ToString()))
            {
                //Check if the master still exists.
                if (LinkPlugin.summonCharacterMaster[instance.Value.ToString()])
                {
                    //Kill Clone, Destroy it on the server, then remove the key from the server Dictionary.
                    if (LinkPlugin.summonCharacterMaster[instance.Value.ToString()].GetBodyObject())
                    {
                        LinkPlugin.summonCharacterMaster[instance.Value.ToString()].TrueKill();
                        Explode();
                    }
                    if (LinkPlugin.summonCharacterMaster[instance.Value.ToString()].gameObject)
                    {
                        NetworkServer.Destroy(LinkPlugin.summonCharacterMaster[instance.Value.ToString()].gameObject);
                    }
                }

                LinkPlugin.summonCharacterMaster.Remove(instance.Value.ToString());
            }
        }
    }
}
