using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;
using UnityEditor;
using Code.Core.Pooling;
using Unity.VisualScripting;
using System.Collections;
using System.Linq;
using Unity.Cinemachine;
public class Entity : MonoBehaviour, IDamageable
{
    [field: SerializeField] public Color color = Color.white;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float impulseForce = 0.2f;
    [field : SerializeField] public float Health { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }

    [field: SerializeField] public float MaxStamina { get; set; }

    [field: SerializeField] public float Stamina { get; set; }

    [field : SerializeField] public float Mana { get; set; }
    [field : SerializeField] public float MaxMana { get; set; }

    [field: SerializeField] public float MaxShield { get; set; }
    [field: SerializeField] public float Shield { get; set; }
    [field: SerializeField] public int GardLevel { get; set; }
    [field: SerializeField] public float KnockbackResistance { get; set; }
    [field: SerializeField] public Rigidbody2D Rigidbody2D { get; protected set; }
    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public AudioSource audioSource { get; set; }
    [field: SerializeField] public string Faction { get; set; }
    [field: SerializeField] public int GardLevelplus { get; set; } = 0;
    [field: SerializeField] public int GardLevelsubplus { get; set; } = 0;
    [field: SerializeField] public int GardLevelsubsubplus { get; set; } = 0;
    [field: SerializeField] public UnityEvent OnGardEvent;
    [field: SerializeField] public UnityEvent OnTakeDamageEvent;
    [field: SerializeField] public UnityEvent OnDieEvent;
    [field: SerializeField] protected GameObject shieldHitEffect;
    [field: SerializeField] protected GameObject shieldDestroyEffect;
    [field: SerializeField] protected GameObject hitEffect;
    [field: SerializeField] protected GameObject gardEffect;
    [field: SerializeField] protected GameObject cEJEffect;
    [field: SerializeField] protected GameObject deathEffect;
    
    [field: SerializeField] public HPBar HPBarSystem { get; set; }
    [field : SerializeField] public StatObject statSystem;
    [field: SerializeField] public StatManager statObject;
    [field: SerializeField] public Movement movement { get; set; }
    [field : SerializeField] public EntityStatusManager entityStatusManager { get; set; }
    public bool isAttacking = false;
    public bool noHeal = false;
    public bool noAttack = false;
    public bool noSkill = false;
    public bool noDamage = false;
    public bool noKnockback = false;
    public bool isDead = false;
    public bool isGround = false;
    public bool canJump = true;
    public bool canMove = true;
    public bool canStaminaHeal = true;
    public bool isGard = false;
    public bool isRunning = false;
    
    public bool trueisDead = false;
    private bool isFinalDead = false;
    [SerializeField]private  SkillControllerBase skillController;

    [SerializeField] private GameObject dashEffect;

    public EntityEventHolder entityEventHolder;

    public Vector2 facedir = Vector2.zero;

    public float hitTime = 0f;
    private Coroutine hitCoroutine;
    public bool isHit = false;
    public bool isflyHit = false;
    public List< Statuseffect> statusEffects = new List<Statuseffect>();
    private float deadTime = 0f;

    public bool HasStatusEffect(string effectName)
    {

        foreach (var effect in statusEffects)
        {
            if (effect.GetType().Name == effectName)
            {
                return true;
            }
        }
        return false;
    }

    public void TriggerHit()
    {
        hitTime = 0.2f;
        isHit = true;
        foreach (var effect in skillController.skillDatas)
        {
            effect.skill.StopSkill();
        }
        isAttacking = false;
        Animator.SetBool("isHit", true);
    }
    
