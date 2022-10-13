using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LinkMod.Modules
{
    public static class Buffs
    {
        // armor buff gained during roll
        internal static BuffDef SpinAttackSlowDebuff;
        internal static BuffDef HylianShieldSlowDebuff;

        internal static void RegisterBuffs()
        {
            Sprite slowSprite = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdSlow80.asset").WaitForCompletion().iconSprite;
            SpinAttackSlowDebuff = AddNewBuff("Spin Attack Slow", slowSprite, Color.blue, false, false);
            HylianShieldSlowDebuff = AddNewBuff("Hylian Shield Slow Movement", slowSprite, Color.blue, false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}