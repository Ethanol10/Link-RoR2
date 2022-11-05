using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace LinkMod.Modules.Networking.OnHitProjectile
{
    internal class SuperBombOnHit : MonoBehaviour, IProjectileImpactBehavior
    {
        public NetworkInstanceId netID;
        public GameObject bodyObj;
        public CharacterBody body;

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            //On hit, we want to spawn x amount more bombs up to 10 based on attackspeed
            Chat.AddMessage($"body: {this.netID}");
            bodyObj = Util.FindNetworkObject(this.netID);
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
                    float z = Modules.StaticValues.superBombSalvoRadius * Mathf.Cos(currentInc);
                    float x = Modules.StaticValues.superBombSalvoRadius * Mathf.Sin(currentInc);
                    float y = Modules.StaticValues.superBombSalvoRadius;

                    Vector3 dir = new Vector3(x, y, z).normalized;
                    FireProjectile(dir, pointOfImpact);
                    currentInc += increment;
                }
            }
        }

        public void FireProjectile(Vector3 direction, Vector3 origin)
        {
            ProjectileManager.instance.FireProjectile(Modules.Projectiles.superBombChildrenPrefab,
                origin,
                Util.QuaternionSafeLookRotation(direction),
                base.gameObject,
                Modules.StaticValues.superBombChildrenBlastDamageCoefficient * body.damage,
                4000f,
                body.RollCrit(),
                DamageColorIndex.Default,
                null,
                Modules.Config.standardBombMaxThrowPower.Value);
        }
    }
}
