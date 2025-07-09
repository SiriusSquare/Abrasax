using UnityEngine;

[CreateAssetMenu(fileName = "StatObject", menuName = "Scriptable Objects/StatObject")]
public class StatObject : ScriptableObject 

{


    [field:SerializeField] public float MaxHealth { get; private set; } = 100f;

    [field: SerializeField] public float HealthRecoverPower { get; private set; } = 1f;
    [field: SerializeField] public float MaxStamina { get; private set; } = 100f;

    [field: SerializeField] public float StaminaRecoverPower { get; private set; } = 1f;
    [field: SerializeField] public float MaxMana { get; private set; } = 100f;

    [field: SerializeField] public float ManaRecoverPower { get; private set; } = 1f;

    [field: SerializeField] public float AttackPower { get; private set; } = 10f;
    [field: SerializeField] public float AttackPowerMultiplier { get; private set; } = 1f;

    [field: SerializeField] public float AttackSpeed { get; private set; } = 1f;

    [field: SerializeField] public float Defense { get; private set; } = 5f;
    [field: SerializeField] public float CriticalChance { get; private set; } = 5f;
    [field: SerializeField] public float CriticalDamageMultiplier { get; private set; } = 1.5f;
    [field: SerializeField] public int CriticalGardPenetration { get; private set; } = 2;


    [field: SerializeField] public float KnockbackResistance { get; private set; } = 0f;
    [field: SerializeField] public int DefGardLevel { get; private set; } = 0;
    [field: SerializeField] public float MoveSpeed { get; private set; } = 9f;
    [field: SerializeField] public float JumpForce { get; private set; } = 12.5f;
    [field: SerializeField] public float CooldownTime { get; private set; } = 1f;
    [field: SerializeField] public int MaxJumpCount { get; private set; } = 1;
    [field : SerializeField] public int Gold { get; private set; } = 0;
    [field: SerializeField] public int ArrowPoint { get; private set; } = 0;

}
