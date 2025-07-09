using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Video;

[System.Serializable]
public struct EffectTransformData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

[System.Serializable]
public struct SkillActiveForce
{
    public Vector3 Entrypos;
    public Vector3 Middlepos;
    public Vector3 Endpos;
}

[System.Serializable]
public class EffectEntry
{
    public int index;
    public EffectTransformData data;
}

[System.Serializable]
public class SkillActiveForceEntry
{
    public int index;
    public SkillActiveForce forceData;
}

[System.Serializable]
public class SkillEventEntry
{
    public int index;
    public UnityEngine.Events.UnityEvent onSkillEvent;
}

[System.Serializable]
public enum SkillMoveType
{
    Force = 0,
    Teleport = 1,
    Dive = 2,
    MoveedForce = 3,
    DoMove = 4,
    IsJumpAndDown = 5,
}

public abstract class Skill : MonoBehaviour
{
    [field: SerializeField] public Color Skillcolor { get; set; } = Color.white;
    [SerializeField] protected GameEventType[] thisSkillEvent;
    [SerializeField] protected EntityEventHolder entityEventHolder;
    [SerializeField] protected bool isparrenteffect = true;
    [SerializeField] protected bool isrotateplus180 = false;
    [SerializeField] protected bool isnotCombo = false;
    [SerializeField] protected bool isfirstCooldown = false;
    [SerializeField] protected SkillMoveType[] entryskilmovetypes = new SkillMoveType[10];
    [SerializeField] protected SkillMoveType[] middleskilmovetypes = new SkillMoveType[10];
    [SerializeField] protected SkillMoveType[] endskilmovetypes = new SkillMoveType[10];
    [SerializeField] protected int skillGardLevel;
    [SerializeField] public float useHealth;
    [SerializeField] public float useStamina;
    [SerializeField] public float useMana;
    [SerializeField] protected LayerMask groundLayer;
    [Header("Skill Settings")]
    [SerializeField] protected GameObject HitBox;
    [SerializeField] protected float[] skillDamage;
    [SerializeField] protected int[] skillgardPenetration;
    [SerializeField] protected float[] skillInterVal;
    [SerializeField] protected float[] skillstunTime;
    [SerializeField] protected float[] skillEntryDelay;
    [SerializeField] protected float[] skillEndDelay;
    [SerializeField] protected float[] skillLifeTime;

    [Header("Effect Settings")]
    [SerializeField] protected GameObject[] effect;
    [SerializeField] protected GameObject[] hiteffect;
    [SerializeField] private List<EffectEntry> effectTransformList = new();
    protected Dictionary<int, EffectTransformData> effectTransformDict = new();

    [Header("Audio / Animation")]
    [SerializeField] protected AudioClip[] soundEffect;
    [SerializeField] protected AnimationClip[] skillAnimation;
    [SerializeField] protected float[] skillAnimSpeed;

    [Header("Hitbox Settings")]
    [SerializeField] protected Vector2[] skillHitBoxSpawnPos;
    [SerializeField] protected Vector2[] skillHitBoxSpawnSize;

    [Header("Attack Info")]
    [SerializeField] public float finalSkillCoolDown = 0.0f;
    [SerializeField] protected AttackType attackType = AttackType.Null;
    [SerializeField] protected AttackElement attackElement = AttackElement.Null;

    [Header("Usage Settings")]
    [SerializeField] protected int maxSkillUseCount = 0;
    [SerializeField] protected float skillComboDelay = 0.5f;

    [Header("KnockBack Settings")]
    [SerializeField] protected Vector2[] skillKnockBack;

    [Header("Skill Active Force")]
    [SerializeField] private List<SkillActiveForceEntry> skillActiveForceList = new();
    protected Dictionary<int, SkillActiveForce> skillActiveForceDict = new();

    [Header("Skill Events")]
    [SerializeField] private List<SkillEventEntry> skillEventList = new();
    protected Dictionary<int, UnityEngine.Events.UnityEvent> skillEventDict = new();

    [field: SerializeField] public float SkillCoolDown { get; set; }
    [field: SerializeField] public bool SkillDissabled { get; set; } = false;
    [field: SerializeField] public int SkillUseCount { get; set; }

    public float skillComboTimer = 0.0f;
    protected AudioSource audioSource;
    protected Entity owner;
    public Vector2 originpos;

    protected List<Coroutine> skillCoroutines = new();

