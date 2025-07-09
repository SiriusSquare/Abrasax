using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class NormalAttack : Skill
{

    [SerializeField] private AnimationClip downskillanimation;
    protected override void Awake()
    {
        base.Awake();
        Skillcolor = owner != null ? owner.color : Color.white;
    }

    public override IEnumerator TriggerSkill(Entity entity)
    {
        if (owner != null)
        {

            owner.Health -= useHealth;
            if (isfirstCooldown && SkillUseCount == 0)
            {
                owner.Stamina -= useStamina;
                owner.Mana -= useMana;
            }
            else
            {
                owner.Stamina -= useStamina * 0.5f;
                owner.Mana -= useMana * 0.5f;
            }
            owner.HPBarSystem.SetHP();
            owner.HPBarSystem.SetMana();
            owner.GardLevelsubsubplus = skillGardLevel;
        }

        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot trigger skill: GameObject is inactive.");
            yield break;
        }
        if (isfirstCooldown && SkillUseCount == 0)
        {
            SkillCoolDown = finalSkillCoolDown;
        }

        if (entity != null && !entity.isAttacking && !isfirstCooldown)
        {

        }
        else if (entity == null || entity.isAttacking || SkillDissabled)
            yield break;

        owner = entity;
        originpos = owner.transform.position;


        owner.isAttacking = true;
        skillComboTimer = 0.0f;

        int index = SkillUseCount;
        SkillCountUP();

        yield return StartCoroutine(PlaySkillCoroutine(index));
        if (index == skillDamage.Length - 1)
        {
            if (owner != null)
            {
                owner.GardLevelsubsubplus = 0;
            }

            if (!isfirstCooldown)
            {
                SkillCoolDown = finalSkillCoolDown;
            }
            
            SkillUseCount = 0;
        }
        else
        {
            while (isnotCombo && index < skillDamage.Length - 1)
            {
                index = SkillUseCount;
                SkillCountUP();

                yield return StartCoroutine(PlaySkillCoroutine(index));
            }
        }

        owner.isAttacking = false;

    }


    protected override IEnumerator Skill1() => PlaySkillCoroutine(0);
    protected override IEnumerator Skill2() => PlaySkillCoroutine(1);
    protected override IEnumerator Skill3() => PlaySkillCoroutine(2);
    protected override IEnumerator Skill4() => PlaySkillCoroutine(3);

    protected override IEnumerator PlaySkillCoroutine(int index)
    {
        if (owner.Animator != null && skillAnimation != null && index < skillAnimation.Length)
        {
            owner.Animator.speed = skillAnimSpeed[index] * owner.statObject.EffectiveAttackSpeed;
            owner.Animator.Play(skillAnimation[index].name);
        }
        if (skillActiveForceDict.TryGetValue(index, out SkillActiveForce data))
        {
            yield return ApplySkillMovement(data.Entrypos, GetMoveType(entryskilmovetypes, index));
        }

        yield return new WaitForSeconds(skillEntryDelay[index] / owner.statObject.EffectiveAttackSpeed);

        if (skillActiveForceDict.TryGetValue(index, out SkillActiveForce data2))
        {
            yield return ApplySkillMovement(data2.Middlepos, GetMoveType(middleskilmovetypes, index));
        }

        SpawnAttackHitbox(index);

        yield return new WaitForSeconds(skillEndDelay[index] / owner.statObject.EffectiveAttackSpeed);

        if (skillActiveForceDict.TryGetValue(index, out SkillActiveForce data3))
        {
            yield return ApplySkillMovement(data3.Endpos, GetMoveType(endskilmovetypes, index));
        }
    }
    private IEnumerator ApplySkillMovement(Vector2 force, SkillMoveType moveType)
    {
        switch (moveType)
        {
            case SkillMoveType.Force:
                ApplyForce(force);
                break;

            case SkillMoveType.MoveedForce:
                float direction = Mathf.Sign(owner.movement._xMove.x);
                ApplyForce(new Vector2(force.x * direction, force.y));
                break;

            case SkillMoveType.Teleport:
                TryTeleport(force);
                break;

            case SkillMoveType.Dive:
                yield return TryDive(force);
                break;
            case SkillMoveType.DoMove:
                yield return DoMoveTo(force);
                break;
        }
    }
    private IEnumerator DoMoveTo(Vector2 targetPos, float duration = 0.3f)
    {
        if (owner == null) yield break;

        Vector2 start = owner.transform.position;
        float time = 0f;

        Rigidbody2D rb = owner.Rigidbody2D;
        float? savedGravity = null;
        if (rb != null)
        {
            savedGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            owner.transform.position = Vector2.Lerp(start, targetPos, t);
            yield return null;
        }

        owner.transform.position = targetPos;

    }
    private SkillMoveType GetMoveType(SkillMoveType[] array, int index)
    {
        return array != null && index < array.Length ? array[index] : SkillMoveType.Force;
    }



    private void TryTeleport(Vector2 force)
    {
        Vector2 dir = force.normalized * owner.facedir;
        float distance = force.magnitude;
        Vector2 origin = owner.transform.position;
        Vector2 targetPos = origin + dir * distance;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, groundLayer);
        if (hit.collider != null)
        {
            targetPos = hit.point - dir * 0.2f;
        }

        owner.transform.position = targetPos;
    }

    private IEnumerator TryDive(Vector2 force)
    {
        Vector2 dir = force.normalized * owner.facedir;
        float distance = force.magnitude;
        Vector2 origin = owner.transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, groundLayer);
        if (hit.collider != null)
        {
            Vector2 targetPos = hit.point;
            yield return new WaitForSeconds(0.05f);
            owner.transform.position = targetPos;
        }
    }

    private void ApplyForce(Vector2 force)
    {
        if (owner.Rigidbody2D != null)
        {
            owner.Rigidbody2D.AddForce(new Vector2(force.x * owner.facedir.x, force.y), ForceMode2D.Impulse);
        }
    }
    private void ApplyMoveedForce(Vector2 force)
    {
        if (owner.Rigidbody2D != null)
        {
            owner.Rigidbody2D.AddForce(new Vector2(force.x * owner.movement._xMove.x, force.y), ForceMode2D.Impulse);
        }
    }
    private void SpawnAttackHitbox(int index)
    {
        if (index >= skillHitBoxSpawnPos.Length || index >= skillHitBoxSpawnSize.Length)
        {
            Debug.LogWarning("실행할 히트박스가 범위를 벗어났습니다");
            return;
        }

        IPoolable hitbox = PoolManager.Instance.Pop("AttackHitBox");

        Vector2 flippedPos = new Vector2(skillHitBoxSpawnPos[index].x * owner.facedir.x, skillHitBoxSpawnPos[index].y);
        Vector2 spawnPosition = (Vector2)owner.transform.position + flippedPos;

        hitbox.GameObject.transform.position = spawnPosition;

        hitbox.GameObject.transform.parent = owner.transform;


        Vector3 hitboxScale = skillHitBoxSpawnSize[index];
        hitboxScale.x *= owner.facedir.x;
        hitbox.GameObject.transform.localScale = hitboxScale;
        if (!isparrenteffect)
            hitbox.GameObject.transform.parent = null;

        if (effect != null && index < effect.Length && effect[index] != null)
        {
            IPoolable effectObj = PoolManager.Instance.Pop(effect[index].GetComponent<IPoolable>().ItemName);

            effectObj.GameObject.transform.SetParent(owner.transform);

            if (effectTransformDict != null && effectTransformDict.TryGetValue(index, out EffectTransformData data))
            {
                Vector3 effectPos = data.position;
                Vector3 effectScale = data.scale;
                Vector3 effectRotate = data.rotation;

                if (isrotateplus180)
                {
                }
                else
                {
                    effectScale.y *= owner.facedir.x;
                }


                effectObj.GameObject.transform.localPosition = effectPos;
                effectObj.GameObject.transform.localRotation = Quaternion.Euler(effectRotate);
                effectObj.GameObject.transform.localScale = effectScale;
                effectObj.GameObject.GetComponent<Effect>().SetColor(Skillcolor);
            }
            else
            {
                effectObj.GameObject.transform.localPosition = Vector3.zero;
                effectObj.GameObject.transform.localRotation = Quaternion.identity;
                effectObj.GameObject.transform.localScale = Vector3.one;
            }
            if (!isparrenteffect)
                effectObj.GameObject.transform.parent = null;
            effectObj.GameObject.SetActive(true);



            AttackTriggerScript ats = hitbox.GameObject.GetComponent<AttackTriggerScript>();
            ats.ChangeSetting(skillDamage[index] * owner.statObject.FinalAttackPower, skillgardPenetration[index], skillKnockBack[index], hiteffect[index], 0.5f, attackType, attackElement, owner, skillLifeTime[index]);
            ats.ChangeSetting2(skillInterVal[index]);

            if (soundEffect != null && index < soundEffect.Length && soundEffect[index] != null)
            {
                owner.audioSource.PlayOneShot(soundEffect[index]);
            }
            if (entityEventHolder != null && entityEventHolder.pendingEvents != null)
            {
                foreach (var pending in entityEventHolder.pendingEvents)
                {
                    bool isSameSkillEvent = thisSkillEvent != null && Array.Exists(thisSkillEvent, e => e == pending.eventType);
                    if (!isSameSkillEvent) continue;

                    bool isHitEvent = pending.eventType.ToString().EndsWith("Hit");
                    if (isHitEvent)
                    {
                        ats.AddEvent(pending);
                    }
                    else
                    {

                        EventManager.Instance.EventMain(owner, hitbox.GameObject.transform.position,
                            pending.condition, pending.secondaryConditions, pending.effect, pending.statusEffects, pending.eventType);
                    }
                }
            }

            if (owner.entityEventHolder != null && owner.entityEventHolder.pendingEvents != null)
            {
                foreach (var pending in owner.entityEventHolder.pendingEvents)
                {
                    bool isSameSkillEvent = thisSkillEvent != null && Array.Exists(thisSkillEvent, e => e == pending.eventType);
                    if (!isSameSkillEvent) continue;

                    bool isHitEvent = pending.eventType.ToString().EndsWith("Hit");
                    if (isHitEvent)
                    {
                        ats.AddEvent(pending);
                    }
                    else
                    {
                        EventManager.Instance.EventMain(owner,(Vector2) hitbox.GameObject.transform.position,
                            pending.condition, pending.secondaryConditions, pending.effect, pending.statusEffects, pending.eventType);
                    }
                }
            }


        }

        hitbox.GameObject.SetActive(true);
    }
}
