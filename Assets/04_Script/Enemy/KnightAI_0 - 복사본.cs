using UnityEngine;
using System.Collections;

public class KnightAI_1 : SkillControllerBase
{
    public enum SensorShape { Box, Circle, Capsule }

    [System.Serializable]
    public class SensorBox
    {
        public string name = "Sensor";
        public SensorShape shape = SensorShape.Box;
        public Vector2 offset = Vector2.zero;
        public Vector2 size = Vector2.one;
        public float radius = 0.5f;
        public CapsuleDirection2D capsuleDirection = CapsuleDirection2D.Vertical;
        public Color gizmoColor = Color.green;
        public LayerMask GroundMask;
        public LayerMask TargetMask;
    }

    [SerializeField] private SensorBox[] sensors;
    [field: SerializeField] private float[] skillCooldowns;
    [field: SerializeField] private float[] currentCooldown;

    private Entity chaseTarget = null;
    private bool canChase = false;
    private bool stopMove = false;

    // ★ isAttacking 강제 종료를 위해 추가된 타이머
    private float attackTimer = 0f;
    protected override void Update()
    {
      
    }
    protected void FixedUpdate()
    {
        
        if (entity.isDead) return;
        if (entity.isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= 4f)
            {
                entity.isAttacking = false;
                attackTimer = 0f;
                Debug.Log($"[KnightAI_1] isAttacking이 4초 이상 지속되어 강제로 false로 변경되었습니다.");
            }
        }
        else
        {
            attackTimer = 0f;
        }
        
        
        for (int i = 0; i < currentCooldown.Length; i++)
        {
            if (currentCooldown[i] > 0f)
                currentCooldown[i] -= Time.deltaTime;
        }

        
        if (chaseTarget == null || !chaseTarget.gameObject.activeInHierarchy)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                chaseTarget = playerObj.GetComponent<Entity>();
                canChase = chaseTarget != null;
            }
            else
            {
                canChase = false;
            }
        }

        stopMove = false;

        foreach (var sensor in sensors)
        {
            Vector2 flippedOffset = sensor.offset;
            flippedOffset.x *= entity.facedir.x;

            Vector2 worldPos = (Vector2)transform.position + flippedOffset;
            LayerMask detectionMask = sensor.GroundMask | sensor.TargetMask;

            RaycastHit2D hit = sensor.shape switch
            {
                SensorShape.Box => Physics2D.BoxCast(worldPos, sensor.size, 0f, Vector2.zero, 0f, detectionMask),
                SensorShape.Circle => Physics2D.CircleCast(worldPos, sensor.radius, Vector2.zero, 0f, detectionMask),
                SensorShape.Capsule => Physics2D.CapsuleCast(worldPos, sensor.size, sensor.capsuleDirection, 0f, Vector2.zero, detectionMask),
                _ => new RaycastHit2D()
            };
            Entity targetEntity = hit.collider ? hit.collider.GetComponent<Entity>() : null;
            bool isPlayer = targetEntity != null && targetEntity.Faction == "Player";

            if (sensor.name == "PlayerDirectionBox" && isPlayer)
            {
                canChase = true;
                chaseTarget = targetEntity;
                stopMove = false;
            }

            if (sensor.name == "JumpAirBox" && !entity.isHit && !entity.isflyHit)
            {
                bool isGrounded = Physics2D.OverlapBox(worldPos, sensor.size, 0f, sensor.GroundMask);
                if (!isGrounded)
                {
                    entity.movement.OnJumpEvent?.Invoke();
                    entity.movement.HandleJump();
                }
            }

            if (sensor.name == "AttackBox3" && isPlayer && !entity.isAttacking && currentCooldown[2] <= 0f && !entity.isHit && !entity.isflyHit)
            {
                if (skillDatas.Count > 2 && skillDatas[2].skill != null && skillDatas[2].skill.gameObject.activeInHierarchy)
                {
                    currentCooldown[2] = skillCooldowns[2];
                    StartCoroutine(skillDatas[2].skill.TriggerSkill(entity));
                }
            }

            if (sensor.name == "AttackBox2" && isPlayer && !entity.isAttacking && currentCooldown[1] <= 0f && !entity.isHit && !entity.isflyHit)
            {
                if (skillDatas.Count > 0 && skillDatas[1].skill != null && skillDatas[1].skill.gameObject.activeInHierarchy)
                {
                    currentCooldown[1] = skillCooldowns[1];
                    StartCoroutine(skillDatas[1].skill.TriggerSkill(entity));
                }
            }

            if (sensor.name == "AttackBox1" && isPlayer && !entity.isAttacking && currentCooldown[0] <= 0f && !entity.isHit && !entity.isflyHit)
            {
                if (skillDatas.Count > 0 && skillDatas[0].skill != null && skillDatas[0].skill.gameObject.activeInHierarchy)
                {
                    currentCooldown[0] = skillCooldowns[0];
                    StartCoroutine(skillDatas[0].skill.TriggerSkill(entity));
                }
            }

            if (sensor.name == "PlayerUnDirectionBox" && isPlayer && entity.canMove && !entity.isHit && !entity.isflyHit)
            {
                stopMove = true;
            }
        }

        if (!stopMove && canChase && chaseTarget != null && entity.canMove && !entity.isHit && !entity.isflyHit)
        {
            Vector2 moveDir = ((Vector2)chaseTarget.transform.position - (Vector2)transform.position).normalized;
            entity.movement._xMove = moveDir;

            if (Mathf.Abs(moveDir.x) > 0.1f)
            {
                bool isRight = moveDir.x > 0;
                if (entity.movement.IsFacingRight() != isRight)
                {
                    entity.movement.SendMessage("Flip", isRight);
                }
            }
        }
        else if (stopMove)
        {
            entity.movement._xMove = Vector2.zero;
        }
    }

    protected override void OnSkillsInitialized()
    {
    }

    private void OnDrawGizmosSelected()
    {
        if (sensors == null) return;

        foreach (var sensor in sensors)
        {
            Vector2 flippedOffset = sensor.offset;
            if (Application.isPlaying && entity != null)
                flippedOffset.x *= entity.facedir.x;

            Vector2 worldPos = (Vector2)transform.position + flippedOffset;
            Gizmos.color = sensor.gizmoColor;

            switch (sensor.shape)
            {
                case SensorShape.Box:
                    Gizmos.DrawWireCube(worldPos, sensor.size);
                    break;

                case SensorShape.Circle:
                    DrawWireCircle(worldPos, sensor.radius);
                    break;

                case SensorShape.Capsule:
                    DrawWireCapsule(worldPos, sensor.size, sensor.capsuleDirection);
                    break;
            }
        }
    }

    private void DrawWireCircle(Vector2 center, float radius)
    {
        const int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + Vector2.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    private void DrawWireCapsule(Vector2 center, Vector2 size, CapsuleDirection2D direction)
    {
        float radius = direction == CapsuleDirection2D.Vertical ? size.x / 2 : size.y / 2;
        float height = direction == CapsuleDirection2D.Vertical ? size.y - radius * 2 : size.x - radius * 2;

        Vector2 up = direction == CapsuleDirection2D.Vertical ? Vector2.up : Vector2.right;

        Vector2 top = center + up * (height / 2);
        Vector2 bottom = center - up * (height / 2);

        Gizmos.DrawWireCube(center, size);
        DrawWireCircle(top, radius);
        DrawWireCircle(bottom, radius);
    }
}
