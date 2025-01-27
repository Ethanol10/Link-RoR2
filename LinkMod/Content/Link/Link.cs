﻿using BepInEx.Configuration;
using EntityStates;
using LinkMod.Modules;
using LinkMod.Modules.Characters;
using LinkMod.Modules.Survivors;
using LinkMod.SkillStates.Link;
using LinkMod.SkillStates.Link.BowAndArrow;
using LinkMod.SkillStates.Link.GenericItemStates;
using LinkMod.SkillStates.Link.HylianShield;
using LinkMod.SkillStates.Link.MasterSwordPrimary;
using LinkMod.SkillStates.Link.MasterSwordSpinAttack;
using LinkMod.SkillStates.Link.RuneBomb;
using LinkMod.SkillStates.Link.StandardBomb;
using LinkMod.SkillStates.Link.SwapLoadout;
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

            characterPortrait = Assets.mainAssetBundle.LoadAsset<Texture>("lonkIcon"),
            bodyColor = new Color(176f/255f, 1.0f, 62f/255f),

            crosshair = Assets.LoadCrosshair("Standard"),

            maxHealth = 100f,
            healthRegen = 2f,
            armor = 10f,

            jumpCount = 2,
        };

        internal static Material defaultBodyMat = Materials.CreateHopooMaterial("Skin1Material");
        internal static Material defaultTool = Materials.CreateHopooMaterial("SwordShieldMaterial");

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "BodyObj",
                    material = defaultBodyMat,
                },
                new CustomRendererInfo
                {
                    childName = "SwordObj",
                    material = defaultTool,
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "ShieldObj",
                    material = defaultTool,
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "SheatheObj",
                    material = defaultTool,
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "SheathedShieldObj",
                    material = defaultTool,
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "SheathedSwordObj",
                    material = defaultTool,
                    ignoreOverlays = true
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(LinkCharacterMain);

        public override ItemDisplaysBase itemDisplays => new LinkItemDisplays();

        //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;



        /********************* SKILL DEF *******************************/
        //MasterSword SkillDef
        public static SkillDef masterSwordSkillDef;
        //HylianShield SkillDef
        public static SkillDef hylianShieldSkillDef;
        //Roll SkillDef
        public static SkillDef rollSkillDef;
        //SpinAttack SkillDef
        public static SkillDef spinAttackSkillDef;

        //Rune Bomb
        public static SkillDef runeBombSpawn;
        public static SkillDef runeBombDetonate;

        //Standard Bomb 
        public static SkillDef standardBombSpawn;

        //Super Bomb 
        public static SkillDef superBombSpawn;

        //Bow and Arrow Skills
        public static SkillDef arrowEntryPoint;
        public static SkillDef swapArrowEquipped;
        public static SkillDef swapArrowFiringType;
        public static SkillDef activateComboShot;

        //Extra skill swaps
        public static SkillDef swordLoadoutSkillDef;
        public static SkillDef bombLoadoutSkillDef;
        public static SkillDef arrowLoadoutSkillDef;
        public static SkillDef miscellaneousLoadoutSkillDef;
        /********************* SKILL DEF *******************************/




        internal static int mainRendererIndex { get; set; } = 5;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();
            bodyPrefab.AddComponent<LinkController>();
            EntityStateMachine linkEntityStateMachine = bodyPrefab.GetComponent<EntityStateMachine>();
            linkEntityStateMachine.initialStateType = new SerializableEntityStateType(typeof(LinkSpawnState));
        }

        public override void InitializeHurtboxes(HealthComponent healthComponent) 
        {
            base.InitializeHurtboxes(healthComponent);

            HurtBoxGroup mainHurtboxGroup = characterBodyModel.gameObject.GetComponent<HurtBoxGroup>();

            ChildLocator childLocator = characterBodyModel.GetComponent<ChildLocator>();

            //make a hurtbox for the shield since this works apparently !
            HurtBox shieldHurtbox = childLocator.FindChild("ShieldHurtbox").gameObject.AddComponent<HurtBox>();
            shieldHurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
            shieldHurtbox.healthComponent = healthComponent;
            shieldHurtbox.isBullseye = false;
            shieldHurtbox.damageModifier = HurtBox.DamageModifier.Barrier;
            shieldHurtbox.hurtBoxGroup = mainHurtboxGroup;

            mainHurtboxGroup.hurtBoxes = new HurtBox[] {
                shieldHurtbox,
                mainHurtboxGroup.hurtBoxes[0],
            };
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
            Skills.CreateExtraSkillFamilies(bodyPrefab, false);

            string prefix = LinkPlugin.DEVELOPER_PREFIX;

            #region Primary
            //Creates a skilldef for a typical primary 
            masterSwordSkillDef = Skills.CreateSkillDef(new SkillDefInfo(prefix + "_LINK_BODY_PRIMARY_MASTER_SWORD_NAME",
                                                                                      prefix + "_LINK_BODY_PRIMARY_MASTER_SWORD_DESCRIPTION",
                                                                                      Assets.mainAssetBundle.LoadAsset<Sprite>("masterSwordIcon"),
                                                                                      new EntityStates.SerializableEntityStateType(typeof(MasterSword)),
                                                                                      "Weapon",
                                                                                      true));
            masterSwordSkillDef.keywordTokens = new string[]
            {
                "POPCORN_LINK_BODY_PRIMARY_MASTER_SWORD_BEAM_KEYWORD",
                "POPCORN_LINK_BODY_PRIMARY_MASTER_SWORD_GROUNDED_SWING_KEYWORD",
                "POPCORN_LINK_BODY_PRIMARY_MASTER_SWORD_GROUNDED_DASH_KEYWORD",
                "POPCORN_LINK_BODY_PRIMARY_MASTER_SWORD_AERIAL_SWING_KEYWORD",
                "POPCORN_LINK_BODY_PRIMARY_MASTER_SWORD_AERIAL_DOWNSTAB_KEYWORD"
            };

            Skills.AddPrimarySkills(bodyPrefab, masterSwordSkillDef);
            #endregion

            #region Secondary
            hylianShieldSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SECONDARY_HYLIAN_SHIELD_NAME",
                skillNameToken = prefix + "_LINK_BODY_SECONDARY_HYLIAN_SHIELD_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SECONDARY_HYLIAN_SHIELD_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("hylianShieldIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(HylianShieldStart)),
                activationStateMachineName = "Weapon",
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
                keywordTokens = new string[] { "POPCORN_LINK_BODY_STEADY_KEYWORD" }
            });

            Skills.AddSecondarySkills(bodyPrefab, hylianShieldSkillDef);
            #endregion

            #region Utility
            rollSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_UTILITY_ROLL_NAME",
                skillNameToken = prefix + "_LINK_BODY_UTILITY_ROLL_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_UTILITY_ROLL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("rollIcon"),
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
            spinAttackSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SPECIAL_SPIN_ATTACK_NAME",
                skillNameToken = prefix + "_LINK_BODY_SPECIAL_SPIN_ATTACK_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SPECIAL_SPIN_ATTACK_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("spinAttackIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SpinAttack)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 10f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "POPCORN_LINK_BODY_FOCUSED_KEYWORD" }
            });

            Skills.AddSpecialSkills(bodyPrefab, spinAttackSkillDef);
            #endregion

            #region Extra Skills Region

            swordLoadoutSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SWORD_LOADOUT_NAME",
                skillNameToken = prefix + "_LINK_BODY_SWORD_LOADOUT_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SWORD_LOADOUT_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("swordLoadoutIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapSwordLoadout)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0.5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "" }
            });
            Skills.AddExtraSkillSlotPrimary(bodyPrefab, swordLoadoutSkillDef);

            arrowLoadoutSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_ARROW_LOADOUT_NAME",
                skillNameToken = prefix + "_LINK_BODY_ARROW_LOADOUT_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_ARROW_LOADOUT_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("arrowLoadoutIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapArrowLoadout)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0.5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "" }
            });
            Skills.AddExtraSkillSlotSecondary(bodyPrefab, arrowLoadoutSkillDef);

            bombLoadoutSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_BOMB_LOADOUT_NAME",
                skillNameToken = prefix + "_LINK_BODY_BOMB_LOADOUT_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_BOMB_LOADOUT_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("bombLoadoutIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapBombLoadout)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0.5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "" }
            });
            Skills.AddExtraSkillSlotUtility(bodyPrefab, bombLoadoutSkillDef);

            miscellaneousLoadoutSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_MISCELLANEOUS_LOADOUT_NAME",
                skillNameToken = prefix + "_LINK_BODY_MISCELLANEOUS_LOADOUT_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_MISCELLANEOUS_LOADOUT_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("noWorkIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapMiscellaneousLoadout)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0.5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "" }
            });
            Skills.AddExtraSkillSlotSpecial(bodyPrefab, miscellaneousLoadoutSkillDef);
            #endregion

            #region Rune Bomb
            runeBombSpawn = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_RUNE_BOMB_SPAWN_NAME",
                skillNameToken = prefix + "_LINK_BODY_RUNE_BOMB_SPAWN_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_RUNE_BOMB_SPAWN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("runeBombIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(RuneBombSpawn)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "" }
            });

            runeBombDetonate = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_RUNE_BOMB_DETONATE_NAME",
                skillNameToken = prefix + "_LINK_BODY_RUNE_BOMB_DETONATE_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_RUNE_BOMB_DETONATE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("runeBombIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(RuneBombDetonate)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "" }
            });
            #endregion

            #region Standard Bomb

            standardBombSpawn = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_STANDARD_BOMB_SPAWN_NAME",
                skillNameToken = prefix + "_LINK_BODY_STANDARD_BOMB_SPAWN_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_STANDARD_BOMB_SPAWN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("bombIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(StandardBombSpawn)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "" }
            });

            #endregion

            #region Super Bomb

            superBombSpawn = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SUPER_BOMB_SPAWN_NAME",
                skillNameToken = prefix + "_LINK_BODY_SUPER_BOMB_SPAWN_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SUPER_BOMB_SPAWN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("superBombIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SuperBombSpawn)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "" }
            });

            #endregion

            #region Bow and Arrow

            arrowEntryPoint = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_ARROW_FIRE_NAME",
                skillNameToken = prefix + "_LINK_BODY_ARROW_FIRE_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_ARROW_FIRE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("arrowIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(DrawBow)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "" }
            });

            swapArrowEquipped = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SWAP_ARROW_TYPE_NAME",
                skillNameToken = prefix + "_LINK_BODY_SWAP_ARROW_TYPE_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SWAP_ARROW_TYPE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("noWorkIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapArrowType)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "" }
            });

            swapArrowFiringType = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_SWAP_ARROW_FIRE_TYPE_NAME",
                skillNameToken = prefix + "_LINK_BODY_SWAP_ARROW_FIRE_TYPE_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_SWAP_ARROW_FIRE_TYPE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("noWorkIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SwapArrowFireType)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "" }
            });

            activateComboShot = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_LINK_BODY_COMBO_ARROW_TOGGLE_NAME",
                skillNameToken = prefix + "_LINK_BODY_COMBO_ARROW_TOGGLE_NAME",
                skillDescriptionToken = prefix + "_LINK_BODY_COMBO_ARROW_TOGGLE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("noWorkIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(ComboShotToggle)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "" }
            });
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
                },
                new SkinDef.MeshReplacement // Sheathed objects
                {
                    mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("shieldMesh"),
                    renderer = defaultRenderers[4].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("swordMesh"),
                    renderer = defaultRenderers[5].renderer
                },
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