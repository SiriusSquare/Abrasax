using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PendingEvent
{
    public Entity owner;
    public Entity target;
    public Vector2? targetPosition;
    public GameEventType eventType;
    public EventCondition condition;
    public SecondaryCondition[] secondaryConditions;
    public EventEffect effect;
    public Statuseffect[] statusEffects;
}

public class EntityEventHolder : MonoBehaviour
{
     public List<PendingEvent> pendingEvents = new List<PendingEvent>();

    public void AddEvent(PendingEvent evt)
    {
        pendingEvents.Add(evt);
    }

    public void SetOwnerTargetAll(Entity owner, Entity target)
    {
        foreach (var evt in pendingEvents)
        {
            evt.owner = owner;
            evt.target = target;
            evt.targetPosition = null;
        }
    }

    public void SetOwnerTargetPositionAll(Entity owner, Vector2 targetPos)
    {
        foreach (var evt in pendingEvents)
        {
            evt.owner = owner;
            evt.target = null;
            evt.targetPosition = targetPos;
        }
    }

    public void ExecuteAll()
    {
        foreach (var evt in pendingEvents)
        {
            if (evt.owner == null || (evt.target == null && !evt.targetPosition.HasValue))
            {
                continue;
            }

            if (evt.target != null)
            {
                EventManager.Instance.EventMain(
                    evt.owner,
                    evt.target,
                    evt.condition,
                    evt.secondaryConditions,
                    evt.effect,
                    evt.statusEffects,
                    evt.eventType
                );
            }
            else if (evt.targetPosition.HasValue)
            {
                EventManager.Instance.EventMain(
                    evt.owner,
                    evt.targetPosition.Value,
                    evt.condition,
                    evt.secondaryConditions,
                    evt.effect,
                    evt.statusEffects,
                    evt.eventType
                );
            }
        }
    }

    }
