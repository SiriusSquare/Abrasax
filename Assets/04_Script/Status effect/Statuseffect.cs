using UnityEngine;

public abstract class Statuseffect : MonoBehaviour, IPoolable
{
    [field: SerializeField] public Sprite StatusIcon { get; protected set; }
    [field: SerializeField] public Color StatusIconColor { get; protected set; }
    [field: SerializeField] public float RotateTime { get; protected set; }
    [field: SerializeField] public Color DamageColor { get; protected set; }

    [field: SerializeField] public bool UseTimer { get; protected set; }

    [field: SerializeField] public int MaxStack { get; set; }
    [field: SerializeField] public int Stack { get; set; }

    [field: SerializeField] public float Duration { get; set; }

    [SerializeField] private string _itemName;
    public string ItemName => _itemName;

    public GameObject GameObject => gameObject;

    [field: SerializeField] public Entity TargetEntity { get; set; }
    [field: SerializeField] public Entity OwnerEntity { get; set; }

    [SerializeField] protected GameObject mainEffect;
    [SerializeField] protected GameObject[] invokeEffects;
    protected IPoolable mainEffectInstance;
    
    protected virtual void Start() { }

    protected virtual void Update()
    {
        if (Stack <= 0)
        {
            RemoveFromTarget();
            Stack = 0;
            Duration = RotateTime;
            if (TargetEntity != null && TargetEntity.statusEffects.Contains(this))
            {
                TargetEntity.statusEffects.Remove(this);
            }
            PoolManager.Instance.Push(this);
            return;
        }

        if (Stack > MaxStack)
            Stack = MaxStack;

        if (UseTimer && Duration > 0)
        {
            Duration -= Time.deltaTime;
            if (Duration <= 0)
            {
                EffectInvoke();
                
            }
        }
    }

    public virtual void ResetItem()
    {
        Stack = 0;
        Duration = RotateTime;
        if (TargetEntity != null && TargetEntity.statusEffects.Contains(this))
        {
            TargetEntity.statusEffects.Remove(this);
        }
    }

    

    public virtual void SetOwner(Entity owner, Entity target)
    {
        OwnerEntity = owner;
        TargetEntity = target;

        if (TargetEntity != null && !TargetEntity.statusEffects.Contains(this))
            TargetEntity.statusEffects.Add(this);
        if (Stack == 0)
        {
            Stack = 1; // Ensure at least one stack is present
        }
        if (mainEffect != null)
        {
            mainEffectInstance = PoolManager.Instance.Pop(mainEffect.name);
            mainEffectInstance.GameObject.transform.rotation = Quaternion.identity;
            mainEffectInstance.GameObject.transform.SetParent(TargetEntity.transform);
            mainEffectInstance.GameObject.transform.position = TargetEntity.transform.position;
            mainEffectInstance.GameObject.SetActive(true);
        }
    }




    private void RemoveFromTarget()
    {
        if (TargetEntity != null)
            TargetEntity.statusEffects.Remove(this);
    }

    public virtual void EffectInvoke()
    {
        Duration = RotateTime;
    }

    public virtual void AddStack(int count)
    {
        Stack = Mathf.Min(Stack + count, MaxStack);
    }

    public virtual void RemoveStack(int count)
    {
        Stack = Mathf.Max(Stack - count, 0);
    }
}
