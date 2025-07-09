using System;
using System.Collections.Generic;
using UnityEngine;
using Code.Core.Pooling;

public class AttackTriggerScript : MonoBehaviour, IPoolable
{
    public List<PendingEvent> pendingEvents = new();
    [SerializeField] private AttackType attackType = AttackType.Null;
    [SerializeField] private AttackElement attackElement = AttackElement.Null;

    [SerializeField] private float damage = 10f;
    [SerializeField] private int guardPenetration = 0;
    [SerializeField] private Vector2 knockback = Vector2.zero;
    [SerializeField] private GameObject effect;
    [SerializeField] private Entity ownerEntity;
    [SerializeField] private float lifeTime;
    [SerializeField] private float attackInterval = 0.5f;
    [SerializeField] private float stun = 0.2f;
    [SerializeField] private string itemName = "AttackHitBox";
    
    public string ItemName => itemName;
    public GameObject GameObject => gameObject;

    private Dictionary<IDamageable, float> damageTimers = new();
    private HashSet<IDamageable> insideTargets = new();

    private float currentlifetime;

    public void AddEvent(PendingEvent evt)
    {
        if (pendingEvents == null)
            pendingEvents = new List<PendingEvent>();
        pendingEvents.Add(evt);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable target))
        {
            if (collision.TryGetComponent<Entity>(out Entity entity))
            {
                if (ownerEntity != null && entity.Faction == ownerEntity.Faction)
                {
                    return; 
                }
            }
            insideTargets.Add(target);
            if (!damageTimers.ContainsKey(target))
                damageTimers[target] = attackInterval;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable target))
        {
            if (collision.TryGetComponent<Entity>(out Entity entity))
            {
                if (ownerEntity != null && entity.Faction == ownerEntity.Faction)
                {
                    return;
                }
            }
            insideTargets.Remove(target);
            damageTimers.Remove(target);
        }
    }


    private void Update()
    {
        var targets = new List<IDamageable>(insideTargets);
        foreach (var target in targets)
        {
            if (target == null)
            {
                insideTargets.Remove(target);
                continue;
            }

            if (!damageTimers.ContainsKey(target))
                continue;

            damageTimers[target] += Time.deltaTime;
            if (damageTimers[target] >= attackInterval)
            {
                ApplyDamage(target);
                damageTimers[target] = 0f;
            }
        }

        currentlifetime += Time.deltaTime;
        if (currentlifetime >= lifeTime)
        {
            ResetItem();
            PoolManager.Instance.Push(this);
        }
    }



    private void ApplyDamage(IDamageable target)
    {
        if (ownerEntity == null)
        {
            
            target.TakeDamage(damage, guardPenetration, knockback, attackType, null, transform,effect);
            
            return;
        }
        else
        {

            if (ownerEntity == null)
            {
                target.TakeDamage(damage, guardPenetration, knockback, attackType, null, transform, effect);
                return;
            }
            else
            {
                float finalDamage = damage;
                int finalGuardPenetration = guardPenetration;
                HitType hitType = HitType.Normal;

                if (ownerEntity.statObject.CriticalChance > UnityEngine.Random.Range(0f, 100f))
                {
                    finalDamage *= ownerEntity.statObject.CriticalDamageMultiplier;
                    finalGuardPenetration += ownerEntity.statObject.CriticalGardPenetration;
                    hitType = HitType.Critical;
                }

                target.TakeDamage(finalDamage, finalGuardPenetration, knockback, attackType, ownerEntity, transform, effect, stun, hitType);

                if (target is Entity entityTarget && pendingEvents != null)
                {
                    Debug.Log($"이벤트 실행 중, pendingEvents 개수: {pendingEvents.Count}");
                    foreach (var pe in pendingEvents)
                    {
                        Debug.Log($"이벤트 타입: {pe.eventType}, 대상: {entityTarget.name}");
                        foreach (var secCond in pe.secondaryConditions)
                        {
                            secCond.parameters[1] = finalDamage;
                        }
                        
                        EventManager.Instance.EventMain(ownerEntity, entityTarget, pe.condition, pe.secondaryConditions, pe.effect, pe.statusEffects, pe.eventType);

                    }
                }
            }
        }

        if (effect != null)
        {
            //var spawnedEffect = PoolManager.Instance.Pop(effect.name);
            
        }
    }

    public void ChangeSetting(float dam, int gP, Vector2 kb, GameObject effectPrefab, float aI, AttackType atType, AttackElement atElement, Entity owner, float lifetime)
    {
        damage = dam;
        guardPenetration = gP;
        knockback = kb;
        if (effect != null)
        {
            effect = effectPrefab;
        }
        effect = effectPrefab;
        attackInterval = aI;
        attackType = atType;
        attackElement = atElement;
        ownerEntity = owner;
        lifeTime = lifetime;
    }
    public void ChangeSetting2(float attackintaval, float stuntime = 0.2f)
    {
        attackInterval = attackintaval;
        stun = stuntime;
    }
    private void OnEnable()
    {
        insideTargets.Clear();
        damageTimers.Clear();


    }

    private void OnDisable()
    { 
        insideTargets.Clear();
        damageTimers.Clear();
    }
    private void OnDestroy()
    {
        if (PoolManager.HasInstance)
        {
            PoolManager.Instance.Remove(this);
        }
    }
    public void ResetItem()
    {
        insideTargets.Clear();
        damageTimers.Clear();
        currentlifetime = 0f;
        pendingEvents.Clear();

    }

}
