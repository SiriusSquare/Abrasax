using UnityEngine;

public class SlimeAI : SkillControllerBase
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
    [SerializeField] private float cooldownTime = 3f;
    [SerializeField] private float currentCooldown = 0f;
    [SerializeField] private float xjumpPower = 1f;
    [SerializeField] private float yjumpPower = 1f;
    private Entity chaseTarget = null;
    private bool canChase = false;
    private bool stopMove = false;

    protected override void Update()
    {
        if (entity.isDead) return;

        if (currentCooldown > 0f)
            currentCooldown -= Time.deltaTime;

        stopMove = false;

        foreach (var sensor in sensors)
        {
            Vector2 flippedOffset = sensor.offset;
            flippedOffset.x *= entity.facedir.x;

            Vector2 worldPos = (Vector2)transform.position + flippedOffset;
            LayerMask detectionMask = sensor.GroundMask | sensor.TargetMask;

            Collider2D hit = sensor.shape switch
            {
                SensorShape.Box => Physics2D.OverlapBox(worldPos, sensor.size, 0f, detectionMask),
                SensorShape.Circle => Physics2D.OverlapCircle(worldPos, sensor.radius, detectionMask),
                SensorShape.Capsule => Physics2D.OverlapCapsule(worldPos, sensor.size, sensor.capsuleDirection, 0f, detectionMask),
                _ => null
            };

            Entity targetEntity = hit ? hit.GetComponent<Entity>() : null;
            bool isPlayer = targetEntity != null && targetEntity.Faction == "Player";

            if (sensor.name == "PlayerDirectionBox" && isPlayer)
            {
                canChase = true;
                chaseTarget = targetEntity;
                stopMove = false;
            }
            if (sensor.name == "JumpAirBox" && !entity.isHit && !entity.isflyHit && entity.isGround)
            {
                bool isGrounded = Physics2D.OverlapBox(worldPos, sensor.size, 0f, sensor.GroundMask);
                if (!isGrounded)
                {
                    entity.movement.OnJumpEvent?.Invoke();
                    entity.movement.HandleJump();
                }
            }


        }

        if (!stopMove && canChase && chaseTarget != null && entity.canMove && !entity.isHit && !entity.isflyHit && entity.isGround)
        {
            Vector2 moveDir = ((Vector2)chaseTarget.transform.position - (Vector2)transform.position).normalized;
            

            if (Mathf.Abs(moveDir.x) > 0.1f)
            {
                bool isRight = moveDir.x > 0;
                if (entity.movement.IsFacingRight() != isRight)
                {
                    entity.movement.SendMessage("Flip", isRight);
                }
            }
            if (currentCooldown <= 0f)
            {
                entity.Rigidbody2D.AddForce(new Vector2(moveDir.x * entity.statObject.MoveSpeed * xjumpPower, yjumpPower),ForceMode2D.Impulse);
                currentCooldown = cooldownTime;
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
