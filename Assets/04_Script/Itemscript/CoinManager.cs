using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject ArrowPointPrefab;
    public static CoinManager Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator DeadCoinSplit(Entity target, float coinsplitTime)
    {
        Vector2 tp = target.transform.position;
        int totalGold = target.statObject.Gold;
        if (totalGold <= 0) yield break;

        WaitForSeconds sa = new WaitForSeconds(coinsplitTime);
        StatType statType = StatType.Gold;
        Color coinColor = GetColorByStatType(statType);


        while (totalGold > 0)
        {
            int dropAmount = Mathf.Min(Random.Range(50, 100), totalGold);
            totalGold -= dropAmount;

            SpawnCoin(tp, statType, dropAmount, coinColor);
            yield return sa;
        }

        target.statObject.Gold = 0;
    }

    private void SpawnCoin(Vector2 position, StatType statType, int amount, Color color)
    {
        IPoolable ci = PoolManager.Instance.Pop("Coin");
        CoinItemScript coinItem = ci.GameObject.GetComponent<CoinItemScript>();



        ci.GameObject.transform.position = position + Vector2.up;

        Rigidbody2D rb = coinItem.rb;
        Vector2 force = new Vector2(Random.Range(-2f, 2f) * rb.mass, Random.Range(1f, 2f)) * rb.mass;
        rb.AddForce(force, ForceMode2D.Impulse);
        bool merged = false;
        coinItem.gameObject.GetComponent<SpriteRenderer>().color = GetColorByStatType(StatType.Gold);
        for (int i = 0; i < coinItem.statIncreases.Count; i++)
        {
            if (coinItem.statIncreases[i].stat == statType)
            {
                var info = coinItem.statIncreases[i];
                info.amount += amount;
                coinItem.statIncreases[i] = info;
                merged = true;
                coinItem.gameObject.GetComponent<SpriteRenderer>().color = GetColorByStatType(StatType.Gold);
                break;
            }
        }

        if (!merged)
        {
            coinItem.statIncreases.Add(new StatIncreaseInfo
            {
                stat = statType,
                amount = amount,
                color = color
            });
        }
    }

    private Color GetColorByStatType(StatType statType)
    {
        return statType switch
        {
            // 체력 계열 - 초록색 계열
            StatType.MaxHealth => new Color(0.2f, 1f, 0.2f),
            StatType.HealthRecoverPower => new Color(0.5f, 1f, 0.5f),
            StatType.CurrentHealth => new Color(0.4f, 1f, 0.4f),

            // 스태미나 계열 - 연노란색 계열
            StatType.MaxStamina => new Color(1f, 1f, 0.6f),
            StatType.StaminaRecoverPower => new Color(1f, 0.95f, 0.6f),
            StatType.CurrentStamina => new Color(1f, 0.95f, 0.6f),

            // 마나 계열 - 파란색 계열
            StatType.MaxMana => new Color(0.4f, 0.6f, 1f),
            StatType.ManaRecoverPower => new Color(0.6f, 0.7f, 1f),
            StatType.CurrentMana => new Color(0.5f, 0.8f, 1f),

            // 쉴드 - 민트색/하늘색 계열
            StatType.CurrentShield => new Color(0.6f, 1f, 1f),

            // 공격력 - 오렌지
            StatType.AttackPower => new Color(1f, 0.5f, 0f),
            StatType.AttackPowerMultiplier => new Color(1f, 0.7f, 0.3f),

            // 공격속도 - 연한 민트색
            StatType.AttackSpeed => new Color(0.6f, 1f, 0.9f),

            // 크리티컬 - 연두형 초록색
            StatType.CriticalChance => new Color(0.7f, 1f, 0.4f),
            StatType.CriticalDamageMultiplier => new Color(0.9f, 1f, 0.5f),
            StatType.CriticalGardPenetration => new Color(0.8f, 1f, 0.6f),

            // 넉백 저항 - 보라색
            StatType.KnockbackResistance => new Color(0.7f, 0.4f, 1f),
            StatType.Defense => new Color(0.6f, 0.6f, 0.6f),
            StatType.DefGardLevel => new Color(0.5f, 0.5f, 0.5f),

            // 이동속도 - 연한 분홍색
            StatType.MoveSpeed => new Color(1f, 0.7f, 0.9f),

            // 점프력 - 연보라색
            StatType.JumpForce => new Color(0.8f, 0.7f, 1f),
            StatType.MaxJumpCount => new Color(0.9f, 0.7f, 1f),

            // 쿨다운 - 회색 청록빛
            StatType.CooldownTime => new Color(0.5f, 0.8f, 0.8f),

            // 골드 - 금색
            StatType.Gold => new Color(1f, 0.84f, 0f),

            // 애로우 포인트 - 핑크색
            StatType.ArrowPoint => new Color(1f, 0.5f, 0.8f),

            // 기본
            _ => Color.white
        };
    }
}
