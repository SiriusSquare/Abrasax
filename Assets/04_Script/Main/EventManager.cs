using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SecondaryCondition;
#region Serializable Event Structures

[Serializable]
public class EventCondition
{
    public enum ConditionType
    {
        HealthAbove, HealthBelow,
        StaminaAbove,
        ManaAbove,
        FloorAbove, FloorBetween, FloorBelow,
 Always
    }

    public ConditionType conditionType;
    public float threshold;
    public float multiplier = 1f;

}

[Serializable]
public class SecondaryCondition
{
    public enum SecondaryConditionType
    {
        None, RandomChance,
         PlayerDamageAbove,
         PlayerHitDamageAbove
    }

    public SecondaryConditionType conditionType;
    public List<float> parameters = new(2);
}

[Serializable]
public class EventEffect
{
    public enum EffectType
    {
        Bleed, Stun, Burn, Frost, Drench, Poison, Shock,
        AttackUp, DefenseUp, Shield,
        Impact, Explosion, HeatWave, Vampirism, Wave
    }

    public EffectType effectType;
    public List<float> parameters = new(5);
}

[Serializable]
public class CustomEventData
{
    public Entity target;
    public Transform source;
    public List<EventCondition> conditions = new();
    public List<SecondaryCondition> secondaryConditions = new();
    public List<EventEffect> effects = new();
    public float recentDamageDealt;
    public float recentDamageTaken;
}

#endregion

public enum GameEventType
{

    PlayerAttack,
    PlayerAttackHit,
    PlayerBasicAttack,
    PlayerBasicAttackHit,
    PlayerPierce,
    PlayerPierceHit,
    PlayerTriSlash,
    PlayerTriSlashHit,
    PlayerUppercut,
    PlayerUppercutHit,
    PlayerSpinSlash,
    PlayerSpinSlashHit,
    PlayerCounter,
    PlayerCounterHit,
    PlayerHorizontal,
    PlayerHorizontalHit,
    EquinoxMultiPierce,
    EquinoxMultiPierceHit,
    EquinoxMultiCut,
    EquinoxMultiCutHit,
    EquinoxBigCut,
    EquinoxBigCutHit,

    PlayerGuard,
    PlayerHit,
}


public class EventManager : MonoBehaviour
{
    [SerializeField] private GameObject impecteffect;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject magiceffect;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    public static EventManager Instance { get; private set; }

    private static readonly HashSet<EventEffect.EffectType> IgnoreInVectorEffectTypes = new()
    {
        EventEffect.EffectType.Bleed, EventEffect.EffectType.Stun, EventEffect.EffectType.Burn,
        EventEffect.EffectType.Frost, EventEffect.EffectType.Drench,
        EventEffect.EffectType.Poison, EventEffect.EffectType.Shock,
        EventEffect.EffectType.AttackUp, EventEffect.EffectType.DefenseUp,
        EventEffect.EffectType.Shield, EventEffect.EffectType.Impact
    };

    private static readonly HashSet<SecondaryCondition.SecondaryConditionType> IgnoreInVectorSecondaryTypes = new()
    {

        SecondaryCondition.SecondaryConditionType.PlayerDamageAbove,

        SecondaryCondition.SecondaryConditionType.PlayerHitDamageAbove
    };

