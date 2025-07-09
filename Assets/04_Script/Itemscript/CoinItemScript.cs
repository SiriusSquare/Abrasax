using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IItem
{
    float TargetDir { get; set; }
    float Speed { get; set; }
    LayerMask TargetLayer { get; set; }
    void UseItem(Entity entity);
}

public enum StatType
{
    MaxHealth,
    HealthRecoverPower,
    MaxStamina,
    StaminaRecoverPower,
    MaxMana,
    ManaRecoverPower,
    AttackPower,
    AttackPowerMultiplier,
    AttackSpeed,
    Defense,
    CriticalChance,
    CriticalDamageMultiplier,
    CriticalGardPenetration,
    KnockbackResistance,
    DefGardLevel,
    MoveSpeed,
    JumpForce,
    CooldownTime,
    MaxJumpCount,
    Gold,
    ArrowPoint,
    CurrentHealth,
    CurrentStamina,
    CurrentMana,
    CurrentShield,
    
}

[Serializable]
public struct StatIncreaseInfo
{
    public StatType stat;
    public float amount;
    public Color color;
}

public class CoinItemScript : MonoBehaviour, IItem, IPoolable
{
    [SerializeField] private string itemName;

    [SerializeField] private float targetDir = 2f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float firstenableTime = 0.5f;
    [SerializeField] public List<StatIncreaseInfo> statIncreases;

    public string ItemName => itemName;
    public GameObject GameObject => gameObject;

    public float TargetDir { get => targetDir; set => targetDir = value; }
    public float Speed { get => speed; set => speed = value; }

