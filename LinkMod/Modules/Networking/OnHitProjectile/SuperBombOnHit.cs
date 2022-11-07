using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace LinkMod.Modules.Networking.OnHitProjectile
{
    internal class SuperBombOnHit : MonoBehaviour, IProjectileImpactBehavior
    {
        public GameObject bodyObj;
        public CharacterBody body;

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            //On hit, we want to spawn x amount more bombs up to 10 based on attackspeed
            bodyObj = gameObject.GetComponent<ProjectileController>().owner;
            body = bodyObj.GetComponent<CharacterBody>();

            if (body && body.hasEffectiveAuthority)
            {
                int childAmount = Modules.StaticValues.superBombMinAmountOfChildren > Modules.Config.superBombChildrenMaxAmount.Value / 2 ?
                                    Modules.Config.superBombChildrenMaxAmount.Value / 2 : Modules.StaticValues.superBombMinAmountOfChildren;
                //Check attack speed
                //if less than the minimum, spawn that hardcoded amount, otherwise spawn on a 1:1 basis
                if (childAmount < (int)body.attackSpeed)
                {
                    childAmount = Mathf.Clamp((int)body.attackSpeed, Modules.StaticValues.superBombMinAmountOfChildren, Modules.Config.superBombChildrenMaxAmount.Value);
                }

                Vector3 pointOfImpact = impactInfo.estimatedPointOfImpact;
                float increment = (Mathf.PI * 2f) / (float)childAmount;
                float currentInc = 0f;

                //Determine the angle at which it should shoot each child projectile.
                for (int i = 0; i < childAmount; i++)
                {
                    //we want to aim for a radius around the bomb.
                    // x = rcos(theta)
                    // y = rsin(theta)
                    float x = Modules.StaticValues.superBombSalvoRadius * Mathf.Cos(currentInc);
                    float z = Modules.StaticValues.superBombSalvoRadius * Mathf.Sin(currentInc);
                    float y = Modules.StaticValues.superBombSalvoRadius;

                    Vector3 dir = new Vector3(x, y, z).normalized;
                    FireProjectile(dir, new Vector3(pointOfImpact.x + x/8.0f, pointOfImpact.y, pointOfImpact.z + z/8.0f));
                    currentInc += increment;
                }
            }
        }

        public void FireProjectile(Vector3 direction, Vector3 origin)
        {
            ProjectileManager.instance.FireProjectile(Modules.Projectiles.superBombChildrenPrefab,
                new Vector3(origin.x, origin.y + 2f, origin.z),
                Util.QuaternionSafeLookRotation(direction),
                bodyObj,
                Modules.StaticValues.superBombChildrenBlastDamageCoefficient * body.damage,
                4000f,
                body.RollCrit(),
                DamageColorIndex.Default,
                null,
                Modules.Config.standardBombMaxThrowPower.Value);
        }
    }
}