    public static readonly Dictionary<GameEventType, float> GameEventMultipliers = new()
    {
        { GameEventType.PlayerAttack, 0.25f },
        { GameEventType.PlayerAttackHit, 0.15f },
        { GameEventType.PlayerBasicAttack, 1f },
        { GameEventType.PlayerBasicAttackHit, 0.5f },
        { GameEventType.PlayerPierce, 1.5f },
        { GameEventType.PlayerPierceHit, 0.75f },
        { GameEventType.PlayerTriSlash, 2f },
        { GameEventType.PlayerTriSlashHit, 1f },
        { GameEventType.PlayerUppercut, 3f },
        { GameEventType.PlayerUppercutHit, 1.5f },
        { GameEventType.PlayerSpinSlash, 4f },
        { GameEventType.PlayerSpinSlashHit, 0.25f },
        { GameEventType.PlayerCounter, 5f },
        { GameEventType.PlayerCounterHit, 1f },
        { GameEventType.PlayerHorizontal, 2f },
        { GameEventType.PlayerHorizontalHit, 1f },
        { GameEventType.EquinoxMultiPierce, 5f },
        { GameEventType.EquinoxMultiPierceHit, 2f },
        { GameEventType.EquinoxMultiCut, 7f },
        { GameEventType.EquinoxMultiCutHit, 1f },
        { GameEventType.EquinoxBigCut, 10f },
        { GameEventType.EquinoxBigCutHit, 3f },
        { GameEventType.PlayerGuard, 1f },
        { GameEventType.PlayerHit, 1f }
    };
    public static readonly Dictionary<EventCondition.ConditionType, string[]> GameConditionNames = new()
{
    { EventCondition.ConditionType.HealthBelow, new[] { "ü����", "% ������ �� " } },
    { EventCondition.ConditionType.HealthAbove, new[] { "ü����", "% �̻��� �� " } },

    { EventCondition.ConditionType.StaminaAbove, new[] { "���¹̳���", "% �̻��� �� " } },

    { EventCondition.ConditionType.ManaAbove, new[] { "������", "% �̻��� �� " } },
    { EventCondition.ConditionType.FloorBelow, new[] { "�÷ξ", "�� ������ �� " } },
    { EventCondition.ConditionType.FloorAbove, new[] { "�÷ξ", "�� �̻��� �� " } },
    { EventCondition.ConditionType.FloorBetween, new[] { "�÷ξ 10�� ����� �� ", } },
    { EventCondition.ConditionType.Always, new[] { "�׻� " } }
};
    public static readonly Dictionary<SecondaryCondition.SecondaryConditionType, string[]> GameSecondaryConditionNames = new()
{
    { SecondaryCondition.SecondaryConditionType.RandomChance, new[] { "Ȯ���� " } },
        { SecondaryCondition.SecondaryConditionType.PlayerDamageAbove, new[] { "��������"," �̻��� �� " } },
        { SecondaryCondition.SecondaryConditionType.PlayerHitDamageAbove, new[] { "��������", " �̻��� �� " } },

    };
    public static readonly Dictionary<GameEventType, string> GameEventNames = new()
{
    { GameEventType.PlayerAttack, "�÷��̾� ���� �� " },
    { GameEventType.PlayerAttackHit, "�÷��̾� ���� ���� �� " },
    { GameEventType.PlayerBasicAttack, "�÷��̾� �⺻ ���� ��� �� " },
    { GameEventType.PlayerBasicAttackHit, "�÷��̾� �⺻ ���� ���� �� " },
    { GameEventType.PlayerPierce, "��� ���� �� " },
    { GameEventType.PlayerPierceHit, "��� ���� �� " },
    { GameEventType.PlayerTriSlash, "Ʈ���̽����� ��� �� " },
    { GameEventType.PlayerTriSlashHit, "Ʈ���̽����� ���� �� " },
    { GameEventType.PlayerUppercut, "�÷�ġ�� ��� �� " },
    { GameEventType.PlayerUppercutHit, "�÷�ġ�� ���� �� " },
    { GameEventType.PlayerSpinSlash, "ȸ������ ��� �� " },
    { GameEventType.PlayerSpinSlashHit, "ȸ������ ���� �� " },
    { GameEventType.PlayerCounter, "������̵� ��� �� " },
    { GameEventType.PlayerCounterHit, "������̵� ���� �� " },
    { GameEventType.PlayerHorizontal, "���κ��� ��� �� " },
    { GameEventType.PlayerHorizontalHit, "���ι�� ���� �� " },
    { GameEventType.EquinoxMultiPierce, "�����콺 �������� ��� �� " },
    { GameEventType.EquinoxMultiPierceHit, "�����콺 �������� ���� �� "  },
    { GameEventType.EquinoxMultiCut, "�����콺 ���ܳ��� ��� �� " },
    { GameEventType.EquinoxMultiCutHit, "�����콺 ���ܳ��� ���� �� " },
    { GameEventType.EquinoxBigCut, "�����콺 ������ ��� �� " },
    { GameEventType.EquinoxBigCutHit, "�����콺 ������ ���� �� " },
    { GameEventType.PlayerGuard, "�÷��̾� ���� �� " },
    { GameEventType.PlayerHit, "�÷��̾� �ǰ� �� " }
};
    public static readonly Dictionary<EventEffect.EffectType, string[]> EffectTypeNames = new()
{
        {EventEffect.EffectType.Bleed, new[] { "���ø�ŭ ���� �ΰ� ", " "}},
        {EventEffect.EffectType.Stun, new[] { "���ø�ŭ ���� �ΰ� ", " "}},
        {EventEffect.EffectType.Burn, new[] { "���ø�ŭ ȭ�� �ΰ� ", " "}},
        {EventEffect.EffectType.Frost, new[] { "���ø�ŭ ���� �ΰ� ", " "}},
        {EventEffect.EffectType.Drench, new[] { "���ø�ŭ ħ�� �ΰ� ", " " }},
        {EventEffect.EffectType.Poison, new[] { "���ø�ŭ �� �ΰ� ", ""}},
        {EventEffect.EffectType.Shock, new[] { "���ø�ŭ ���� �ΰ� ", ""}},
         {EventEffect.EffectType.AttackUp, new[] {"���� ���ݷ� ���� ", ""}},
        {EventEffect.EffectType.DefenseUp, new[] {"���� ���� ���� ", ""}},
        {EventEffect.EffectType.Shield, new[] {"���� ���� ���� ", ""}},
        {EventEffect.EffectType.Impact, new[] {"��� �߻� ", "���ط��� ", "������ "}},
        {EventEffect.EffectType.Explosion, new[] {"���� �߻� ", "���ط��� ", "�������� "}},
        {EventEffect.EffectType.Wave, new[] {"�ĵ� �߻� ", "���ط��� ", "���� ���� ", "���� Ƚ�� "}},
        {EventEffect.EffectType.HeatWave, new[] {"���� �߻� ", "���ط��� ", "���� �ð� ", "���� ���� ���� "}},
        {EventEffect.EffectType.Vampirism, new[] { "%��ŭ ���� ", ""}}

};

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void EventMain(Entity owner, Entity target, EventCondition ec, SecondaryCondition[] sec, EventEffect effect, Statuseffect[] se, GameEventType eventType)
    {
        Debug.Log($"�̹�Ʈ ����> {owner},{target},{ec},{sec[0]},{effect},{eventType}");
        if (!CheckPrimaryCondition(owner, ec, se)) return;
        float multiplier = GameEventMultipliers.TryGetValue(eventType, out var baseMultiplier)
                        ? baseMultiplier
                        : 1f;
        Debug.Log($"�̹�Ʈ ����> {owner},{target},{ec},{sec[0]},{effect},{eventType}");
        if (!CheckSecondaryCondition(owner, target, sec[0])) return;
        ApplyEffectWithMultiplier(owner, target, effect, multiplier, sec[0]);
    }

