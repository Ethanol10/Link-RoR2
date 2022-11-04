namespace LinkMod.Modules
{
    internal static class StaticValues
    {
        internal const float swordDamageCoefficient = 2.8f;

        internal const float gunDamageCoefficient = 4.2f;

        internal const float bombDamageCoefficient = 16f;

        //Master sword 
        internal const float msGroundedBasicSwing = 0.8f;
        internal const float msGroundedFinalSwing = 3.5f;
        internal const float msGroundedDashAttack = 4.0f;
        internal const float msAerialDoubleSwingFirst = 1.0f;
        internal const float msAerialDoubleSwingSecond = 2.5f;
        internal const float msAerialDownstab = 2.0f;

        //Master sword beam
        internal const float swordBeamProcCoefficient = 1.0f;
        internal const float swordBeamDamageCoefficientBase = 2.0f;
        internal const float swordBeamForce = 150f;
        internal const float swordBeamLifetime = 1.5f;
        internal const float swordBeamProjectileSpeed = 20f;
        internal const float healthRequiredToFirePercentage = 0.85f;

        //Spin attack
        internal const float spinAttackMinorBlastDamageCoefficient = 1f;
        internal const float spinAttackMajorBlastDamageCoefficientBase = 10.0f;
        internal const float spinAttackMinorRadius = 15f;
        internal const float spinAttackMajorRadius = 20f;
        internal const int spinAttackBaseMinorHit = 5;
        internal const float spinAttackMaxMultiplier = 20f;
        internal const float spinAttackMoveSpeedReduction = 0.2f;
        internal const float spinAttackArmourIncrease = 40f;

        //Hylian Shield 
        internal const float hylianShieldReducedMoveSpeed = 0.6f;
        internal const float hylianShieldArmor = 30f;
        internal const float jumpPowerReduced = 0.7f;
        internal const int maxJumpCount = 1;

        //Rune Bomb
        internal const float runeBombBlastDamageCoefficient = 7f;
        internal const float runeBombBlastForce = 3000f;
        internal const float runeBombRadius = 10f;

        //Standard Bomb
        internal const float standardBombBlastDamageCoefficient = 10f;
        internal const float standardBombBlastForce = 4000f;
        internal const float standardBombRadius = 12f;

        //Super Bomb
        internal const float superBombBlastDamageCoefficient = 20f;
        internal const float superBombBlastForce = 4000f;
        internal const float superBombRadius = 15f;
        internal const float superBombChildrenBlastDamageCoefficient = 4f;
        internal const float superBombChildrenRadius = 5f;
        internal const float superBombChildrenBlastForce = 1000f;
    }
}