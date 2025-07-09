using UnityEngine;

public class StatManager : MonoBehaviour

{


    [field: SerializeField] public float MaxHealth { get; set; } = 100f;

    [field: SerializeField] public float HealthRecoverPower { get; set; } = 1f;
    [field: SerializeField] public float MaxStamina { get; set; } = 100f;

    [field: SerializeField] public float StaminaRecoverPower { get; set; } = 1f;
    [field: SerializeField] public float MaxMana { get; set; } = 100f;

    [field: SerializeField] public float ManaRecoverPower { get; set; } = 1f;

    [field: SerializeField] public float AttackPower { get; set; } = 10f;
    [field: SerializeField] public float AttackPowerMultiplier { get; set; } = 1f;

    [field: SerializeField] public float AttackSpeed { get; set; } = 1f;

    [field: SerializeField] public float Defense { get; set; } = 5f;
    [field: SerializeField] public float CriticalChance { get; set; } = 5f;
    [field: SerializeField] public float CriticalDamageMultiplier { get; set; } = 1.5f;
    [field: SerializeField] public int CriticalGardPenetration { get; set; } = 2;


    [field: SerializeField] public float KnockbackResistance { get; set; } = 0f;
    [field: SerializeField] public int DefGardLevel { get; set; } = 0;
    [field: SerializeField] public float MoveSpeed { get; set; } = 9f;
    [field: SerializeField] public float JumpForce { get; set; } = 12.5f;
    [field: SerializeField] public float CooldownTime { get; set; } = 1f;
    [field: SerializeField] public int MaxJumpCount { get; set; } = 1;
    [field: SerializeField] public int Gold { get; set; } = 0;
    [field: SerializeField] public int ArrowPoint { get; set; } = 0;

    [field: SerializeField] public Entity Entity { get; set; }
    public float EffectiveStaminaRecoverPower
    {
        get
        {
            if (Entity.entityStatusManager == null)
                return StaminaRecoverPower;

            var shockEffect = Entity.entityStatusManager.defaultEffectSettings.Find(e => e.type == StatusEffectType.Shock);
            int shockStack = shockEffect != null ? shockEffect.stack : 0;

            float reductionRatio = 0.02f * shockStack;
            reductionRatio = Mathf.Clamp(reductionRatio, 0f, 1f);

            return StaminaRecoverPower * (1f - reductionRatio);
        }
    }

    public float EffectiveAttackSpeed
    {
        get
        {
            if (Entity.entityStatusManager == null)
                return AttackSpeed;

            var frostEffect = Entity.entityStatusManager.defaultEffectSettings.Find(e => e.type == StatusEffectType.Frost);
            int frostStack = frostEffect != null ? frostEffect.stack : 0;

            float reductionRatio = 0.005f * frostStack;
            reductionRatio = Mathf.Clamp(reductionRatio, 0f, 1f);

            return AttackSpeed * (1f - reductionRatio);
        }
    }

    public float FinalAttackPower
    {
        get
        {
            if (AttackPower < 0 || AttackPowerMultiplier < 0)
                return 0f;
            var swEffect = Entity.entityStatusManager.defaultEffectSettings.Find(e => e.type == StatusEffectType.AttackUp);
            int Stack = swEffect != null ? swEffect.stack : 0;
            return AttackPower * AttackPowerMultiplier * (1 + (0.03f * Stack));
        }
        private set { }
    }

}
