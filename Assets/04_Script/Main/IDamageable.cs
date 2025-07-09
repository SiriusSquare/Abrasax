using System;
using UnityEngine;
using UnityEngine.Events;


public enum AttackType
{
    Null = 0,
    Physics = 1,
    Projectile = 2,
    Magic = 3,
    HeatWave = 4,
    Wave = 5,
    Boom = 6,
    Laser = 7,
    SlashWave = 8,
    Impect = 9,
}
public enum AttackElement
{
    Null = 0,
    Flame = 1,
    Water = 2,
    Natural = 3,
    Ice = 4,
    Electric = 5,
    Light = 6,
    Dark = 7,
    Angel = 8,
    Demon = 9,
    Fantasy = 10,
    Cyber = 11,
    Ancient = 12,
    Digital = 13,
}
public interface IDamageable
{
    public float Health { get; set; }
    public float Shield { get; set; }
    public int GardLevel { get; set; }
    public float KnockbackResistance { get; set; }
    public string Faction { get; set; }

    public void TakeDamage(float damage, int gardPenetration, Vector2 knockback, AttackType atType, Entity Attacker,Transform attackPos,GameObject effect = null, float stuntime = 0.2f,HitType hittype = HitType.Normal);
    public void Heal(float healAmount);
    public void Die();

    public void KnockBack(Vector2 knockback);
}
