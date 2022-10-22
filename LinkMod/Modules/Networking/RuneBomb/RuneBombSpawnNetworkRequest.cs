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
        Vector3 throwPos;
        Vector3 throwDirection;
        float throwForce;

        GameObject playerObj;

        public RuneBombSpawnNetworkRequest()
        {

        }

        public RuneBombSpawnNetworkRequest(NetworkInstanceId netID, Vector3 throwPos, Vector3 throwDirection, float throwForce)
        {
            this.netID = netID;
            this.throwPos = throwPos;
            this.throwDirection = throwDirection;
            this.throwForce = throwForce;
        }

        public void Deserialize(NetworkReader reader)
        {
            netID = reader.ReadNetworkId();
            throwPos = reader.ReadVector3();
            throwDirection = reader.ReadVector3();
            throwForce = reader.ReadSingle();
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netID);
            writer.Write(throwPos);
            writer.Write(throwDirection);
            writer.Write(throwForce);
        }

        public void OnReceived()
        {
            //Master Summon.
            if (NetworkServer.active) 
            {
                CharacterMaster master;
                master = MasterSummonForBombBody();
                ApplyForceToBomb(master);
            }
        }

        public CharacterMaster MasterSummonForBombBody() 
        {
            playerObj = Util.FindNetworkObject(netID);
            CharacterMaster playerMaster = playerObj.GetComponent<CharacterMaster>();
            CharacterBody body = playerMaster.GetBody();

            GameObject masterObj = MasterCatalog.FindMasterPrefab("RuneBombMonsterMaster");
            CharacterMaster master;

            MasterSummon minionSummon = new MasterSummon();
            minionSummon.masterPrefab = masterObj;
            minionSummon.ignoreTeamMemberLimit = true;
            minionSummon.teamIndexOverride = TeamIndex.Neutral;
            minionSummon.summonerBodyObject = playerObj;
            minionSummon.position = throwPos;
            minionSummon.rotation = Quaternion.LookRotation(throwDirection);

            master = minionSummon.Perform();

            //destroy old instance under the same username.
            if (LinkPlugin.summonCharacterMaster.ContainsKey(netID.Value.ToString()))
            {
                DestroyOldBomb(netID);
            }
            LinkPlugin.summonCharacterMaster.Add(netID.Value.ToString(), master);

            return master;
        }

        public void Explode()
        {
            CharacterBody body = playerObj.GetComponent<CharacterBody>();
            CharacterMaster master = LinkPlugin.summonCharacterMaster[netID.ToString()];
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
            //Play the effect
        }

        public void ApplyForceToBomb(CharacterMaster master) 
        {
            GameObject bodyObj = master.GetBodyObject();
            CharacterBody body = bodyObj.GetComponent<CharacterBody>();
            HealthComponent health = body.healthComponent;

            DamageInfo damageInfo = new DamageInfo
            {
                damage = 0f,
                crit = false,
                attacker = playerObj,
                inflictor = playerObj,
                position = bodyObj.transform.position,
                canRejectForce = false,
                force = throwDirection.normalized * throwForce,
                rejected = false
            };
            health.TakeDamageForce(damageInfo);     
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
