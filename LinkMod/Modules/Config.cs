using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions;
using System;
using RiskOfOptions.OptionConfigs;

namespace LinkMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<float> multiplierSpinAttack;
        public static ConfigEntry<float> armorSpinAttack;
        public static ConfigEntry<float> moveSpeedPenaltySpinAttack;

        public static ConfigEntry<float> glideSpeed;
        public static ConfigEntry<float> glideAcceleration;
        
        public static ConfigEntry<float> bombRecieveForce;
        public static ConfigEntry<float> runeBombSelfDestructTimer;
        public static ConfigEntry<float> bombMaxThrowPower;
        public static ConfigEntry<float> bombTimerToMaxCharge;

        public static void ReadConfig()
        {
            multiplierSpinAttack = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("00 - Sword", "Spin Attack Max Multiplier"),
                20f,
                new ConfigDescription("Determines damage and time to charge, 1 second is 1x multiplier. E.g 20s = 20x multiplier.",
                    null,
                    Array.Empty<object>()
                )
            );
            armorSpinAttack = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("00 - Sword", "Spin Attack Armor Buff"),
                40f,
                new ConfigDescription("Determines Armor increase when charging your spin attack.",
                    null,
                    Array.Empty<object>()
                )
            );
            moveSpeedPenaltySpinAttack = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("00 - Sword", "Spin Attack Movement Penalty"),
                0.2f,
                new ConfigDescription("Determines Spin Attack Movement Penalty when charging the attack.",
                    null,
                    Array.Empty<object>()
                )
            );
            glideSpeed = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("01 - Parasail", "Falling Speed when Parasail is deployed"), 
                60f, 
                new ConfigDescription("Determines the base speed of descent when the parasail is deployed.", 
                    null,
                    Array.Empty<object>()
                )
            );
            glideAcceleration = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("01 - Parasail", "Falling acceleration when Parasail is deployed"), 
                29.6f, 
                new ConfigDescription("Determines the acceleration when falling when the parasail is deployed", 
                    null, 
                    Array.Empty<object>()
                )
            );
            bombRecieveForce = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("02 - Bomb", "Force recieved and applied to bomb"),
                20f,
                new ConfigDescription("Determines how much force should be multiplied on the bomb when hit.",
                    null,
                    Array.Empty<object>()
                )
            );
            runeBombSelfDestructTimer = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("02 - Bomb", "Time before rune bomb self destructs"),
                60f,
                new ConfigDescription("Determines when the rune bomb should explode when not detonated manually",
                    null,
                    Array.Empty<object>()
                )
            );

            bombMaxThrowPower = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("02 - Bomb", "Max force applied to bomb when throw is fully charged"),
                400f,
                new ConfigDescription("Determines how powerful your throw is when the throw is fully charged",
                    null,
                    Array.Empty<object>()
                )
            );

            bombTimerToMaxCharge = LinkPlugin.instance.Config.Bind<float>
            (
                new ConfigDefinition("02 - Bomb", "Timer to fully charge a throw"),
                2f,
                new ConfigDescription("Determines how long it takes for Link to charge up a full powered throw", null, Array.Empty<object>())
            );
        }

        public static void SetupRiskOfOptions()
        {
            //Risk of Options intialization
            ModSettingsManager.AddOption(
                new StepSliderOption(
                    multiplierSpinAttack,
                    new StepSliderConfig
                    {
                        min = 5f,
                        max = 200f,
                        increment = 1f
                    }
                ));
            ModSettingsManager.AddOption(
                new StepSliderOption(
                    armorSpinAttack,
                    new StepSliderConfig
                    {
                        min = 0f,
                        max = 200f,
                        increment = 10f
                    }
                ));
            ModSettingsManager.AddOption(
                new StepSliderOption(
                    moveSpeedPenaltySpinAttack,
                    new StepSliderConfig
                    {
                        min = 0f,
                        max = 5f,
                        increment = 0.1f
                    }
                ));
            ModSettingsManager.AddOption(
                new StepSliderOption(
                    glideSpeed, 
                    new StepSliderConfig { 
                        min = 0,
                        max = 100f,
                        increment = 0.05f
                    }
                ));

            ModSettingsManager.AddOption(
                new StepSliderOption(
                    glideAcceleration,
                    new StepSliderConfig { 
                        min = 0f,
                        max = 100f,
                        increment = 0.05f
                    }    
                ));
            ModSettingsManager.AddOption(
                new StepSliderOption(
                    bombRecieveForce,
                    new StepSliderConfig
                    {
                        min = 1f,
                        max = 1000f,
                        increment = 1f
                    }
                ));
            ModSettingsManager.AddOption(
                new StepSliderOption(
                        runeBombSelfDestructTimer,
                        new StepSliderConfig 
                        {
                            min = 10f,
                            max = 1000000f,
                            increment = 10f
                        }
                    )
                );
            ModSettingsManager.AddOption(
                new StepSliderOption(
                    bombMaxThrowPower,
                    new StepSliderConfig
                    {
                        min = 10f,
                        max = 1000f,
                        increment = 10f
                    }
                ));
            ModSettingsManager.AddOption(
                new StepSliderOption(
                    bombTimerToMaxCharge,
                    new StepSliderConfig
                    {
                        min = 1f,
                        max = 20f,
                        increment = 0.5f
                    }
                ));
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return LinkPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}