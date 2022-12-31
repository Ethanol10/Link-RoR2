using LinkMod.Modules.Characters;
using RoR2;
using RoR2.Projectile;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace LinkMod.Modules.Networking.OnHitProjectile
{
    public class ArrowOnHit : MonoBehaviour, IProjectileImpactBehavior
    {
        public bool didHit = false;
        public float arrowLifetime = 10f;
        public float stopwatch;
        
        void IProjectileImpactBehavior.OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (impactInfo.collider)
            {
                //check collider

                GameObject enemyObj = impactInfo.collider.gameObject;
                CharacterBody enemyBody = enemyObj.GetComponent<CharacterBody>();

                ProjectileController projCon = gameObject.GetComponent<ProjectileController>();

                if (enemyBody)
                {
                    bool teamValid = enemyObj.GetComponent<TeamComponent>().teamIndex != projCon.owner.GetComponent<TeamComponent>().teamIndex;

                    if (NetworkServer.active && teamValid)
                    {
                        ProjectileDamage projDmg = gameObject.GetComponent<ProjectileDamage>();

                        DamageInfo info = new DamageInfo
                        {
                            damage = projDmg.damage,
                            crit = projDmg.crit,
                            attacker = projCon.owner,
                            inflictor = gameObject,
                            position = impactInfo.estimatedPointOfImpact,
                            force = new Vector3(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z) * projDmg.force,
                            rejected = false,
                            damageType = DamageType.Generic,
                            canRejectForce = false
                        };

                        enemyBody.healthComponent.TakeDamage(info);
                        didHit = true;

                    }
                }
                else 
                {
                    //When something doesn't have a body, it's probably a wall or something else.
                    //Handle Impale 
                }
            }
        }

        public void Start() 
        {
            stopwatch = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (didHit)
            {
                //Mark for Destruction

                stopwatch += Time.deltaTime;
            }
        }

        public void OnDestroy() 
        {
            //nothing yet.
        }
    }
}