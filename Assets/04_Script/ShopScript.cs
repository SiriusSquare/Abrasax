
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public enum ShopMode
{
    Default = 0,
    Randomized = 1,
    BulkSale = 2
}

public class ShopScript : MonoBehaviour
{


    private string decurationName = "";
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject ShopIcon;
    [SerializeField] private GameObject text;
    private CanvasGroup textCanvasGroup;
    private Vector3 textOriginalPos;
    [SerializeField] private bool isRandom;
    [SerializeField] private int maxItemCount = 10;
    [SerializeField] private StatType currentStatType;
    private Color color;
    [SerializeField] private bool isRandomMaxCount = false;
    [SerializeField] private ShopMode shopMode = ShopMode.Default;
    private System.Random random = new System.Random();
    [SerializeField] private int itemPrice = 0;
    private void Awake()
    {
        if (text != null)
        {
            textCanvasGroup = text.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                Debug.LogError("주시하는 자는 너를 바라볼 것이다. 너는 ");
            }
            textOriginalPos = text.transform.localPosition;

            textCanvasGroup.alpha = 0f;
            text.transform.localPosition = textOriginalPos;
        }

        if (isRandom)
        {
            currentStatType = GetRandomRoomPrefab();
            color = GetColorByStatType(currentStatType);
        }

        if (isRandomMaxCount)
        {
            maxItemCount = GetRandomMaxItemCount(currentStatType);
        }
        ShopIcon.GetComponent<SpriteRenderer>().color = GetColorByStatType(currentStatType);
        switch (shopMode)
        {
            case ShopMode.Randomized:
                maxItemCount = random.Next(1,8); // 1~3
                currentStatType = GetRandomRoomPrefab();
                itemPrice = UnityEngine.Random.Range(600, 2001);
                ShopIcon.GetComponent<SpriteRenderer>().color = Color.black;
                break;

            case ShopMode.BulkSale:
                maxItemCount = UnityEngine.Random.Range(5, 11);
                currentStatType = GetCommonStatType();
                itemPrice = Mathf.FloorToInt(GetPriceWithMode(currentStatType) * maxItemCount * 0.7f);
                ShopIcon.GetComponent<SpriteRenderer>().color = GetColorByStatType(currentStatType);
                break;

            default:
                itemPrice = GetPriceWithMode(currentStatType);
                break;
        }


