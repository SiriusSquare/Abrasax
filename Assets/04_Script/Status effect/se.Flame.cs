using System.Collections;
using UnityEngine;

public class se_Flame : Statuseffect
{
    private Coroutine damageCoroutine;

    protected override void Start()
    {
        base.Start();
        damageCoroutine = StartCoroutine(DamageOverTime());
    }

    private IEnumerator DamageOverTime()
    {
        if (Stack > 0 && TargetEntity != null && !TargetEntity.isDead)
            {
                float damage = Stack * 5f;
                TargetEntity.TakeDamage(
                    damage,
                    gardPenetration: 999999,
                    knockback: Vector2.zero,
                    attackType: AttackType.Magic,
                    Attacker: OwnerEntity,
                    attackpos: transform,
                    effect: null
                );
            }
        yield return null;
    }

    public override void EffectInvoke()
    {
        base.EffectInvoke();

        StartCoroutine(DamageOverTime());
        Stack--;

    }

    public override void ResetItem()
    {
        base.ResetItem();

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = StartCoroutine(DamageOverTime());
    }
}
