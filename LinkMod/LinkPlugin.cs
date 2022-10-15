using BepInEx;
using BepInEx.Bootstrap;
using EmotesAPI;
using LinkMod.Content.Link;
using LinkMod.Modules.Networking.Miscellaneous;
using R2API.Networking;
using R2API.Networking.Interfaces;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace LinkMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI",
        "NetworkingAPI"
    })]

    public class LinkPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.PopcornFactory.LoZ-LinkMod";
        public const string MODNAME = "LoZ-LinkMod";
        public const string MODVERSION = "0.0.1";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "POPCORN";

        public static LinkPlugin instance;
        public static bool emotesAvailable = false;

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.Config.SetupRiskOfOptions();
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new Link().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            Hook();
            SetupNetworkMessages();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.CharacterModel.Start += CharacterModel_Start;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            if (Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI"))
            {
                emotesAvailable = true;
                On.RoR2.SurvivorCatalog.Init += SurvivorCatalog_Init;
            }
        }

        private void SetupNetworkMessages() 
        {
            //Micellaneous
            NetworkingAPI.RegisterMessageType<ServerForceFallStateNetworkRequest>();

            //MasterSword
            NetworkingAPI.RegisterMessageType<ServerForceShieldBlockSuccessAnimNetworkRequest>();

            //Hylian Shield
            NetworkingAPI.RegisterMessageType<ServerForceShieldBlockSuccessAnimNetworkRequest>();
        }

        private void SurvivorCatalog_Init(On.RoR2.SurvivorCatalog.orig_Init orig)
        {
            orig();
            foreach (var item in SurvivorCatalog.allSurvivorDefs)
            {
                if (item.bodyPrefab.name == "LinkBody")
                {
                    CustomEmotesAPI.ImportArmature(item.bodyPrefab, Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("LinkHumanoid"));
                }
            }
        }

        private static bool DetermineDirectionOfAttackIsWithinRange(CharacterBody attacker, CharacterBody linkVictim) 
        {
            //Dot product the positions of the player look vector and the vector between attacker and link victim
            //1 = same dir
            //0 = perpendicular
            //-1 = opposite dir

            Vector3 lookingDir = linkVictim.inputBank.aimDirection.normalized;
            Vector3 linkToEnemyDir = attacker.transform.position - linkVictim.transform.position;
            linkToEnemyDir = linkToEnemyDir.normalized;
            if (Vector3.Dot(lookingDir, linkToEnemyDir) > 0.5f) 
            {
                return true;
            }

            return false;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {

            //Check if damage is from the front, by using the dot product to determine the vector between where link is facing
            //and where the enemy is attacking from.
            //Fuck the extra hitbox for now.
            if (self) 
            {
                if (self.body) 
                {
                    if (damageInfo.attacker) 
                    {
                        CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                        CharacterBody linkBody = self.body;
                        if (attackerBody) 
                        {
                            if (linkBody.baseNameToken == DEVELOPER_PREFIX + "_LINK_BODY_NAME")
                            {
                                //Check if blocking 
                                LinkController linkController = linkBody.GetComponent<LinkController>();
                                if (linkController.isShielding && self.body.HasBuff(Modules.Buffs.HylianShieldBuff))
                                {
                                    //now that we know it's link, check if the attack is from the front of link.
                                    if (DetermineDirectionOfAttackIsWithinRange(attackerBody, linkBody))
                                    {
                                        new ServerForceShieldBlockSuccessAnimNetworkRequest(linkBody.masterObjectId).Send(NetworkDestination.Clients);
                                        damageInfo.rejected = true;
                                        EffectData effectData = new EffectData
                                        {
                                            origin = damageInfo.position,
                                            rotation = Util.QuaternionSafeLookRotation((damageInfo.force != Vector3.zero) ? damageInfo.force : UnityEngine.Random.onUnitSphere)
                                        };
                                        EffectManager.SpawnEffect(HealthComponent.AssetReferences.bearEffectPrefab, effectData, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo);
        }

        private void CharacterModel_Start(On.RoR2.CharacterModel.orig_Start orig, CharacterModel self)
        {
            orig(self);
            if (self.gameObject.name.Contains("LinkDisplay"))
            {
                LinkRandomIdleAnimController displayHandler = self.gameObject.GetComponent<LinkRandomIdleAnimController>();
                if (!displayHandler)
                {
                    ChildLocator childLoc = self.gameObject.GetComponent<ChildLocator>();

                    if (childLoc)
                    {
                        displayHandler = self.gameObject.AddComponent<LinkRandomIdleAnimController>();
                    }
                }
                else 
                {
                    displayHandler.rerollAnim();
                }
            }
        }


        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.SpinAttackSlowDebuff)) 
                {
                    self.moveSpeed *= Modules.StaticValues.spinAttackMoveSpeedReduction;
                    self.armor += Modules.StaticValues.spinAttackArmourIncrease;
                }
                if (self.HasBuff(Modules.Buffs.HylianShieldBuff)) 
                {
                    self.moveSpeed *= Modules.StaticValues.hylianShieldReducedMoveSpeed;
                    self.armor += Modules.StaticValues.hylianShieldArmor;
                    self.jumpPower *= Modules.StaticValues.jumpPowerReduced;
                    self.maxJumpCount = Modules.StaticValues.maxJumpCount;
                }
            }
        }
    }
}