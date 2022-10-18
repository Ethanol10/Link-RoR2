using System;
using System.Reflection;
using EntityStates;
using LinkMod.Modules;
using LinkMod.Modules.Characters;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LinkMod.Content.Link
{
    internal class RuneBomb : CharacterBase
    {
        public override string bodyName => "RuneBomb";

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "RuneBombBody",
            bodyNameToken = LinkPlugin.DEVELOPER_PREFIX + "_RUNE_BOMB_BODY_NAME",
            subtitleNameToken = LinkPlugin.DEVELOPER_PREFIX + "_RUNE_BOMB_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("bruh"),
            bodyColor = new Color(176f / 255f, 1.0f, 62f / 255f),

            crosshair = Assets.LoadCrosshair("Standard"),

            maxHealth = 999999f,
            healthRegen = 999999f,
            armor = 999999999f,

            jumpCount = 0,
        };


        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "GrenadeMesh",
                    material = Materials.CreateHopooMaterial("RuneBombMaterial"),
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "PinMesh",
                    material = Materials.CreateHopooMaterial("RuneBombMaterial"),
                    ignoreOverlays = true
                },
        };

        public override Type characterMainState => typeof(GenericCharacterMain);

        public override void InitializeSkills()
        {
            
        }

        internal override void InitializeCharacterBodyAndModel()
        {
            //Get explosivepot and replace the model. addressable: RoR2/Base/ExplosivePotDestructible/ExplosivePotDestructibleBody.prefab
            GameObject potObject = Addressables.LoadAssetAsync<GameObject>(key:"RoR2/Base/ExplosivePotDestructible/ExplosivePotDestructibleBody.prefab").WaitForCompletion();
            GameObject runeBomb = PrefabAPI.InstantiateClone(potObject, bodyName + "Body");
            GameObject model = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>($"mdl{bodyName}");
            Transform modelBase = new GameObject("ModelBase").transform;
            modelBase.parent = runeBomb.transform;
            modelBase.localPosition = bodyInfo.modelBasePosition;
            modelBase.localRotation = Quaternion.identity;
            Transform modelBaseTransform = modelBase.transform;
            #region CharacterBody
            CharacterBody bodyComponent = runeBomb.GetComponent<CharacterBody>();
            //identity
            bodyComponent.name = bodyInfo.bodyName;
            bodyComponent.baseNameToken = bodyInfo.bodyNameToken;
            bodyComponent.subtitleNameToken = bodyInfo.subtitleNameToken;
            bodyComponent.portraitIcon = bodyInfo.characterPortrait;
            bodyComponent.bodyColor = bodyInfo.bodyColor;

            bodyComponent._defaultCrosshairPrefab = bodyInfo.crosshair;
            bodyComponent.hideCrosshair = false;
            bodyComponent.preferredPodPrefab = bodyInfo.podPrefab;

            //stats
            bodyComponent.baseMaxHealth = bodyInfo.maxHealth;
            bodyComponent.baseRegen = bodyInfo.healthRegen;
            bodyComponent.levelArmor = bodyInfo.armorGrowth;
            bodyComponent.baseMaxShield = bodyInfo.shield;

            bodyComponent.baseDamage = bodyInfo.damage;
            bodyComponent.baseAttackSpeed = bodyInfo.attackSpeed;
            bodyComponent.baseCrit = bodyInfo.crit;

            bodyComponent.baseMoveSpeed = bodyInfo.moveSpeed;
            bodyComponent.baseJumpPower = bodyInfo.jumpPower;

            //level stats
            bodyComponent.autoCalculateLevelStats = bodyInfo.autoCalculateLevelStats;

            bodyComponent.levelDamage = bodyInfo.damageGrowth;
            bodyComponent.levelAttackSpeed = bodyInfo.attackSpeedGrowth;
            bodyComponent.levelCrit = bodyInfo.critGrowth;

            bodyComponent.levelMaxHealth = bodyInfo.healthGrowth;
            bodyComponent.levelRegen = bodyInfo.regenGrowth;
            bodyComponent.baseArmor = bodyInfo.armor;
            bodyComponent.levelMaxShield = bodyInfo.shieldGrowth;

            bodyComponent.levelMoveSpeed = bodyInfo.moveSpeedGrowth;
            bodyComponent.levelJumpPower = bodyInfo.jumpPowerGrowth;

            //other
            bodyComponent.baseAcceleration = bodyInfo.acceleration;

            bodyComponent.baseJumpCount = bodyInfo.jumpCount;

            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;

            bodyComponent.hullClassification = HullClassification.Human;

            bodyComponent.isChampion = false;
            #endregion
            Modules.Prefabs.SetupModelLocator(runeBomb, modelBaseTransform, model.transform);
            Modules.Prefabs.SetupRigidbody(runeBomb);


            #region MainHurtbox
            Modules.Prefabs.SetupMainHurtbox(runeBomb, model);
            #endregion
            Modules.Content.AddCharacterBodyPrefab(runeBomb);
            bodyPrefab = runeBomb;
            InitializeCharacterModel();
        }

        public override void InitializeItemDisplays() 
        {
            //nothing.
        }
    }
}

