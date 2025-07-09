using System.Collections;
using UnityEngine;

public class se_Bleeding : Statuseffect
{
    private Coroutine damageCoroutine;
    [SerializeField] private GameObject hite;
    protected override void Start()
    {
        base.Start();
        damageCoroutine = StartCoroutine(DamageOverTime());
    }

    private IEnumerator DamageOverTime()
    {
        if (Stack > 0 && TargetEntity != null && !TargetEntity.isDead)
            {
                float damage = Stack;
                TargetEntity.TakeDamage(
                    damage,
                    gardPenetration: 999999,
                    knockback: Vector2.zero,
                    attackType: AttackType.Magic,
                    Attacker: OwnerEntity,
                    attackpos: transform,
                    effect: hite
                );
            }
        yield return null;
    }

    public override void EffectInvoke()
    {
        base.EffectInvoke();

        StartCoroutine(DamageOverTime());

    }

    private void OnDisable()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
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