        RefreshShopDescription();

        
    }
    public int GetPriceWithMode(StatType stat)
    {
        int basePrice = GetItemamountGold(stat);
        return shopMode switch
        {
            ShopMode.BulkSale => Mathf.RoundToInt(basePrice * 0.8f),
            ShopMode.Randomized => UnityEngine.Random.Range(basePrice / 2, basePrice * 2),
            _ => basePrice,
        };
    }
    private float nextBuyTime = 0f;

    private void Update()
    {
        if (nextBuyTime > 0f)
        {
            nextBuyTime -= Time.deltaTime;
        }
    }
    private int GetRandomMaxItemCount(StatType stat)
    {
        return stat switch
        {
            StatType.CurrentHealth => UnityEngine.Random.Range(8, 15),
            StatType.MaxHealth => UnityEngine.Random.Range(7, 12),
            StatType.HealthRecoverPower => UnityEngine.Random.Range(6, 12),

            StatType.AttackPower => UnityEngine.Random.Range(8, 15),
            StatType.AttackSpeed => UnityEngine.Random.Range(8, 15),
            StatType.CriticalChance => UnityEngine.Random.Range(6, 12),
            StatType.CriticalDamageMultiplier => UnityEngine.Random.Range(3, 7),

            StatType.MaxStamina => UnityEngine.Random.Range(6, 10),
            StatType.StaminaRecoverPower => UnityEngine.Random.Range(6, 10),
            StatType.MaxMana => UnityEngine.Random.Range(6, 10),
            StatType.ManaRecoverPower => UnityEngine.Random.Range(6, 10),

            StatType.CurrentShield => UnityEngine.Random.Range(4, 8),
            StatType.KnockbackResistance => UnityEngine.Random.Range(3, 6),
            StatType.CooldownTime => UnityEngine.Random.Range(3, 6),
            StatType.MoveSpeed => UnityEngine.Random.Range(4, 7),
            StatType.MaxJumpCount => 1,

            StatType.Defense => UnityEngine.Random.Range(4, 8),
            StatType.AttackPowerMultiplier => UnityEngine.Random.Range(1, 3),

            // 기본값
            _ => UnityEngine.Random.Range(4, 10),
        };
    }


    private Color GetColorByStatType(StatType statType)
    {
        return statType switch
        {

            StatType.MaxHealth => new Color(0.2f, 1f, 0.2f),
            StatType.HealthRecoverPower => new Color(0.5f, 1f, 0.5f),
            StatType.CurrentHealth => new Color(0.4f, 1f, 0.4f),


            StatType.MaxStamina => new Color(1f, 1f, 0.6f),
            StatType.StaminaRecoverPower => new Color(1f, 0.95f, 0.6f),
            StatType.CurrentStamina => new Color(1f, 0.95f, 0.6f),


            StatType.MaxMana => new Color(0.4f, 0.6f, 1f),
            StatType.ManaRecoverPower => new Color(0.6f, 0.7f, 1f),
            StatType.CurrentMana => new Color(0.5f, 0.8f, 1f),


            StatType.CurrentShield => new Color(0.6f, 1f, 1f),


            StatType.AttackPower => new Color(1f, 0.5f, 0f),
            StatType.AttackPowerMultiplier => new Color(1f, 0.7f, 0.3f),


            StatType.AttackSpeed => new Color(0.6f, 1f, 0.9f),


            StatType.CriticalChance => new Color(0.7f, 1f, 0.4f),
            StatType.CriticalDamageMultiplier => new Color(0.9f, 1f, 0.5f),
            StatType.CriticalGardPenetration => new Color(0.8f, 1f, 0.6f),


            StatType.KnockbackResistance => new Color(0.7f, 0.4f, 1f),
            StatType.Defense => new Color(0.6f, 0.6f, 0.6f),
            StatType.DefGardLevel => new Color(0.5f, 0.5f, 0.5f),


            StatType.MoveSpeed => new Color(1f, 0.7f, 0.9f),


            StatType.JumpForce => new Color(0.8f, 0.7f, 1f),
            StatType.MaxJumpCount => new Color(0.9f, 0.7f, 1f),


            StatType.CooldownTime => new Color(0.5f, 0.8f, 0.8f),


            StatType.Gold => new Color(1f, 0.84f, 0f),


            StatType.ArrowPoint => new Color(1f, 0.5f, 0.8f),


            _ => Color.white
        };
    }
    private bool isTextVisible = false;
    private Sequence textSequence;
    private void ShowElevatorText(string message)
    {
        if (text == null || textCanvasGroup == null) return;
        if (isTextVisible) return;

        isTextVisible = true;


        textSequence?.Kill(true);

        text.SetActive(true);

        var textComponentTMP = text.GetComponent<TMPro.TextMeshProUGUI>();
        if (textComponentTMP != null)
            textComponentTMP.text = message;
        else
        {
            var textComponent = text.GetComponent<UnityEngine.UI.Text>();
            if (textComponent != null)
                textComponent.text = message;
        }

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
        if (!isTextVisible) return;

        isTextVisible = false;


        textSequence?.Kill(true);

        textSequence = DOTween.Sequence();
        textSequence.Append(text.transform.DOLocalMoveY(textOriginalPos.y - 2f, 0.25f).SetEase(Ease.InCubic));
        textSequence.Join(textCanvasGroup.DOFade(0f, 0.25f));
        textSequence.OnComplete(() => text.SetActive(false));
        textSequence.Play();
    }

    public StatType GetRandomRoomPrefab()
    {
        float i = UnityEngine.Random.Range(0f, 100f);
        StatType stap = StatType.CurrentHealth;

        if (i < 6f)
            stap = StatType.CurrentHealth;
        else if (i < 10f)
            stap = StatType.CurrentShield;
        else if (i < 14f)
            stap = StatType.MaxHealth;
        else if (i < 17.5f)
            stap = StatType.MaxStamina;
        else if (i < 21f)
            stap = StatType.MaxMana;

        else if (i < 28f)
            stap = StatType.HealthRecoverPower;
        else if (i < 35f)
            stap = StatType.StaminaRecoverPower;
        else if (i < 42f)
            stap = StatType.ManaRecoverPower;

        else if (i < 56f)
            stap = StatType.AttackPower;
        else if (i < 62f)
            stap = StatType.AttackPowerMultiplier;
        else if (i < 74f)
            stap = StatType.AttackSpeed;
        else if (i < 81f)
            stap = StatType.CriticalChance;
        else if (i < 88f)
            stap = StatType.CriticalDamageMultiplier;

        else if (i < 93f)
            stap = StatType.CooldownTime;
        else if (i < 96f)
            stap = StatType.MoveSpeed;
        else if (i < 98f)
            stap = StatType.KnockbackResistance;
        else
            stap = StatType.MaxJumpCount;

        return stap;
    }



    public float GetItemamount(StatType stat)
    {
        return stat switch
        {

            StatType.MaxHealth => 5f,
            StatType.HealthRecoverPower => 0.1f,
            StatType.CurrentHealth => 5f,
            StatType.MaxStamina => 5f,
            StatType.StaminaRecoverPower => 0.1f,
            StatType.CurrentStamina => 5f,
            StatType.MaxMana => 5f,
            StatType.ManaRecoverPower => 0.1f,
            StatType.CurrentMana => 5f,
            StatType.CurrentShield => 10f,
            StatType.AttackPower => 1f,
            StatType.AttackPowerMultiplier => 0.05f,
            StatType.AttackSpeed => 0.08f,
            StatType.CriticalChance => 1f,
            StatType.CriticalDamageMultiplier => 0.1f,
            StatType.CriticalGardPenetration => 1,
            StatType.KnockbackResistance => 0.13f,
            StatType.Defense => 1f,
            StatType.DefGardLevel => 1,
            StatType.MoveSpeed => 1f,
            StatType.JumpForce => 1f,
            StatType.MaxJumpCount => 1f,
            StatType.CooldownTime => 0.05f,
        };
    }
    public int GetItemamountGold(StatType stat)
    {
        return stat switch
        {
            StatType.MaxHealth => 250,
            StatType.HealthRecoverPower => 350,
            StatType.CurrentHealth => 120,
            StatType.MaxStamina => 250,
            StatType.StaminaRecoverPower => 350, 
            StatType.MaxMana => 250,
            StatType.ManaRecoverPower => 350,
            StatType.CurrentMana => 120,
            StatType.CurrentShield => 120,
            StatType.AttackPower => 250,
            StatType.AttackPowerMultiplier => 500,
            StatType.AttackSpeed => 300,
            StatType.CriticalChance => 350,
            StatType.CriticalDamageMultiplier => 400,
            StatType.CriticalGardPenetration => 400,
            StatType.KnockbackResistance => 300,
            StatType.Defense => 300,
            StatType.DefGardLevel => 400,
            StatType.MoveSpeed => 300,
            StatType.JumpForce => 350,
            StatType.MaxJumpCount => 1000,
            StatType.CooldownTime => 1000,

            StatType.Gold => 1,
            StatType.ArrowPoint => 150,

            _ => 150,
        };
    }


    public string GetItemNameKorean(StatType stat)
    {
        return stat switch
        {

            StatType.MaxHealth => "최대 체력",
            StatType.HealthRecoverPower => "체력 회복력",
            StatType.CurrentHealth => "체력",
            StatType.MaxStamina => "최대 스태미나",
            StatType.StaminaRecoverPower => "스태미나 회복력",
            StatType.CurrentStamina => "스태미나",
            StatType.MaxMana => "최대 마나",
            StatType.ManaRecoverPower => "마나 회복력",
            StatType.CurrentMana => "마나",
            StatType.CurrentShield => "쉴드",
            StatType.AttackPower => "공격력",
            StatType.AttackPowerMultiplier => "공격 배수",
            StatType.AttackSpeed => "공격 속도",
            StatType.CriticalChance => "치명타 확률",
            StatType.CriticalDamageMultiplier => "치명타 피해량",
            StatType.CriticalGardPenetration => "치명타 방어 무시",
            StatType.KnockbackResistance => "넉백 저항",
            StatType.Defense => "방어력",
            StatType.DefGardLevel => "방어시 가드 레벨",
            StatType.MoveSpeed => "이동 속도",
            StatType.JumpForce => "점프력",
            StatType.MaxJumpCount => "최대 점프 횟수",
            StatType.CooldownTime => "스킬 쿨타임 회전률",
            StatType.Gold => "골드",
            StatType.ArrowPoint => "애로우 포인트",
        };
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTextVisible)
        {
            ShowElevatorText(decurationName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isTextVisible)
        {
            HideElevatorText();
        }
    }
    private StatType GetCommonStatType()
    {
        StatType[] commonStats = new StatType[]
        {
        StatType.CurrentHealth, StatType.CurrentShield, StatType.CriticalChance,
        StatType.CriticalDamageMultiplier,StatType.AttackSpeed,
        };

        return commonStats[UnityEngine.Random.Range(0, commonStats.Length)];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {


        if (Keyboard.current.upArrowKey.isPressed && nextBuyTime <= 0f && collision.CompareTag("Player"))
        {

            Entity entity = collision.GetComponent<Entity>();
            int price = GetPriceWithMode(currentStatType);

            if (shopMode == ShopMode.BulkSale)
            {
                int totalPrice = Mathf.FloorToInt(price * maxItemCount * 0.7f);
                if (entity.statObject.Gold < totalPrice)
                {

                    return;
                }

                entity.statObject.Gold -= totalPrice;

                for (int i = 0; i < maxItemCount; i++)
                {
                    ActiveItemPop(entity);
                }

                Destroy(gameObject);
                return;
            }

            if (entity.statObject.Gold < price)
            {
                Debug.Log("돈이 부족합니다!");
                return;
            }

            entity.statObject.Gold -= price;
            ActiveItemPop(entity);
            maxItemCount--;
            nextBuyTime = 0.12f;

            RefreshShopDescription();

            if (shopMode == ShopMode.Randomized)
            {
                currentStatType = GetRandomRoomPrefab();
                ShopIcon.GetComponent<SpriteRenderer>().color = Color.black;
            }
            else
            {
                ShopIcon.GetComponent<SpriteRenderer>().color = GetColorByStatType(currentStatType);
            }

            if (maxItemCount <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    private string GetShopDescription()
    {
        string statName = GetItemNameKorean(currentStatType);
        float amount = GetItemamount(currentStatType);
        int price = GetPriceWithMode(currentStatType);

        switch (shopMode)
        {
            case ShopMode.Randomized:
                return
                    $"【???】\n" +
                    $"??? 증가\n" +
                    $"비용: ???G ({maxItemCount}개 남음)\n";

            case ShopMode.BulkSale:
                return
                    $"【{statName} 특가】\n" +
                    $"{statName} +{amount} 증가\n" +
                    $"특가: {price}G ({maxItemCount}개 남음)\n";

            default:
                return
                    $"【{statName} 강화】\n" +
                    $"{statName} +{amount} 증가\n" +
                    $"비용: {price}G ({maxItemCount}개 남음)\n";
        }
    }

    private void RefreshShopDescription()
    {
        decurationName = GetShopDescription();
        text.GetComponent<TMPro.TextMeshProUGUI>().text = decurationName;
    }
    private void ActiveItemPop(Entity entity)
    {
        IPoolable ci = PoolManager.Instance.Pop(coinPrefab.name);
        CoinItemScript coinItem = ci.GameObject.GetComponent<CoinItemScript>();
        ci.GameObject.transform.position = (Vector2)transform.position + Vector2.up;

        Rigidbody2D rb = coinItem.rb;
        Vector2 force = new Vector2(UnityEngine.Random.Range(-2f, 2f) * rb.mass, UnityEngine.Random.Range(1f, 2f)) * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);

        bool merged = false;
        ci.GameObject.GetComponent<SpriteRenderer>().color = GetColorByStatType(currentStatType);

        IPoolable statEffect = PoolManager.Instance.Pop("ShopEffect");
        if (statEffect.GameObject != null)
        {
            statEffect.GameObject.transform.position = transform.position;
            statEffect.GameObject.GetComponent<Effect>().SetColor(Color.white);
            statEffect.GameObject.SetActive(true);
        }

        float actualAmount = 0f;
        StatType finalStat = currentStatType;

        switch (shopMode)
        {
            case ShopMode.Randomized:
                actualAmount = UnityEngine.Random.Range(1, 4);
                finalStat = GetRandomRoomPrefab();
                break;

            case ShopMode.BulkSale:
                actualAmount = GetItemamount(currentStatType);
                break;

            default:
                actualAmount = GetItemamount(currentStatType);
                break;
        }

        for (int i = 0; i < coinItem.statIncreases.Count; i++)
        {
            if (coinItem.statIncreases[i].stat == finalStat)
            {
                var info = coinItem.statIncreases[i];
                info.amount += actualAmount;
                coinItem.statIncreases[i] = info;
                merged = true;
                break;
            }
        }

        if (!merged)
        {
            coinItem.statIncreases.Add(new StatIncreaseInfo
            {
                stat = finalStat,
                amount = actualAmount,
                color = GetColorByStatType(finalStat)
            });
        }
    }



}
