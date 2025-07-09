using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
}

public class EnchantShopScript : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject ShopIcon;
    [SerializeField] private GameObject text;
    [SerializeField] private PendingEvent pendingEvent;

    private CanvasGroup textCanvasGroup;
    private Vector3 textOriginalPos;

    [SerializeField] private Rarity rare = Rarity.Common;

    private System.Random random = new System.Random();
    [SerializeField] private int itemPrice = 0;

    [SerializeField] private bool isRandomCount = false;
    [SerializeField] private int maxItemCount = 5;
    [SerializeField] private int setmaxItemCount = 10;
    [SerializeField] private int setminItemCount = 0;

    private bool isTextVisible = false;
    private Sequence textSequence;

    private float nextBuyTime = 0f;

    private void Awake()
    {
        if (isRandomCount)
        {
            maxItemCount = random.Next(setminItemCount, setmaxItemCount + 1);
        }
        CreateRandomEvent();
        if (text != null)
        {
            textCanvasGroup = text.GetComponent<CanvasGroup>();
            textOriginalPos = text.transform.localPosition;

            textCanvasGroup.alpha = 0f;
            text.transform.localPosition = textOriginalPos;

            RefreshShopDescription();
        }
    }

    private void CreateRandomEvent()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < 0.4f) rare = Rarity.Common;
        else if (rand < 0.65f) rare = Rarity.Uncommon;
        else if (rand < 0.80f) rare = Rarity.Rare;
        else if (rand < 0.95f) rare = Rarity.Epic;
        else rare = Rarity.Legendary;

        if (rare == Rarity.Common) itemPrice = 100;
        else if (rare == Rarity.Uncommon) itemPrice = 125;
        else if (rare == Rarity.Rare) itemPrice = 150;
        else if (rare == Rarity.Epic) itemPrice = 175;
        else if (rare == Rarity.Legendary) itemPrice = 200;
        SetupPendingEvent();
    }

    private void SetupPendingEvent()
    {
        Array allEventTypes = Enum.GetValues(typeof(GameEventType));
        pendingEvent.eventType = (GameEventType)allEventTypes.GetValue(UnityEngine.Random.Range(0, allEventTypes.Length));

        pendingEvent.condition = CreateConditionForRarity(rare);
        pendingEvent.secondaryConditions[0] = CreateSecondaryConditionForRarity(rare) ;
        pendingEvent.effect = CreateEffectForRarity(rare, pendingEvent.eventType);
    }

    private EventCondition CreateConditionForRarity(Rarity rarity)
    {
        var allValues = Enum.GetValues(typeof(EventCondition.ConditionType));
        EventCondition.ConditionType conditionType = (EventCondition.ConditionType)allValues.GetValue(UnityEngine.Random.Range(0, allValues.Length));

        float value = 0f;

        switch (conditionType)
        {
            case EventCondition.ConditionType.HealthBelow:
                value = rarity switch
                {
                    Rarity.Common => UnityEngine.Random.Range(60, 80),
                    Rarity.Uncommon => UnityEngine.Random.Range(65, 85),
                    Rarity.Rare => UnityEngine.Random.Range(70, 90),
                    Rarity.Epic => UnityEngine.Random.Range(75, 95),
                    Rarity.Legendary => UnityEngine.Random.Range(80, 99),
                    _ => 60f
                };
                break;

            case EventCondition.ConditionType.HealthAbove:
                value = rarity switch
                {
                    Rarity.Common => UnityEngine.Random.Range(30, 50),
                    Rarity.Uncommon => UnityEngine.Random.Range(25, 45),
                    Rarity.Rare => UnityEngine.Random.Range(20, 40),
                    Rarity.Epic => UnityEngine.Random.Range(15, 35),
                    Rarity.Legendary => UnityEngine.Random.Range(10, 30),
                    _ => 30f
                };
                break;

            case EventCondition.ConditionType.StaminaAbove:
                value = rarity switch
                {
                    Rarity.Common => UnityEngine.Random.Range(30, 50),
                    Rarity.Uncommon => UnityEngine.Random.Range(25, 45),
                    Rarity.Rare => UnityEngine.Random.Range(20, 40),
                    Rarity.Epic => UnityEngine.Random.Range(15, 35),
                    Rarity.Legendary => UnityEngine.Random.Range(10, 30),
                    _ => 30f
                };
                break;

            case EventCondition.ConditionType.ManaAbove:
                value = rarity switch
                {
                    Rarity.Common => UnityEngine.Random.Range(30, 50),
                    Rarity.Uncommon => UnityEngine.Random.Range(25, 45),
                    Rarity.Rare => UnityEngine.Random.Range(20, 40),
                    Rarity.Epic => UnityEngine.Random.Range(15, 35),
                    Rarity.Legendary => UnityEngine.Random.Range(10, 30),
                    _ => 30f
                };
                break;

            case EventCondition.ConditionType.FloorAbove:
                value = rarity switch
                {
                    Rarity.Common => UnityEngine.Random.Range(0, 45),
                    Rarity.Uncommon => UnityEngine.Random.Range(0, 40),
                    Rarity.Rare => UnityEngine.Random.Range(0, 35),
                    Rarity.Epic => UnityEngine.Random.Range(0, 30),
                    Rarity.Legendary => UnityEngine.Random.Range(0, 25),
                    _ => 40f
                };
                break;

            case EventCondition.ConditionType.FloorBelow:
                value = rarity switch
                {
                    Rarity.Common => UnityEngine.Random.Range(55, 100),
                    Rarity.Uncommon => UnityEngine.Random.Range(60, 100),
                    Rarity.Rare => UnityEngine.Random.Range(65, 100),
                    Rarity.Epic => UnityEngine.Random.Range(70, 100),
                    Rarity.Legendary => UnityEngine.Random.Range(75, 100),
                    _ => 30f
                };
                break;

            case EventCondition.ConditionType.FloorBetween:
            case EventCondition.ConditionType.Always:
                value = 0f;
                break;
        }

        return new EventCondition
        {
            conditionType = conditionType,
            threshold = value
        };
    }


    private SecondaryCondition CreateSecondaryConditionForRarity(Rarity rarity)
    {
        bool isHitEvent = pendingEvent.eventType == GameEventType.PlayerAttackHit
        || pendingEvent.eventType == GameEventType.PlayerBasicAttackHit
                      || pendingEvent.eventType == GameEventType.PlayerPierceHit
                      || pendingEvent.eventType == GameEventType.PlayerTriSlashHit
                      || pendingEvent.eventType == GameEventType.PlayerUppercutHit
                      || pendingEvent.eventType == GameEventType.PlayerSpinSlashHit
                      || pendingEvent.eventType == GameEventType.PlayerCounterHit
                      || pendingEvent.eventType == GameEventType.PlayerHorizontalHit
                      || pendingEvent.eventType == GameEventType.EquinoxMultiPierceHit
                      || pendingEvent.eventType == GameEventType.EquinoxMultiCutHit
                      || pendingEvent.eventType == GameEventType.EquinoxBigCutHit;

        List<float> parameters = new List<float> { 0f, 0f, 0f };
        SecondaryCondition.SecondaryConditionType conditionType;

        if (!isHitEvent)
        {
            // 히트 이벤트가 아닐 때는 None 또는 RandomChance
            conditionType = UnityEngine.Random.value > 0.5f
                ? SecondaryCondition.SecondaryConditionType.None
                : SecondaryCondition.SecondaryConditionType.RandomChance;

            if (conditionType == SecondaryCondition.SecondaryConditionType.RandomChance)
            {
                parameters[0] = Mathf.Max(UnityEngine.Random.Range(50f + ((int)rarity * 10), 100f), 100f);
            }
        }
        else
        {
            // 히트 이벤트일 때 원래 로직
            Array conditionValues = Enum.GetValues(typeof(SecondaryCondition.SecondaryConditionType));
            conditionType = (SecondaryCondition.SecondaryConditionType)conditionValues.GetValue(UnityEngine.Random.Range(0, conditionValues.Length));

            switch (conditionType)
            {
                case SecondaryCondition.SecondaryConditionType.RandomChance:
                    parameters[0] = Mathf.Max(UnityEngine.Random.Range(50f + ((int)rarity * 10), 100f), 100f);
                    break;

                case SecondaryCondition.SecondaryConditionType.PlayerDamageAbove:
                case SecondaryCondition.SecondaryConditionType.PlayerHitDamageAbove:
                    parameters[0] = UnityEngine.Random.Range(30f, 100f);
                    break;

                case SecondaryCondition.SecondaryConditionType.None:
                    break;

            }
        }


        switch (conditionType)
        {
            case SecondaryCondition.SecondaryConditionType.RandomChance:
                parameters[0] = Mathf.Max(UnityEngine.Random.Range(50f + ((int)rarity * 10),100f),100f);
                break;


            case SecondaryCondition.SecondaryConditionType.PlayerDamageAbove:
            case SecondaryCondition.SecondaryConditionType.PlayerHitDamageAbove:
                parameters[0] = UnityEngine.Random.Range(30f, 100f);
                break;

            case SecondaryCondition.SecondaryConditionType.None:
                break;
        }

        return new SecondaryCondition
        {
            conditionType = conditionType,
            parameters = parameters
        };
    }

    private EventEffect CreateEffectForRarity(Rarity rare, GameEventType eventType)
    {
        bool isHitEvent = eventType == GameEventType.PlayerAttackHit
                          || eventType == GameEventType.PlayerBasicAttackHit
                          || eventType == GameEventType.PlayerPierceHit
                          || eventType == GameEventType.PlayerTriSlashHit
                          || eventType == GameEventType.PlayerUppercutHit
                          || eventType == GameEventType.PlayerSpinSlashHit
                          || eventType == GameEventType.PlayerCounterHit
                          || eventType == GameEventType.PlayerHorizontalHit
                          || eventType == GameEventType.EquinoxMultiPierceHit
                          || eventType == GameEventType.EquinoxMultiCutHit
                          || eventType == GameEventType.EquinoxBigCutHit;

        Array effectValues = Enum.GetValues(typeof(EventEffect.EffectType));
        List<EventEffect.EffectType> bannedForNonHit = new List<EventEffect.EffectType>
    {
        EventEffect.EffectType.Bleed,
        EventEffect.EffectType.Stun,
        EventEffect.EffectType.Burn,
        EventEffect.EffectType.Frost,
        EventEffect.EffectType.Drench,
        EventEffect.EffectType.Poison,
        EventEffect.EffectType.Shock,
        EventEffect.EffectType.Impact,
        EventEffect.EffectType.Vampirism,
    };
        EventEffect.EffectType selectedType;

        do
        {
            selectedType = (EventEffect.EffectType)effectValues.GetValue(UnityEngine.Random.Range(0, effectValues.Length));
        } while (!isHitEvent && bannedForNonHit.Contains(selectedType));

        List<float> parameters = new List<float> { 0f, 0f, 0f };
        switch (selectedType)
        {
            case EventEffect.EffectType.Vampirism:
                parameters[0] = rare switch
                {
                    Rarity.Common => 10,
                    Rarity.Uncommon => 20,
                    Rarity.Rare => 30,
                    Rarity.Epic => 40,
                    Rarity.Legendary => 50,
                    _ => 1
                };
                break;

            case EventEffect.EffectType.Bleed:
            case EventEffect.EffectType.Stun:
            case EventEffect.EffectType.Burn:
            case EventEffect.EffectType.Frost:
            case EventEffect.EffectType.Drench:
            case EventEffect.EffectType.Poison:
            case EventEffect.EffectType.Shock:
            case EventEffect.EffectType.AttackUp:
            case EventEffect.EffectType.DefenseUp:
                parameters[0] = rare switch
                {
                    Rarity.Common => 1,
                    Rarity.Uncommon => 2,
                    Rarity.Rare => 3,
                    Rarity.Epic => 4,
                    Rarity.Legendary => 5,
                    _ => 1
                };
                break;

            case EventEffect.EffectType.Shield:
                parameters[0] = rare switch
                {
                    Rarity.Common => 1,
                    Rarity.Uncommon => 2,
                    Rarity.Rare => 3,
                    Rarity.Epic => 4,
                    Rarity.Legendary => 5,
                    _ => 1
                };
                break;

            case EventEffect.EffectType.Impact:
                parameters[0] = rare switch
                {
                    Rarity.Common => 1,
                    Rarity.Uncommon => 2,
                    Rarity.Rare => 3,
                    Rarity.Epic => 4,
                    Rarity.Legendary => 5,
                    _ => 1
                };
                parameters[1] = rare switch
                {
                    Rarity.Common => 0.4f,
                    Rarity.Uncommon => 0.6f,
                    Rarity.Rare => 0.8f,
                    Rarity.Epic => 1.0f,
                    Rarity.Legendary => 1.25f,
                    _ => 1.0f
                };
                break;

            case EventEffect.EffectType.Explosion:
                parameters[0] = rare switch
                {
                    Rarity.Common => 0.6f,
                    Rarity.Uncommon => 1.4f,
                    Rarity.Rare => 2f,
                    Rarity.Epic => 2.9f,
                    Rarity.Legendary => 5f,
                    _ => 1f
                };
                parameters[1] = rare switch
                {
                    Rarity.Common => 2f,
                    Rarity.Uncommon => 3f,
                    Rarity.Rare => 4f,
                    Rarity.Epic => 5f,
                    Rarity.Legendary => 6f,
                    _ => 2f
                };
                break;

            case EventEffect.EffectType.Wave:
                parameters[0] = rare switch
                {
                    Rarity.Common => 0.6f,
                    Rarity.Uncommon => 0.8f,
                    Rarity.Rare => 1f,
                    Rarity.Epic => 1.2f,
                    Rarity.Legendary => 1.4f,
                    _ => 1f
                };
                parameters[1] = rare switch
                {
                    Rarity.Common => 0.4f,
                    Rarity.Uncommon => 0.3f,
                    Rarity.Rare => 0.2f,
                    Rarity.Epic => 0.15f,
                    Rarity.Legendary => 0.1f,
                    _ => 0.2f
                };
                parameters[2] = rare switch
                {
                    Rarity.Common => 2f,
                    Rarity.Uncommon => 3f,
                    Rarity.Rare => 4f,
                    Rarity.Epic => 5f,
                    Rarity.Legendary => 6f,
                    _ => 2f
                };
                break;

            case EventEffect.EffectType.HeatWave:
                parameters[0] = rare switch
                {
                    Rarity.Common => 0.3f,
                    Rarity.Uncommon => 0.4f,
                    Rarity.Rare => 0.5f,
                    Rarity.Epic => 0.6f,
                    Rarity.Legendary => 0.7f,
                    _ => 1f
                };
                parameters[1] = rare switch
                {
                    Rarity.Common => 0.6f,
                    Rarity.Uncommon => 0.8f,
                    Rarity.Rare => 1f,
                    Rarity.Epic => 1.2f,
                    Rarity.Legendary => 1.6f,
                    _ => 0.2f
                };
                parameters[2] = rare switch
                {
                    Rarity.Common => 3f,
                    Rarity.Uncommon => 2f,
                    Rarity.Rare => 2f,
                    Rarity.Epic => 1f,
                    Rarity.Legendary => 0f,
                    _ => 1f
                };
                break;

            default:
                parameters[0] = 1f;
                break;
        }

        return new EventEffect
        {
            effectType = selectedType,
            parameters = parameters
        };
    }




    private void Update()
    {
        if (nextBuyTime > 0f) nextBuyTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isTextVisible) return;
        ShowElevatorText(SettigTextsa(pendingEvent) + $"\n{rare} 등급, {itemPrice} AP, {maxItemCount} 개 남음");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !isTextVisible) return;
        HideElevatorText();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        

        if (Keyboard.current.upArrowKey.isPressed && nextBuyTime <= 0f && collision.CompareTag("Player"))
        {
            Entity entity = collision.GetComponent<Entity>();
            if (entity == null) return;

            if (entity.statObject.ArrowPoint < itemPrice) return;
            entity.statObject.ArrowPoint -= itemPrice;
            ActiveItemPop();
        }
        }




    private void ShowElevatorText(string message)
    {
        if (text == null || textCanvasGroup == null) return;

        isTextVisible = true;

        textSequence?.Kill(true);

        text.SetActive(true);
        var textComponent = text.GetComponent<TMPro.TextMeshProUGUI>();
        if (textComponent != null) textComponent.text = message;

        text.transform.localPosition = textOriginalPos + new Vector3(0, -5f, 0);
        textCanvasGroup.alpha = 0f;

        textSequence = DOTween.Sequence();
        textSequence.Append(text.transform.DOLocalMoveY(textOriginalPos.y, 0.25f).SetEase(Ease.OutCubic));
        textSequence.Join(textCanvasGroup.DOFade(1f, 0.25f));
        textSequence.Play();
    }

    private void HideElevatorText()
    {
        if (text == null || textCanvasGroup == null) return;

        isTextVisible = false;

        textSequence?.Kill(true);
        textSequence = DOTween.Sequence();
        textSequence.Append(text.transform.DOLocalMoveY(textOriginalPos.y - 2f, 0.25f).SetEase(Ease.InCubic));
        textSequence.Join(textCanvasGroup.DOFade(0f, 0.25f));
        textSequence.OnComplete(() => text.SetActive(false));
        textSequence.Play();
    }

    private void RefreshShopDescription()
    {
        text.GetComponent<TMPro.TextMeshProUGUI>().text = SettigTextsa(pendingEvent) + $"\n{rare} 등급, {itemPrice} AP, {maxItemCount} 개 남음";
    }

    private void ActiveItemPop()
    {
        IPoolable ci = PoolManager.Instance.Pop(coinPrefab.name);
        CoinItemScript coinItem = ci.GameObject.GetComponent<CoinItemScript>();

        ci.GameObject.transform.position = (Vector2)transform.position + Vector2.up;

        var eventHolder = ci.GameObject.GetComponent<EntityEventHolder>();
        eventHolder.pendingEvents = new List<PendingEvent>();
        eventHolder.pendingEvents.Add(pendingEvent);

        Rigidbody2D rb = coinItem.rb;
        Vector2 force = new Vector2(UnityEngine.Random.Range(-2f, 2f) * rb.mass, UnityEngine.Random.Range(1f, 2f)) * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);

        IPoolable statEffect = PoolManager.Instance.Pop("ShopEffect");
        if (statEffect.GameObject != null)
        {
            statEffect.GameObject.transform.position = transform.position;
            statEffect.GameObject.GetComponent<Effect>().SetColor(Color.white);
            statEffect.GameObject.SetActive(true);
        }
        maxItemCount--;
        if (maxItemCount <= 0)
        {
            Destroy(gameObject);
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
}