    public void EventMain(Entity owner, Vector2 targetPos, EventCondition ec, SecondaryCondition[] sec, EventEffect effect, Statuseffect[] se, GameEventType eventType)
    {
        Debug.Log($"�̹�Ʈ ����> {owner},{targetPos},{ec},{sec[0]},{effect},{eventType}");
        if (!CheckPrimaryCondition(owner, ec, se)) return;
        float multiplier = GameEventMultipliers.TryGetValue(eventType, out var baseMultiplier)
                        ? baseMultiplier
                        : 1f;
        Debug.Log($"�̹�Ʈ ����> {owner},{targetPos},{ec},{sec[0]},{effect},{eventType}");
        if (!CheckSecondaryCondition(owner, sec[0])) return;
        ApplyEffectWithMultiplier(owner, targetPos, effect, multiplier);

    }

    private bool CheckPrimaryCondition(Entity entity, EventCondition ec, Statuseffect[] se)
    {
        Debug.Log($"CheckPrimaryCondition�̹�Ʈ ����> {entity},{ec}");
        return ec.conditionType switch
        {
            EventCondition.ConditionType.HealthAbove => (entity.Health / entity.MaxHealth) * 100 > ec.threshold,
            EventCondition.ConditionType.HealthBelow => (entity.Health / entity.MaxHealth) * 100 < ec.threshold,
            EventCondition.ConditionType.StaminaAbove => (entity.Stamina / entity.MaxStamina) * 100 > ec.threshold,

            EventCondition.ConditionType.ManaAbove => (entity.Mana / entity.MaxMana) * 100 > ec.threshold,

            EventCondition.ConditionType.FloorAbove => StageManager.Instance.Floor > ec.threshold,
            EventCondition.ConditionType.FloorBelow => StageManager.Instance.Floor < ec.threshold,
            EventCondition.ConditionType.FloorBetween => StageManager.Instance.Floor % 10 == 0,
            EventCondition.ConditionType.Always => true,
            _ => false,
        };
    }

