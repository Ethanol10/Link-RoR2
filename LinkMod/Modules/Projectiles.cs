using LinkMod.Modules.Networking.OnHitProjectile;
using R2API;
using R2API.Networking;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace LinkMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject swordBeamPrefab;
        internal static GameObject standardBombPrefab;
        internal static GameObject superBombPrefab;
        internal static GameObject superBombChildrenPrefab;

        internal static GameObject basicArrowPrefab;

        internal static void RegisterProjectiles()
        {
            CreateSwordBeam();
            CreateStandardBomb();
            CreateSuperBombChildren();
            CreateSuperBomb();
            CreateBasicArrow();

            AddProjectile(swordBeamPrefab);
            AddProjectile(standardBombPrefab);
            AddProjectile(superBombChildrenPrefab);
            AddProjectile(superBombPrefab);
            AddProjectile(basicArrowPrefab);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void CreateBasicArrow() 
        {
            basicArrowPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("LinkArrow");
            basicArrowPrefab.AddComponent<NetworkIdentity>();

            ProjectileController projectileCon = basicArrowPrefab.AddComponent<ProjectileController>();
            projectileCon.procCoefficient = 1f;

            ProjectileDamage projectileDamage = basicArrowPrefab.AddComponent<ProjectileDamage>();
            projectileDamage.damage = 10f;
            projectileDamage.crit = false;
            projectileDamage.force = 1000f;
            projectileDamage.damageType = DamageType.Generic;

            ProjectileSimple projectileSimple = basicArrowPrefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 5f;
            projectileSimple.desiredForwardSpeed = 30f;

            basicArrowPrefab.AddComponent<ArrowOnHit>();
            ProjectileImpaleOnEnemy impaleCon = basicArrowPrefab.AddComponent<ProjectileImpaleOnEnemy>();
            impaleCon.impalePrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("LinkArrow");

            PrefabAPI.RegisterNetworkPrefab(basicArrowPrefab);

        }

        private static void CreateSuperBombChildren() 
        {
            superBombChildrenPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("LinkChildBomb");
            superBombChildrenPrefab.AddComponent<NetworkIdentity>();

            ProjectileController projectileController = superBombChildrenPrefab.AddComponent<ProjectileController>();
            projectileController.procCoefficient = 1f;

            ProjectileDamage projectileDamage = superBombChildrenPrefab.AddComponent<ProjectileDamage>();
            projectileDamage.damage = 10f;
            projectileDamage.crit = false;
            projectileDamage.force = 1000f;
            projectileDamage.damageType = DamageType.Generic;

            ProjectileImpactExplosion projectileExplosion = superBombChildrenPrefab.AddComponent<ProjectileImpactExplosion>();
            projectileExplosion.explosionEffect = Modules.Assets.bombExplosionEffect;
            projectileExplosion.blastRadius = Modules.StaticValues.superBombChildrenRadius;
            projectileExplosion.blastDamageCoefficient = 1f;
            projectileExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileExplosion.destroyOnEnemy = false;
            projectileExplosion.destroyOnWorld = true;
            projectileExplosion.lifetimeAfterImpact = 2f;
            projectileExplosion.lifetime = 5f;

            ProjectileSimple projectileSimple = superBombChildrenPrefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 10f;
            projectileSimple.desiredForwardSpeed = 30f;

            PrefabAPI.RegisterNetworkPrefab(superBombChildrenPrefab);
        }

        private static void CreateSuperBomb() 
        {
            superBombPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("LinkSuperBomb");
            superBombPrefab.AddComponent<NetworkIdentity>();

            ProjectileController projectileController = superBombPrefab.AddComponent<ProjectileController>();
            projectileController.procCoefficient = 1f;

            ProjectileDamage projectileDamage = superBombPrefab.AddComponent<ProjectileDamage>();
            projectileDamage.damage = 10f;
            projectileDamage.crit = false;
            projectileDamage.force = 1000f;
            projectileDamage.damageType = DamageType.Generic;

            ProjectileImpactExplosion projectileExplosion = superBombPrefab.AddComponent<ProjectileImpactExplosion>();
            projectileExplosion.explosionEffect = Modules.Assets.bombExplosionEffect;
            projectileExplosion.blastRadius = Modules.StaticValues.superBombRadius;
            projectileExplosion.blastDamageCoefficient = 1f;
            projectileExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileExplosion.destroyOnEnemy = true;
            projectileExplosion.destroyOnWorld = true;
            projectileExplosion.lifetimeAfterImpact = 2f;
            projectileExplosion.lifetime = 3f;

            ProjectileSimple projectileSimple = superBombPrefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 3f;
            projectileSimple.desiredForwardSpeed = 30f;

            superBombPrefab.AddComponent<SuperBombOnHit>();


            PrefabAPI.RegisterNetworkPrefab(superBombPrefab);
        }

        private static void CreateStandardBomb() 
        {
            standardBombPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("LinkNormalBomb");
            standardBombPrefab.AddComponent<NetworkIdentity>();

            ProjectileController projectileController = standardBombPrefab.AddComponent<ProjectileController>();
            projectileController.procCoefficient = 1f;

            ProjectileDamage projectileDamage = standardBombPrefab.AddComponent<ProjectileDamage>();
            projectileDamage.damage = 10f;
            projectileDamage.crit = false;
            projectileDamage.force = 1000f;
            projectileDamage.damageType = DamageType.Generic;

            ProjectileImpactExplosion projectileExplosion = standardBombPrefab.AddComponent<ProjectileImpactExplosion>();
            projectileExplosion.explosionEffect = Modules.Assets.bombExplosionEffect;
            projectileExplosion.blastRadius = Modules.StaticValues.standardBombRadius;
            projectileExplosion.blastDamageCoefficient = 1f;
            projectileExplosion.falloffModel = BlastAttack.FalloffModel.None;
            projectileExplosion.destroyOnEnemy = true;
            projectileExplosion.destroyOnWorld = true;
            projectileExplosion.lifetimeAfterImpact = 2f;
            projectileExplosion.lifetime = 3f;

            ProjectileSimple projectileSimple = standardBombPrefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 3f;
            projectileSimple.desiredForwardSpeed = 30f;


            standardBombPrefab.AddComponent<StandardBombOnHit>();

            PrefabAPI.RegisterNetworkPrefab(standardBombPrefab);
        }

        private static void CreateSwordBeam()
        {
            swordBeamPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("SwordBeam");
            // Ensure that the child is set in the right position in Unity!!!!
            Modules.Prefabs.SetupHitbox(swordBeamPrefab, swordBeamPrefab.transform.GetChild(0), "swordbeam");
            swordBeamPrefab.AddComponent<NetworkIdentity>();
            ProjectileController swordProjectileController = swordBeamPrefab.AddComponent<ProjectileController>();

            ProjectileDamage swordbeamProjectileDamage = swordBeamPrefab.AddComponent<ProjectileDamage>();
            InitializeSwordBeamDamage(swordbeamProjectileDamage);

            ProjectileSimple swordbeamTrajectory = swordBeamPrefab.AddComponent<ProjectileSimple>();
            InitializeSwordBeamTrajectory(swordbeamTrajectory);

            ProjectileOverlapAttack swordbeamOverlapAttack = swordBeamPrefab.AddComponent<ProjectileOverlapAttack>();
            InitializeSwordBeamOverlapAttack(swordbeamOverlapAttack);
            swordBeamPrefab.AddComponent<SwordbeamOnHit>();

            swordProjectileController.procCoefficient = 1.0f;
            swordProjectileController.canImpactOnTrigger = true;

            PrefabAPI.RegisterNetworkPrefab(swordBeamPrefab);
        }

        internal static void InitializeSwordBeamOverlapAttack(ProjectileOverlapAttack overlap)
        {
            overlap.overlapProcCoefficient = 1.0f;
            overlap.damageCoefficient = 1.0f;
            //overlap.impactEffect = Modules.Assets.waterbladeimpactEffect;
        }

        internal static void InitializeSwordBeamTrajectory(ProjectileSimple simple)
        {
            simple.lifetime = Modules.StaticValues.swordBeamLifetime;
            simple.desiredForwardSpeed = Modules.StaticValues.swordBeamProjectileSpeed;

        }

        internal static void InitializeSwordBeamDamage(ProjectileDamage damageComponent)
        {
            damageComponent.damage = Modules.StaticValues.swordBeamDamageCoefficientBase;
            damageComponent.crit = false;
            damageComponent.force = Modules.StaticValues.swordBeamForce;
            damageComponent.damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }

        internal class SwordbeamOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public NetworkInstanceId netID;

            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                //Maybe something can be done for link
            }
        }

        internal class StandardBombOnHit : MonoBehaviour, IProjectileImpactBehavior
        {
            public NetworkInstanceId netID;

            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                //Maybe something can be done for link
            }
        }
    }
}