    public void ApplyStun(float sans)
    {
        hitTime = sans;
        isHit = true;
        foreach (var effect in skillController.skillDatas)
        {
            effect.skill.StopSkill();
        }
        isAttacking = false;
        Animator.SetBool("isHit", true);
    }
    public void StatManagerSetting()
    {
        if (Faction == "Player")
        {
            statObject.MaxHealth = statSystem.MaxHealth;
            statObject.MaxStamina = statSystem.MaxStamina;
            statObject.MaxMana = statSystem.MaxMana;
            statObject.HealthRecoverPower = statSystem.HealthRecoverPower;
            statObject.StaminaRecoverPower = statSystem.StaminaRecoverPower;
            statObject.ManaRecoverPower = statSystem.ManaRecoverPower;
            statObject.AttackPower = statSystem.AttackPower;
            statObject.AttackPowerMultiplier = statSystem.AttackPowerMultiplier;
            statObject.AttackSpeed = statSystem.AttackSpeed;
            statObject.Defense = statSystem.Defense;
            statObject.CriticalChance = statSystem.CriticalChance;
            statObject.CriticalDamageMultiplier = statSystem.CriticalDamageMultiplier;
            statObject.CriticalGardPenetration = statSystem.CriticalGardPenetration;
            statObject.KnockbackResistance = statSystem.KnockbackResistance;
            statObject.DefGardLevel = statSystem.DefGardLevel;
            statObject.MoveSpeed = statSystem.MoveSpeed;
            statObject.JumpForce = statSystem.JumpForce;
            statObject.CooldownTime = statSystem.CooldownTime;
            statObject.MaxJumpCount = statSystem.MaxJumpCount;
            statObject.Gold = statSystem.Gold;
            statObject.ArrowPoint = statSystem.ArrowPoint;
        }
        else
        {

            statObject.MaxHealth = statSystem.MaxHealth * CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty] * (1 + StageManager.Instance.Floor * 0.05f);
            statObject.MaxStamina = statSystem.MaxStamina * CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty];
            statObject.MaxMana = statSystem.MaxMana * CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty];
            statObject.HealthRecoverPower = statSystem.HealthRecoverPower * CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty];
            statObject.StaminaRecoverPower = statSystem.StaminaRecoverPower * CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty];
            statObject.ManaRecoverPower = statSystem.ManaRecoverPower * CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty];
            statObject.AttackPower = statSystem.AttackPower * CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty] * (1 + StageManager.Instance.Floor * 0.035f);
            statObject.AttackPowerMultiplier = statSystem.AttackPowerMultiplier;
            statObject.AttackSpeed = statSystem.AttackSpeed;
            statObject.Defense = statSystem.Defense;
            statObject.CriticalChance = statSystem.CriticalChance;
            statObject.CriticalDamageMultiplier = statSystem.CriticalDamageMultiplier;
            statObject.CriticalGardPenetration = statSystem.CriticalGardPenetration;
            statObject.KnockbackResistance = statSystem.KnockbackResistance;
            statObject.DefGardLevel = statSystem.DefGardLevel;
            statObject.MoveSpeed = statSystem.MoveSpeed;
            statObject.JumpForce = statSystem.JumpForce;
            statObject.CooldownTime = statSystem.CooldownTime;
            statObject.MaxJumpCount = statSystem.MaxJumpCount;
            statObject.Gold = statSystem.Gold;
            statObject.ArrowPoint = statSystem.ArrowPoint;
        }
    }
    public void Start()
    {
        isHit = false;
        foreach (var effect in skillController.skillDatas)
        {
            effect.skill.StopSkill();
        }
        isAttacking = false;
        StatManagerSetting();
        if (Rigidbody2D == null)
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }
        if (Animator == null)
        {
            Animator = GetComponent<Animator>();
        }
        OnGardEvent = new UnityEvent();
        OnTakeDamageEvent = new UnityEvent();
        OnDieEvent = new UnityEvent();


        if (statObject != null)
        {
            MaxHealth = statObject.MaxHealth;
            MaxStamina = statObject.MaxStamina;
            KnockbackResistance = statObject.KnockbackResistance;
            MaxMana = statObject.MaxMana;
            Health = MaxHealth;
            Stamina = MaxStamina;
            Mana = 0;
            HPBarSystem?.StartSetting();
        }
        else
        {
            MaxHealth = 100f;
            MaxStamina = 100f;
            MaxShield = 0f;
            GardLevel = 0;
            KnockbackResistance = 0f;
            Health = MaxHealth;
            Stamina = MaxStamina;
            Mana = 0;
            HPBarSystem?.StartSetting();
        }
        
        
        

    }
    
    public void Gurad()
    {
        GardLevel = statObject.DefGardLevel + GardLevelplus + GardLevelsubplus + GardLevelsubsubplus;
    }
    private void Update()
    {
        
        if ( !trueisDead && (isDead || Health <= 0))
        {
            Die();
        }
        if ((isDead || trueisDead) && !isFinalDead)
        {
            deadTime += Time.deltaTime;

            if (deadTime >= 3f)
            {
                isFinalDead = true;
                Die();
            }
        }

        if (isFinalDead)
        {
            deadTime += Time.deltaTime;

            if (deadTime >= 5f)
            {
                DeadOne();
            }
            if (deadTime >= 7f)
            {
                Destroy(gameObject);
            }
        }
        if (hitTime > 0f)
        {
            hitTime -= Time.deltaTime;
            if (hitTime <= 0f)
            {
                isHit = false;
                Animator.SetBool("isHit", false);
            }
        }
        // 피격 중이고 공중에 있으면 isflyHit 켜기
        if (isHit && !isGround)
        {
            if (!isflyHit)
            {
                isflyHit = true;
                Animator.SetBool("isflyHit", true);
            }
        }
        // isflyHit 중인데 땅에 닿으면 끄기
        else if (isflyHit && isGround)
        {
            isflyHit = false;
            Animator.SetBool("isflyHit", false);
        }
        if (isRunning && dashEffect != null)
        {
            Stamina -= Time.deltaTime * 35f;
            dashEffect.SetActive(true);
        }
        else if (isRunning == false && dashEffect != null)
        {
            dashEffect.SetActive(false);
        }
        // 가드 중이 아니거나 공격 중일 때만 스태미너 회복
        if (!isGard || isAttacking || !isRunning)
        {
            Stamina += Time.deltaTime * statObject.EffectiveStaminaRecoverPower * (statObject.MaxStamina / 10);
            Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
        }
        Mana = Mathf.Clamp(Mana, 0, MaxMana);
        HPBarSystem?.SetStamina();
        HPBarSystem?.SetMana();
    }

    public void Die()
    {
        isDead = true;
        
        foreach (var effect in skillController.skillDatas)
        {
            effect.skill.StopSkill();
        }
        Animator?.Play("Dead");
        trueisDead = true;
    }

    public void DeadOne()
    {
        if (CoinManager.Instance != null)
        {
            StartCoroutine(CoinManager.Instance.DeadCoinSplit(this, 0.02f));
        }
        
    }
    public void DeadTwo()
    {
        Destroy(gameObject);
    }
    public void Heal(float healAmount)
    {
        if (Health <= 0 || isDead || noHeal)
        {
            return;
        }
        Health += healAmount * statObject.HealthRecoverPower;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        IPoolable sa = PoolManager.Instance.Pop("DamageText");
        if (sa.GameObject != null)
        {
            DamageTextScript damageTextScript = sa.GameObject.GetComponent<DamageTextScript>();
            if (damageTextScript != null)
            {
                damageTextScript.SettingText(healAmount, AttackElement.Null, AttackType.Null, HitType.Heal);
                sa.GameObject.transform.position = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
                sa.GameObject.SetActive(true);
            }
        }
        HPBarSystem?.SetHP();
    }


    public void AddShield(float amount)
    {
        if (Health <= 0 || isDead)
        {
            return;
        }

        Shield += amount;

        if (Shield > MaxShield)
        {
            MaxShield = Shield;
        }
        if (MaxShield > MaxHealth * 2)
        {
            MaxShield = MaxHealth * 2;
            Shield = MaxShield;

        }
        IPoolable sa = PoolManager.Instance.Pop("DamageText");
        if (sa.GameObject != null)
        {
            DamageTextScript damageTextScript = sa.GameObject.GetComponent<DamageTextScript>();
            if (damageTextScript != null)
            {
                damageTextScript.SettingText(amount, AttackElement.Null, AttackType.Null, HitType.AddShield);
                sa.GameObject.transform.position = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
                sa.GameObject.SetActive(true);
            }
        }
    }
    public void KnockBack(Vector2 knockback)
    {
        if (noKnockback || knockback == Vector2.zero)
        {
            return;
        }
        Rigidbody2D.AddForce(knockback * Rigidbody2D.mass, ForceMode2D.Impulse);
    }

    public void CrisisEscapeJump()
    {
        if (isflyHit)
        {
            Rigidbody2D.linearVelocity = Vector2.zero;
            Rigidbody2D.AddForce(new Vector2(0,(statObject.JumpForce / 1.5f) * Rigidbody2D.mass),ForceMode2D.Impulse);
            Stamina -= MaxStamina / 10;
            hitTime = 0;
            isflyHit = false;
            isHit = false;
            Animator.SetBool("isHit", false);
            Animator.SetBool("isflyHit", false);
            IPoolable ka = PoolManager.Instance.Pop(cEJEffect.name);
            if (ka.GameObject != null)
            {
                ka.GameObject.transform.position = transform.position;
                ka.GameObject.SetActive(true);
            }
    

        }
    }
    public void TakeDamage(float damage, int gardPenetration, Vector2 knockback, AttackType attackType, Entity Attacker, Transform attackpos,GameObject effect, float stunTime = 0.2f, HitType hitType = HitType.Normal)
    {
  
        if (isDead)
        {
            return;
        }
        damage -= ((statObject.Defense / 2) + statObject.Defense + entityStatusManager.defaultEffectSettings.Find(e => e.type == StatusEffectType.DefenseUp).stack);
        if (Attacker != null && Attacker.Faction == Faction)
        {
            return;  // 아군 공격은 무시
        }
        if ( damage <= 0 || noDamage || isDead)
        {
            return;
        }
        int currentGardLevel = GardLevel + GardLevelsubplus + GardLevelsubsubplus - entityStatusManager.defaultEffectSettings.Find(e => e.type == StatusEffectType.Drench).stack;
        if (attackType == AttackType.Physics)
        {
            if (gameObject.transform.position.x < Attacker.transform.position.x)
            {
                knockback.x = -knockback.x;
            }
        }
        else
        {
            if (gameObject.transform.position.x < attackpos.transform.position.x)
            {
                knockback.x = -knockback.x;
            }
        }
        if (isGard == true)
        {
            Vector2 attackDir;

            if (attackType == AttackType.Physics)
            {
                attackDir = ((Vector2)Attacker.transform.position - (Vector2)transform.position).normalized;
            }
            else
            {
                attackDir = ((Vector2)attackpos.position - (Vector2)transform.position).normalized;
            }

            if ((0 > facedir.x && 0 > attackDir.x) || (0 < facedir.x && 0 < attackDir.x))

            {
                currentGardLevel += GardLevelplus;
            }
        }
        if (gardPenetration < currentGardLevel)
        {
            if (gardEffect != null)
            {
                Instantiate(gardEffect, transform.position, Quaternion.identity);
            }

            IPoolable saa2 = PoolManager.Instance.Pop("DamageText");
            if (saa2.GameObject != null)
            {
                DamageTextScript damageTextScript = saa2.GameObject.GetComponent<DamageTextScript>();
                if (damageTextScript != null)
                {
                    damageTextScript.SettingText(0, AttackElement.Null, AttackType.Null, HitType.Guard);
                    saa2.GameObject.transform.position = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
                    saa2.GameObject.SetActive(true);
                }
            }
            knockback.y = 0;
            float guardFactor = Mathf.Max(currentGardLevel, 0.1f);
            float knockResist = Mathf.Max(statObject.KnockbackResistance, 0.1f);
            float baseKnockbackScaling = 0.5f;

            float knockbackMultiplier = baseKnockbackScaling / guardFactor / knockResist;
            Vector2 finalKnockback = knockback * knockbackMultiplier;

            Rigidbody2D.AddForce(finalKnockback, ForceMode2D.Impulse);
            Stamina -= (damage / currentGardLevel) * 4.5f;
            Mana += damage / 5f * statObject.ManaRecoverPower;

            impulseSource.GenerateImpulse(impulseForce * knockback.magnitude / currentGardLevel / 10);
            TriggerGuardPendingEvents(transform.position, damage);
            return;
        }
        if (Shield > 0)
        {
            float shieldDamage = Mathf.Min(Shield, damage);
            Shield -= shieldDamage;


            if (shieldHitEffect != null)
            {
                Instantiate(shieldHitEffect, transform.position, Quaternion.identity);
                
            }
            if (Shield <= 0)
            {
                if (shieldDestroyEffect != null)
                {
                    Instantiate(shieldDestroyEffect, transform.position, Quaternion.identity);
                }
                float newdamage = Mathf.Abs(Shield);
                Shield = 0;
                TakeDamage(newdamage, 0, Vector2.zero, AttackType.Null, null, transform, null);
            }
            IPoolable saa2 = PoolManager.Instance.Pop("DamageText");
            if (saa2.GameObject != null)
            {
                DamageTextScript damageTextScript = saa2.GameObject.GetComponent<DamageTextScript>();
                if (damageTextScript != null)
                {
                    damageTextScript.SettingText(damage, AttackElement.Null, AttackType.Null, HitType.ShieldDamage);
                    saa2.GameObject.transform.position = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
                    saa2.GameObject.SetActive(true);
                }
            }
            Rigidbody2D.AddForce(knockback / 3 / Mathf.Max(0.1f, statObject.KnockbackResistance), ForceMode2D.Impulse);
            HPBarSystem?.SetHP();
            Mana += damage / 7 * statObject.ManaRecoverPower;
            
            return;
        }

        Rigidbody2D.AddForce(knockback / Mathf.Max(0.1f,statObject.KnockbackResistance), ForceMode2D.Impulse);
        Animator?.SetTrigger("Hit");
        Health -= damage;
        HPBarSystem?.SetHP();
        IPoolable sa = PoolManager.Instance.Pop("DamageText");
        if (sa.GameObject != null)
        {
            DamageTextScript damageTextScript = sa.GameObject.GetComponent<DamageTextScript>();
            if (damageTextScript != null)
            {
                damageTextScript.SettingText(damage, AttackElement.Null, attackType,hitType);
                sa.GameObject.transform.position = (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-0.5f,0.5f),UnityEngine.Random.Range(-0.5f,0.5f));
                sa.GameObject.SetActive(true);
            }
        }
        IPoolable ka = PoolManager.Instance.Pop(effect.name);
        if (ka.GameObject != null)
        {
            ka.GameObject.transform.position = transform.position;
            ka.GameObject.SetActive(true);
        }
        Mana += damage / 9 * statObject.ManaRecoverPower;
        OnTakeDamageEvent?.Invoke();
        float knockbackhapsan = Mathf.Abs(knockback.x) + Mathf.Abs(knockback.y);
        impulseSource.GenerateImpulse(impulseForce * knockback.magnitude / 10);
        if (Attacker != null)
        {
            float recoverAmount = Mathf.Sqrt(damage / 30f) * Attacker.statObject.ManaRecoverPower;
            recoverAmount = Mathf.Min(recoverAmount, 20f);
            Attacker.Mana = Mathf.Clamp(Attacker.Mana + recoverAmount, 0, Attacker.MaxMana);
            if (Attacker.Mana > Attacker.MaxMana)
            {
                Attacker.Mana = Attacker.MaxMana;
            }
        }
        
        if (knockbackhapsan > 0.2f)
        {
            ApplyStun(stunTime);
        }
        TriggerHitPendingEvents(transform.position,damage);

    }

    private void TriggerHitPendingEvents(Vector2 position, float paramiter)
    {
        if (entityEventHolder != null && entityEventHolder.pendingEvents != null)
        {
            foreach (var pending in entityEventHolder.pendingEvents)
            {
                if (pending.eventType == GameEventType.PlayerHit)
                {
                    pending.secondaryConditions[0].parameters[1] = paramiter;
                    EventManager.Instance.EventMain(this, position,
                        pending.condition, pending.secondaryConditions,
                        pending.effect, pending.statusEffects, pending.eventType);
                }
            }
        }
    }

    private void TriggerGuardPendingEvents(Vector2 position, float paramiter)
    {
        if (entityEventHolder != null && entityEventHolder.pendingEvents != null)
        {
            foreach (var pending in entityEventHolder.pendingEvents)
            {
                if (pending.eventType == GameEventType.PlayerGuard)
                {
                    pending.secondaryConditions[0].parameters[1] = paramiter;
                    EventManager.Instance.EventMain(this, position,
                        pending.condition, pending.secondaryConditions,
                        pending.effect, pending.statusEffects, pending.eventType);
                }
            }
        }
    }
}