    private bool CheckSecondaryCondition(Entity owner, Entity target, SecondaryCondition sc)
    {
        Debug.Log($"CheckSecondaryCondition�̹�Ʈ ����> {owner},{target},{sc}");
        return sc.conditionType switch
        {
            SecondaryCondition.SecondaryConditionType.None => true,
            SecondaryCondition.SecondaryConditionType.RandomChance => UnityEngine.Random.Range(0f,100f) <= (sc.parameters.Count > 0 ? sc.parameters[0] : 1f),

            SecondaryCondition.SecondaryConditionType.PlayerDamageAbove =>
                owner != null && sc.parameters.Count > 0 && sc.parameters[1] > sc.parameters[0],

            SecondaryCondition.SecondaryConditionType.PlayerHitDamageAbove =>
                owner != null && target != null && sc.parameters.Count > 0 && sc.parameters[1] > sc.parameters[0],
            _ => true,
        };
    }
    private bool CheckSecondaryCondition(Entity owner, SecondaryCondition sc)
    {
        return sc.conditionType switch
        {
            SecondaryCondition.SecondaryConditionType.None => true,
            SecondaryCondition.SecondaryConditionType.RandomChance => UnityEngine.Random.Range(0f, 100f) <= (sc.parameters.Count > 0 ? sc.parameters[0] : 1f),
            _ => true,
        };
    }

