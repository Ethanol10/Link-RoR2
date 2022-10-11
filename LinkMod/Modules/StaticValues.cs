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

        //Spin attack
        internal const float spinAttackMinorBlastDamageCoefficient = 0.5f;
        internal const float spinAttackMajorBlastDamageCoefficientBase = 5.0f;
        internal const float spinAttackMinorRadius = 10f;
        internal const float spinAttackMajorRadius = 15f;
        internal const int spinAttackBaseMinorHit = 5;
        internal const float spinAttackMaxMultiplier = 20f;
    }
}