    public LayerMask TargetLayer { get => targetLayer; set => targetLayer = value; }

    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (firstenableTime > 0)
        {
            firstenableTime -= Time.deltaTime;
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocity.x, 0, 2f * Time.deltaTime);
        }
        if (firstenableTime < 0)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, targetDir, targetLayer);
            Collider2D hit2 = Physics2D.OverlapCircle(transform.position, 0.5f, targetLayer);
            if (hit != null && hit.TryGetComponent<Entity>(out Entity entity))
            {
                Vector2 direction = (entity.transform.position - transform.position);

                rb.AddForce(direction * speed, ForceMode2D.Impulse);
                rb.gravityScale = 0f;

            }
            else
            {
                rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, 0, 2f * Time.deltaTime), Mathf.Lerp(rb.linearVelocity.y, 0, 2f * Time.deltaTime));
                
                rb.gravityScale = 2.5f;
            }

            if (hit2 != null && hit2.TryGetComponent<Entity>(out entity))
            {
                UseItem(entity);
                PoolManager.Instance.Push(this);
            }
        }
        
    }

    public void UseItem(Entity entity)
    {
        if (entity.isDead)
        {
            return;
        }
        foreach (var info in statIncreases)
        {
            if (info.amount == 0) continue;

            ApplyStatIncrease(entity.statObject, info.stat, info.amount);
            entity.HPBarSystem?.SetHP();
            entity.HPBarSystem?.SetMana();

            if (info.stat != StatType.CurrentHealth || info.stat != StatType.CurrentShield)
            {
                IPoolable statText = PoolManager.Instance.Pop("ItemText");
                if (statText.GameObject != null)
                {

                    ItemTextScript itemTextScript = statText.GameObject.GetComponent<ItemTextScript>();
                    if (itemTextScript != null)
                    {

                        itemTextScript.SettingText(info.amount, info.stat.ToString(), info.color);
                        statText.GameObject.transform.position = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
                        statText.GameObject.SetActive(true);
                    }
                }
            }
            IPoolable statEffect = PoolManager.Instance.Pop("ItemCollectEffect");
            if (statEffect.GameObject != null)
            {
                statEffect.GameObject.transform.position = transform.position;
                statEffect.GameObject.GetComponent<Effect>().SetColor(info.color);
                statEffect.GameObject.SetActive(true);
            }

        }
        
            var itemEventHolder = GetComponent<EntityEventHolder>();
            if (itemEventHolder != null && entity.entityEventHolder != null)
            {
                foreach (var itemEvent in itemEventHolder.pendingEvents)
                {
                    entity.entityEventHolder.AddEvent(itemEvent);

                    
                    string eventText = SettigTextsa(itemEvent);
                    string eventLogText = SettigTextnonum(itemEvent);
                    FindAnyObjectByType<MenuScript>().ResumeTextPlus(eventLogText);
                    IPoolable statText = PoolManager.Instance.Pop("ItemText");
                    if (statText.GameObject != null)
                    {
                        ItemTextScript itemTextScript = statText.GameObject.GetComponent<ItemTextScript>();
                        if (itemTextScript != null)
                        {
                            itemTextScript.SettingText(0, eventText, Color.white,true);
                            statText.GameObject.transform.position = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
                            statText.GameObject.SetActive(true);
                        }
                    }
                
            }
        }
    }
    private string SettigTextsa(PendingEvent pe)
    {
        string returnstring = "";


        if (EventManager.GameEventNames.TryGetValue(pe.eventType, out var eventName))
            returnstring += $"{eventName}\n";


        if (EventManager.GameConditionNames.TryGetValue(pe.condition.conditionType, out var cond1))
        {
            if (cond1.Length == 1 || pe.condition.conditionType == EventCondition.ConditionType.FloorBetween)
                returnstring += $"{cond1[0]}\n";
            else
                returnstring += $"{cond1[0]} {pe.condition.threshold.ToString("F1")} {cond1[1]}\n";
        }


        if (pe.secondaryConditions != null && pe.secondaryConditions.Length > 0)
        {
            var secondCondition = pe.secondaryConditions[0];
            if (EventManager.GameSecondaryConditionNames.TryGetValue(secondCondition.conditionType, out var cond))
            {
                if (cond.Length == 1)
                    returnstring += $"{secondCondition.parameters[0].ToString("F1")}% {cond[0]}\n";
                else
                    returnstring += $"{cond[0]} {secondCondition.parameters[0].ToString("F1")} {cond[1]}\n";
            }
        }

        if (EventManager.EffectTypeNames.TryGetValue(pe.effect.effectType, out var effectNames))
        {
            var effectParams = pe.effect.parameters;

            for (int i = 1; i < effectNames.Length; ++i)
            {
                if (i - 1 < effectParams.Count)
                {
                    returnstring += $"{effectNames[i]} {effectParams[i - 1].ToString("F2")} ";
                }
            }

            returnstring += $"{effectNames[0]}\n";
        }

        return returnstring.TrimEnd();
    }

    private string SettigTextnonum(PendingEvent pe)
    {
        string returnstring = "";

        if (EventManager.GameEventNames.TryGetValue(pe.eventType, out var eventName))
        {
            returnstring += $"{eventName}";
        }


        if (EventManager.GameConditionNames.TryGetValue(pe.condition.conditionType, out var cond1))
        {
            if (cond1.Length == 1 || pe.condition.conditionType == EventCondition.ConditionType.FloorBetween)
                returnstring += $"{cond1[0]}";
            else
                returnstring += $"{cond1[0]} {pe.condition.threshold} {cond1[1]}";
        }


        if (EventManager.GameSecondaryConditionNames.TryGetValue(pe.secondaryConditions[0].conditionType, out var cond))
        {
            if (cond.Length == 1)
                returnstring += $"{pe.secondaryConditions[0].parameters[0]}% {cond[0]}";
            else
                returnstring += $"{cond[0]} {pe.secondaryConditions[0].parameters[0]} {cond[1]}";
        }


        if (EventManager.EffectTypeNames.TryGetValue(pe.effect.effectType, out var effectNames))
        {
            var effectParams = pe.effect.parameters;

            for (int i = 1; i < effectNames.Length; ++i)
            {

                if (i - 1 < effectParams.Count)
                {
                    returnstring += $"{effectNames[i]} {effectParams[i - 1]} ";
                }
            }


            returnstring += $"{effectNames[0]}";
        }

        return returnstring.TrimEnd();
    }
    private void OnDestroy()
    {
        if (PoolManager.HasInstance)
        {
            PoolManager.Instance.Remove(this);
        }
    }

    private void ApplyStatIncrease(StatManager stats, StatType stat, float amount)
    {
        switch (stat)
        {
            case StatType.MaxHealth:
                stats.MaxHealth += amount;
                break;
            case StatType.HealthRecoverPower:
                stats.HealthRecoverPower += amount;
                break;
            case StatType.MaxStamina:
                stats.MaxStamina += amount;
                break;
            case StatType.StaminaRecoverPower:
                stats.StaminaRecoverPower += amount;
                break;
            case StatType.MaxMana:
                stats.MaxMana += amount;
                break;
            case StatType.ManaRecoverPower:
                stats.ManaRecoverPower += amount;
                break;
            case StatType.AttackPower:
                stats.AttackPower += amount;
                break;
            case StatType.AttackPowerMultiplier:
                stats.AttackPowerMultiplier += amount;
                break;
            case StatType.AttackSpeed:
                stats.AttackSpeed += amount;
                break;
            case StatType.Defense:
                stats.Defense += amount;
                break;
            case StatType.CriticalChance:
                stats.CriticalChance += amount;
                break;
            case StatType.CriticalDamageMultiplier:
                stats.CriticalDamageMultiplier += amount;
                break;
            case StatType.CriticalGardPenetration:
                stats.CriticalGardPenetration += (int)amount;
                break;
            case StatType.KnockbackResistance:
                stats.KnockbackResistance += amount;
                break;
            case StatType.DefGardLevel:
                stats.DefGardLevel += (int)amount;
                break;
            case StatType.MoveSpeed:
                stats.MoveSpeed += amount;
                break;
            case StatType.JumpForce:
                stats.JumpForce += amount;
                break;
            case StatType.CooldownTime:
                stats.CooldownTime -= amount;
                if (stats.CooldownTime < 0.01f) stats.CooldownTime = 0.01f;
                break;
            case StatType.MaxJumpCount:
                stats.MaxJumpCount += (int)amount;
                break;
            case StatType.Gold:
                stats.Gold += (int)amount;
                break;
            case StatType.ArrowPoint:
                stats.ArrowPoint += (int)amount;
                break;
            case StatType.CurrentHealth:
                stats.gameObject.GetComponent<Entity>().Heal(amount);
                break;
            case StatType.CurrentStamina:
                stats.gameObject.GetComponent<Entity>().Stamina += amount;
                break;
            case StatType.CurrentMana:
                stats.gameObject.GetComponent<Entity>().Mana += amount;
                break;
            case StatType.CurrentShield:
                stats.gameObject.GetComponent<Entity>().AddShield(amount);
                break;
        }
    }


    public void ResetItem()
    {
        statIncreases.Clear();
        firstenableTime = 0.5f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 2.5f;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetDir);
    }
#endif
}
