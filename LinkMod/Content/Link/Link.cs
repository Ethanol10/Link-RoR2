using BepInEx.Configuration;
using EntityStates;
using LinkMod.Modules;
using LinkMod.Modules.Characters;
using LinkMod.Modules.Survivors;
using LinkMod.SkillStates.Link;
using LinkMod.SkillStates.Link.MasterSwordPrimary;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LinkMod.Content.Link
{
    internal class Link : SurvivorBase
    {
        public override string bodyName => "Link";

        public const string Link_PREFIX = LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_";
        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => Link_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "LinkBody",
            bodyNameToken = LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_NAME",
            subtitleNameToken = LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_SUBTITLE",

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("texLinkIcon"),
            bodyColor = Color.white,

            crosshair = Assets.LoadCrosshair("Standard"),

            maxHealth = 150f,
            healthRegen = 2f,
            armor = 0f,

            jumpCount = 2,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "BodyObj",
                    material = Materials.CreateHopooMaterial("Skin1Material"),
                },
                new CustomRendererInfo
                {
                    childName = "SwordObj",
                    material = Materials.CreateHopooMaterial("SwordShieldMaterial"),
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "ShieldObj",
                    material = Materials.CreateHopooMaterial("SwordShieldMaterial"),
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "SheatheObj",
                    material = Materials.CreateHopooMaterial("SwordShieldMaterial"),
                    ignoreOverlays = true
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new LinkItemDisplays();

        //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        internal static int mainRendererIndex { get; set; } = 3;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.AddComponent<LinkController>();
            EntityStateMachine linkEntityStateMachine = bodyPrefab.GetComponent<EntityStateMachine>();
            linkEntityStateMachine.initialStateType = new SerializableEntityStateType(typeof(LinkSpawnState));
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform aerialSwingHitbox = childLocator.FindChild("AerialSwingHitbox");
            Modules.Prefabs.SetupHitbox(model, aerialSwingHitbox, "AerialSwingHitbox");

            Transform AerialDownstabHitbox = childLocator.FindChild("AerialDownstabHitbox");
            Modules.Prefabs.SetupHitbox(model, AerialDownstabHitbox, "AerialDownstabHitbox");

            Transform GroundedSwingHitbox = childLocator.FindChild("GroundedSwingHitbox");
            Modules.Prefabs.SetupHitbox(model, GroundedSwingHitbox, "GroundedSwingHitbox");

            Transform GroundedFinalSwingHitbox = childLocator.FindChild("GroundedFinalSwingHitbox");
            Modules.Prefabs.SetupHitbox(model, GroundedFinalSwingHitbox, "GroundedFinalSwingHitbox");

            Transform GroundedDashAttack = childLocator.FindChild("GroundedDashAttack");
            Modules.Prefabs.SetupHitbox(model, GroundedDashAttack, "GroundedDashAttack");
        }

        public override void InitializeSkills()
        {
            Skills.CreateSkillFamilies(bodyPrefab);
            string prefix = LinkPlugin.DEVELOPER_PREFIX;

            #region Primary
            //Creates a skilldef for a typical primary 
            SkillDef primarySkillDef = Skills.CreateSkillDef(new SkillDefInfo(prefix + "_LINK_BODY_PRIMARY_SLASH_NAME",
                                                                                      prefix + "_LINK_BODY_PRIMARY_SLASH_DESCRIPTION",
                                                                                      Assets.mainAssetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(SpinAttack)),
                                                                                      "Weapon",
                                                                                      true));


            Skills.AddPrimarySkills(bodyPrefab, primarySkillDef);
            #endregion

            #region Secondary
            SkillDef shootSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SECONDARY_GUN_NAME",
                skillNameToken = prefix + "_LINK_BODY_SECONDARY_GUN_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SECONDARY_GUN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SpinAttack)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_AGILE" }
            });

            Skills.AddSecondarySkills(bodyPrefab, shootSkillDef);
            #endregion

            #region Utility
            SkillDef rollSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_UTILITY_ROLL_NAME",
                skillNameToken = prefix + "_LINK_BODY_UTILITY_ROLL_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_UTILITY_ROLL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Roll)),
                activationStateMachineName = "Body",
                baseMaxStock = 2,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 2,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.AddUtilitySkills(bodyPrefab, rollSkillDef);
            #endregion

            #region Special
            SkillDef bombSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SPECIAL_BOMB_NAME",
                skillNameToken = prefix + "_LINK_BODY_SPECIAL_BOMB_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SPECIAL_BOMB_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SpinAttack)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 10f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.AddSpecialSkills(bodyPrefab, bombSkillDef);
            #endregion
        }

        public override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Skins.CreateSkinDef(LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                //place your mesh replacements here
                //unnecessary if you don't have multiple skins
                new SkinDef.MeshReplacement
                {
                    mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("defaultBodyMesh"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("swordMesh"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("shieldMesh"),
                    renderer = defaultRenderers[2].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("sheatheMesh"),
                    renderer = defaultRenderers[3].renderer
                }
            };

            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            /*
            Material masteryMat = Modules.Materials.CreateHopooMaterial("matLinkAlt");
            CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                masteryMat,
                masteryMat,
                masteryMat,
                masteryMat
            });

            SkinDef masterySkin = Modules.Skins.CreateSkinDef(LinkPlugin.DEVELOPER_PREFIX + "_LINK_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                masteryRendererInfos,
                mainRenderer,
                model,
                masterySkinUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshLinkSwordAlt"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshLinkAlt"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];

            defaultRenderers.CopyTo(newRendererInfos, 0);

            for (int i = 0; i < mainRendererIndex; i++)
            {
                newRendererInfos[i].defaultMaterial = materials[i];
            }

            return newRendererInfos;
        }
    }
}