    public void ApplyEffectWithMultiplier(Entity owner, Entity target, EventEffect effect, float multiplier, SecondaryCondition sc)
    {
        Debug.Log($"����Ʈ �ο�,{owner},{target},{effect},{multiplier}");
        switch (effect.effectType)
        {
            case EventEffect.EffectType.Bleed:
                {
                    target.entityStatusManager.ApplyEffect(StatusEffectType.Bleed,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Stun:
                {
                    target.entityStatusManager.ApplyEffect(StatusEffectType.Stun,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Frost:
                {
                    target.entityStatusManager.ApplyEffect(StatusEffectType.Frost,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Burn:
                {
                    target.entityStatusManager.ApplyEffect(StatusEffectType.Burn,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Drench:
                {
                    target.entityStatusManager.ApplyEffect(StatusEffectType.Drench,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Poison:
                {
                    target.entityStatusManager.ApplyEffect(StatusEffectType.Poison,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Shock:
                {
                    target.entityStatusManager.ApplyEffect(StatusEffectType.Shock,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.AttackUp:
                {
                    owner.entityStatusManager.ApplyEffect(StatusEffectType.AttackUp,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.DefenseUp:
                {
                    owner.entityStatusManager.ApplyEffect(StatusEffectType.DefenseUp,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Shield:
                owner.AddShield((effect.parameters.Count > 0 ? effect.parameters[0] : 50f) * multiplier);
                break;
            case EventEffect.EffectType.Impact:
                float dmg = (effect.parameters.Count > 0 ? effect.parameters[0] * owner.statObject.FinalAttackPower : 10f) * multiplier;
                float stun = (effect.parameters.Count > 1 ? effect.parameters[1] : 1f) * multiplier;
                IPoolable impectEffect = PoolManager.Instance.Pop("ImpectEffect");
                impectEffect.GameObject.transform.position = target.transform.position;
                target.TakeDamage(dmg * owner.statObject.FinalAttackPower * 0.5f, 5, Vector2.up * 5, AttackType.Impect, owner, target.transform, impecteffect);
                target.ApplyStun(stun);
                impulseSource.GenerateImpulse(stun / 3.5f);
                impectEffect.GameObject.SetActive(true);
                break;
            case EventEffect.EffectType.Explosion:
                float hDmga = (effect.parameters.Count > 0 ? effect.parameters[0] * owner.statObject.FinalAttackPower : 5f) * multiplier;
                float hRada = (effect.parameters.Count > 2 ? effect.parameters[1] : 1f);
                IPoolable hb = PoolManager.Instance.Pop("AttackHitBox");

                hb.GameObject.GetComponent<AttackTriggerScript>().ChangeSetting(hDmga,3,new Vector2(10,10),magiceffect,5f,AttackType.Magic,AttackElement.Null,owner,0.5f);
                Vector3 basePosEx = target.transform.position;
                hb.GameObject.transform.position = new Vector3(basePosEx.x, basePosEx.y, basePosEx.z);
                bool isPlayerOwnera = (owner.Faction == "Player");
                string waveKeya = isPlayerOwnera ? "Explosion" : "EnemyExplosion";
                IPoolable meff = PoolManager.Instance.Pop(waveKeya);
                meff.GameObject.transform.position = new Vector3(basePosEx.x, basePosEx.y, basePosEx.z);
                meff.GameObject.transform.localScale = new Vector3(hRada, hRada, hRada);
                hb.GameObject.transform.localScale = new Vector3(hRada, hRada, hRada);
                hb.GameObject.SetActive(true);
                meff.GameObject.SetActive(true);
                impulseSource.GenerateImpulse(hRada / 7);
                break;
            case EventEffect.EffectType.HeatWave:
                {
                    Vector2 spawnPos;
                    if (TryGetGroundPosition((Vector2)target.transform.position + Vector2.up * 1f, out spawnPos))
                    {
                        float hDmg = (effect.parameters.Count > 0 ? effect.parameters[0] * owner.statObject.FinalAttackPower : 0.2f) * multiplier;
                        float hDur = (effect.parameters.Count > 1 ? effect.parameters[1] : 0.2f);
                        float hRange = (effect.parameters.Count > 2 ? effect.parameters[2] : 1f);
                        IPoolable heatWave;
                        bool isPlayerOwner = (owner.Faction == "Player");
                        string waveKey = isPlayerOwner ? "HeatWave" : "EnemyHeatWave";
                        heatWave = PoolManager.Instance.Pop(waveKey);
                        heatWave.GameObject.GetComponent<HeatWave>().OnSetting(hDmg, hDur, owner,magiceffect);
                        heatWave.GameObject.transform.position = spawnPos + new Vector2(UnityEngine.Random.Range(-hRange,hRange),0);
                        
                        heatWave.GameObject.SetActive(true);
                    }
                    break;
                }

            case EventEffect.EffectType.Wave:
                {
                    Vector2 spawnPos;
                    if (TryGetGroundPosition((Vector2)target.transform.position + Vector2.up * 1f, out spawnPos))
                    {
                        float wDmg = (effect.parameters.Count > 0 ? effect.parameters[0]  * owner.statObject.FinalAttackPower : 1f) * multiplier;
                        float wIntervalTime = (effect.parameters.Count > 1 ? effect.parameters[1] : 0.3f);
                        int wCount = (effect.parameters.Count > 2 ? (int)effect.parameters[2] : 5);
                        float wSpeed = (effect.parameters.Count > 3 ? effect.parameters[3] : 1f);

                        StartCoroutine(SpawnWave(owner, spawnPos, wDmg, wIntervalTime, wCount, wSpeed));
                    }
                    break;
                }
            case EventEffect.EffectType.Vampirism:
                float vamp = (sc.parameters[1] / (effect.parameters.Count > 0 ? effect.parameters[0] : 50f) * multiplier);
                owner.Heal(vamp);
                break;

        }
    }
    private bool TryGetGroundPosition(Vector2 origin, out Vector2 groundPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 10f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            groundPos = hit.point;
            return true;
        }
        groundPos = Vector2.zero;
        return false;
    }
    private IEnumerator SpawnWave(Entity owner, Vector2 centerPosition, float damage, float intervalTime, int count, float speed)
    {
        bool isPlayerOwner = (owner.Faction == "Player");
        string waveKey = isPlayerOwner ? "Wave" : "EnemyWave";
        Debug.Log($"SpawnWave ����: {owner}, {centerPosition}, {damage}, {intervalTime}, {count}, {speed}");
        float pluspos = 0f;
        for (int i = 0; i < count; i++)
        {

            
            IPoolable waveLeft = PoolManager.Instance.Pop(waveKey);
            var leftTransform = waveLeft.GameObject.transform;
            leftTransform.position = new Vector3(centerPosition.x - 1.5f - pluspos, centerPosition.y, 0f);
            leftTransform.localScale = new Vector3(1f, 1f, 1f);

            waveLeft.GameObject.GetComponent<AttackTriggerScript>().ChangeSetting(
                damage,
                3,
                new Vector2(1f, 8f),
                magiceffect,
                1,
                AttackType.Magic,
                AttackElement.Null,
                owner,
                0.5f
            );
            waveLeft.GameObject.SetActive(true);

            IPoolable waveRight = PoolManager.Instance.Pop(waveKey);
            var rightTransform = waveRight.GameObject.transform;
            rightTransform.position = new Vector3(centerPosition.x + 1.5f + pluspos, centerPosition.y, 0f);
            rightTransform.localScale = new Vector3(1f, 1f, 1f);

            waveRight.GameObject.GetComponent<AttackTriggerScript>().ChangeSetting(
                damage,
                3,
                new Vector2(1f, 8f),
                magiceffect,
                1,
                AttackType.Magic,
                AttackElement.Null,
                owner,
                0.5f
            );
            waveRight.GameObject.SetActive(true);
            pluspos += 1.5f;
            yield return new WaitForSeconds(intervalTime);
        }
    }


    private int GetFinalStack(List<float> parameters, float multiplier)
    {
        float baseStack = parameters.Count > 0 ? parameters[0] : 1f;
        float finalValue = baseStack * multiplier;

        if (finalValue >= 1f)
        {
            return Mathf.RoundToInt(finalValue);
        }
        else
        {
            return UnityEngine.Random.value < finalValue ? 1 : 0;
        }
    }
    public void ApplyEffectWithMultiplier(Entity owner, Vector2 target, EventEffect effect, float multiplier)
    {
        Debug.Log($"Vector2 - ����Ʈ �ο�,{owner},{target},{effect},{multiplier}");
        switch (effect.effectType)
        {
            case EventEffect.EffectType.AttackUp:
                {
                    owner.entityStatusManager.ApplyEffect(StatusEffectType.AttackUp,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.DefenseUp:
                {
                    owner.entityStatusManager.ApplyEffect(StatusEffectType.DefenseUp,
                        GetFinalStack(effect.parameters, multiplier));
                    break;
                }
            case EventEffect.EffectType.Shield:
                owner.AddShield((effect.parameters.Count > 0 ? effect.parameters[0] : 50f) * multiplier);
                break;

            case EventEffect.EffectType.Explosion:
                float hDmga = (effect.parameters.Count > 0 ? effect.parameters[0] * owner.statObject.FinalAttackPower : 5f) * multiplier;
                float hRada = (effect.parameters.Count > 2 ? effect.parameters[1] : 1f);
                IPoolable hb = PoolManager.Instance.Pop("AttackHitBox");

                hb.GameObject.GetComponent<AttackTriggerScript>().ChangeSetting(hDmga, 3, new Vector2(10, 10), magiceffect, 5f, AttackType.Magic, AttackElement.Null, owner, 0.5f);
                Vector3 basePosEx = target;
                hb.GameObject.transform.position = new Vector3(basePosEx.x, basePosEx.y, basePosEx.z);
                bool isPlayerOwnera = (owner.Faction == "Player");
                string waveKeya = isPlayerOwnera ? "Explosion" : "EnemyExplosion";
                IPoolable meff = PoolManager.Instance.Pop(waveKeya);
                meff.GameObject.transform.position = new Vector3(basePosEx.x, basePosEx.y, basePosEx.z);
                meff.GameObject.transform.localScale = new Vector3(hRada, hRada, hRada);
                hb.GameObject.transform.localScale = new Vector3(hRada, hRada, hRada);
                hb.GameObject.SetActive(true);
                meff.GameObject.SetActive(true);
                impulseSource.GenerateImpulse(hRada / 7);
                break;
            case EventEffect.EffectType.HeatWave:
                {
                    Vector2 spawnPos;
                    if (TryGetGroundPosition(target + Vector2.up * 1f, out spawnPos))
                    {
                        float hDmg = (effect.parameters.Count > 0 ? effect.parameters[0] * owner.statObject.FinalAttackPower : 0.2f) * multiplier;
                        float hDur = (effect.parameters.Count > 1 ? effect.parameters[1] : 0.2f);
                        float hRange = (effect.parameters.Count > 2 ? effect.parameters[2] : 1f);
                        IPoolable heatWave;
                        bool isPlayerOwner = (owner.Faction == "Player");
                        string waveKey = isPlayerOwner ? "HeatWave" : "EnemyHeatWave";
                        heatWave = PoolManager.Instance.Pop(waveKey);
                        heatWave.GameObject.GetComponent<HeatWave>().OnSetting(hDmg, hDur, owner, magiceffect);
                        heatWave.GameObject.transform.position = spawnPos + new Vector2(UnityEngine.Random.Range(-hRange, hRange), 0);

                        heatWave.GameObject.SetActive(true);
                    }
                    break;
                }

            case EventEffect.EffectType.Wave:
                {
                    Vector2 spawnPos;
                    if (TryGetGroundPosition((Vector2)target + Vector2.up * 1f, out spawnPos))
                    {
                        float wDmg = (effect.parameters.Count > 0 ? effect.parameters[0] * owner.statObject.FinalAttackPower : 1f) * multiplier;
                        float wIntervalTime = (effect.parameters.Count > 1 ? effect.parameters[1] : 0.3f);
                        int wCount = (effect.parameters.Count > 2 ? (int)effect.parameters[2] : 5);
                        float wSpeed = (effect.parameters.Count > 3 ? effect.parameters[3] : 1f);

                        StartCoroutine(SpawnWave(owner, spawnPos, wDmg, wIntervalTime, wCount, wSpeed));
                    }
                    break;
                }

        }
    }

    public bool HasBuff(Entity owner, Entity target, Statuseffect buffSample)
    {
        return target.statusEffects.Exists(buff => buff.name == buffSample.name);
    }
    
}
