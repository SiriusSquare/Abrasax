using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Bleed, Stun, Burn, Frost, Drench, Poison, Shock, AttackUp, DefenseUp
}

[System.Serializable]
public class StatusEffectData
{
    public StatusEffectType type;
    public Sprite icon;
    public Color iconColor;
    public float rotateTime;
    public int maxStack;
    public GameObject effectPrefab;
    public int stack;          // 실제 상태는 이쪽에서 관리
    public float currentTime;  // 실제 상태는 이쪽에서 관리
    public bool useTimer;
    public GameObject effecteffect;
}

public class EntityStatusManager : MonoBehaviour
{
    [SerializeField] public List<StatusEffectData> defaultEffectSettings;

    [field: SerializeField] public Entity Owner { get; private set; }

    private void Update()
    {
        if (Owner == null) return;

        foreach (var effect in defaultEffectSettings)
        {
            bool isActive = effect.stack > 0;
            if (effect.effecteffect != null && effect.effecteffect.activeSelf != isActive)
            {
                effect.effecteffect.SetActive(isActive);
            }

            if (effect.stack <= 0 || !effect.useTimer)
                continue;

            effect.currentTime -= Time.deltaTime;

            if (effect.currentTime <= 0f)
            {
                effect.currentTime = effect.rotateTime;
                TriggerEffect(effect);
            }
        }
    }

    public void ApplyEffect(StatusEffectType type, int stack)
    {
        var effect = defaultEffectSettings.Find(e => e.type == type);
        if (effect == null)
        {
            Debug.LogWarning($"StatusEffect {type} not found in defaultEffectSettings");
            return;
        }

        Debug.Log($"Applying {stack} stacks to {type}. Before: {effect.stack}");
        effect.stack = Mathf.Min(effect.stack + stack, effect.maxStack);
        Debug.Log($"After applying: {effect.stack}");
    }

    private void TriggerEffect(StatusEffectData effect)
    {
        switch (effect.type)
        {
            case StatusEffectType.Burn:
                Owner.TakeDamage(effect.stack * 5, 9999, Vector2.zero, AttackType.Magic, null, transform, null);
                effect.stack--;
                break;
            case StatusEffectType.Stun:
                Owner.ApplyStun(1f);
                effect.stack--;
                break;
            case StatusEffectType.Frost:
                effect.stack--;
                break;
            case StatusEffectType.Drench:
                effect.stack--;
                break;
            case StatusEffectType.Shock:
                Owner.TakeDamage(effect.stack * 3.5f, 9999, Vector2.zero, AttackType.Magic, null, transform, null);
                effect.stack--;
                break;
            case StatusEffectType.Poison:
                Owner.TakeDamage((Owner.Health / 1000) * effect.stack, 9999, Vector2.zero, AttackType.Magic, null, transform, null);
                break;
            case StatusEffectType.Bleed:
                Owner.TakeDamage(effect.stack * 2, 9999, Vector2.zero, AttackType.Null, null, transform, null);
                break;
            case StatusEffectType.AttackUp:
                effect.stack--;
                break;
            case StatusEffectType.DefenseUp:
                effect.stack--;
                break;
        }

        if (effect.effectPrefab != null)
        {
            Instantiate(effect.effectPrefab, Owner.transform.position, Quaternion.identity);
        }
    }
}