    protected Coroutine triggerSkillCoroutine;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateEffectTransformDict();
        UpdateSkillActiveForceDict();
        UpdateSkillEventDict();
    }

    protected virtual void OnValidate()
    {
        UpdateEffectTransformDict();
        UpdateSkillActiveForceDict();
        UpdateSkillEventDict();
    }

    protected void UpdateEffectTransformDict()
    {
        effectTransformDict.Clear();
        foreach (var entry in effectTransformList)
            effectTransformDict[entry.index] = entry.data;
    }

    protected void UpdateSkillActiveForceDict()
    {
        skillActiveForceDict.Clear();
        foreach (var entry in skillActiveForceList)
            skillActiveForceDict[entry.index] = entry.forceData;
    }

    protected void UpdateSkillEventDict()
    {
        skillEventDict.Clear();
        foreach (var entry in skillEventList)
            skillEventDict[entry.index] = entry.onSkillEvent;
    }

    public abstract IEnumerator TriggerSkill(Entity entity);

    protected virtual IEnumerator Skill1() { yield break; }
    protected virtual IEnumerator Skill2() { yield break; }
    protected virtual IEnumerator Skill3() { yield break; }
    protected virtual IEnumerator Skill4() { yield break; }
    protected virtual IEnumerator Skill5() { yield break; }
    protected virtual IEnumerator Skill6() { yield break; }
    protected virtual IEnumerator Skill7() { yield break; }
    protected virtual IEnumerator Skill8() { yield break; }
    protected virtual IEnumerator Skill9() { yield break; }
    protected virtual IEnumerator Skill10() { yield break; }

    protected virtual IEnumerator PlaySkillCoroutine(int index)
    {
        yield break;
    }

    // TriggerSkill 실행 시 코루틴 저장 및 이전 코루틴 취소
    public void StartTriggerSkillCoroutine(Entity entity)
    {
        if (triggerSkillCoroutine != null)
            StopCoroutine(triggerSkillCoroutine);

        triggerSkillCoroutine = StartCoroutine(TriggerSkill(entity));
    }

    public void StopTriggerSkillCoroutine()
    {
        if (triggerSkillCoroutine != null)
        {
            StopCoroutine(triggerSkillCoroutine);
            triggerSkillCoroutine = null;
        }
    }

    public void StartPlaySkillCoroutine(int index)
    {
        StopSkillCoroutines();

        Coroutine coroutine = StartCoroutine(PlaySkillCoroutine(index));
        skillCoroutines.Add(coroutine);
    }

    public void StopSkillCoroutines()
    {
        foreach (var coroutine in skillCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
        skillCoroutines.Clear();
        if (owner != null)
        {
            owner.GardLevelsubsubplus = 0;
        }
    }

    protected virtual void Update()
    {
        SkillCoolDownActive();
        Comboclear();
    }

protected virtual void SkillCoolDownActive()
{
    if (SkillCoolDown > 0.0f)
    {
        if (isfirstCooldown && SkillUseCount > 0)
        {
            SkillCoolDown -= Time.deltaTime;
            return;
        }

        SkillDissabled = true;
        SkillCoolDown -= Time.deltaTime;
        if (SkillCoolDown <= 0.0f)
        {
            SkillDissabled = false;
            SkillCoolDown = 0.0f;
        }
    }
}

    protected virtual void SkillCountUP()
    {
        SkillUseCount++;
        if (SkillUseCount > maxSkillUseCount)
            SkillUseCount = 0;
        else
            skillComboTimer = skillComboDelay / owner.statObject.AttackSpeed;
    }

    protected virtual void Comboclear()
    {
        if (skillComboTimer > 0.0f)
        {
            skillComboTimer -= Time.deltaTime;
            return;
        }
        else
        {
            SkillUseCount = 0;
        }

        if (owner?.Animator == null) return;
        if (SkillUseCount == 0 && !owner.isAttacking)
        {
            AnimatorClipInfo[] currentClips = owner.Animator.GetCurrentAnimatorClipInfo(0);
            if (currentClips.Length > 0)
            {
                string currentClipName = currentClips[0].clip.name;
                foreach (var clip in skillAnimation)
                {
                    if (currentClipName == clip.name)
                    {
                        owner.Animator.speed = 1;
                        owner.Animator.Play("Idle");

                        SkillUseCount = 0;
                        break;
                    }
                }
            }
        }
    }


    protected virtual void InvokeSkillEvent(int index)
    {
        if (skillEventDict.TryGetValue(index, out var evt))
            evt?.Invoke();
    }

    protected virtual void SetAnimationSpeed(int index)
    {
        if (owner?.Animator != null && index < skillAnimation.Length && index < skillAnimSpeed.Length)
        {
            float speedFactor = owner.statObject.AttackSpeed;
            owner.Animator.speed = skillAnimSpeed[index] * speedFactor;
        }
    }

    public void StopSkill()
    {
        skillComboTimer = 0.0f;
        SkillUseCount = 0;

        if (owner != null)
        {
            owner.isAttacking = false;
            owner.GardLevelsubsubplus = 0;
        }
            

        if (owner?.Animator != null)
        {
            owner.Animator.speed = 1;
            owner.Animator.Play("Idle");
        }
        StopSkillCoroutines();
        StopTriggerSkillCoroutine();
        
        StopAllCoroutines();
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        if (skillHitBoxSpawnPos == null || skillHitBoxSpawnSize == null || skillHitBoxSpawnPos.Length != skillHitBoxSpawnSize.Length)
            return;

        Gizmos.color = Color.red;

        for (int i = 0; i < skillHitBoxSpawnPos.Length; i++)
        {
            Vector3 center = transform.position + (Vector3)skillHitBoxSpawnPos[i];
            Vector3 size = (Vector3)skillHitBoxSpawnSize[i];

            Gizmos.DrawWireCube(center, size);
        }
    }
#endif
}
