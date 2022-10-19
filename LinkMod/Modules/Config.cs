using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions;
using System;
using RiskOfOptions.OptionConfigs;

namespace LinkMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<float> glideSpeed;
        public static ConfigEntry<float> glideAcceleration;
        public static ConfigEntry<float> bombRecieveForce;

        public static void ReadConfig()
        {
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
        }

        public static void SetupRiskOfOptions()
        {
            //Risk of Options intialization
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
                        min = 0f,
                        max = 100f,
                        increment = 0.05f
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