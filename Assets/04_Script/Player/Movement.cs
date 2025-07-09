using UnityEngine;
using UnityEngine.Events;

public abstract class Movement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] public Entity entity;

    [Header("Ground Check")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Vector2 gizmoOffset;
    [SerializeField] protected Vector2 gizmoSize;

    public UnityEvent OnJumpEvent;

    public Vector2 _xMove { get; set; }
    protected Vector2 targetSpeed;
    protected float moveSpeedMultiplier = 1f;
    protected int jumpCount;
    protected bool facingRight = true;

    public Vector2 LookDirection => facingRight ? Vector2.right : Vector2.left;

    protected virtual void FixedUpdate()
    {
        Collider2D col = Physics2D.OverlapBox(transform.position + (Vector3)gizmoOffset, gizmoSize, 0, groundLayer);
        entity.isGround = col != null;

        if (entity.isGround)
            jumpCount = entity.statObject.MaxJumpCount;
    }

    protected virtual void Update()
    {

        if (entity.isAttacking || !entity.canMove || entity.isDead)
        {
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX * moveSpeedMultiplier, 0, 6f * Time.deltaTime);
            return;
        }
        if (entity.isHit || entity.isflyHit)
        {
            HandleSpriteFlip(-entity.Rigidbody2D.linearVelocityX);
        }
        else
        {
            ApplyMovement();
            HandleSpriteFlip(_xMove.x);
            entity.Animator.SetFloat("Speed", Mathf.Abs(_xMove.x));
        }
       
    }

    protected virtual void ApplyMovement()
    {
        float runMultiplier = entity.isRunning ? 1f : 1f;
        float runplus = entity.isRunning ? 1f : 0f;
        targetSpeed = _xMove * (entity.statObject.MoveSpeed * runMultiplier * moveSpeedMultiplier);

        rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, targetSpeed.x, 6f * Time.deltaTime) + (runplus * _xMove.x);

        if (Mathf.Abs(rb.linearVelocityX) < 0.02f)
            rb.linearVelocityX = 0f;
    }

    public virtual void HandleJump()
    {
        if (!entity.canJump || (!entity.isGround && jumpCount <= 0) || entity.isDead) return;

        rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);
        rb.AddForce(Vector2.up * entity.statObject.JumpForce * rb.mass, ForceMode2D.Impulse);
        jumpCount--;
        OnJumpEvent?.Invoke();
    }

    protected virtual void HandleSpriteFlip(float moveX)
    {
        if (moveX > 0 && !facingRight)
            Flip(true);
        else if (moveX < 0 && facingRight)
            Flip(false);
    }

    protected void Flip(bool faceRight)
    {
        facingRight = faceRight;
        entity.transform.rotation = faceRight ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        entity.facedir = faceRight ? Vector2.right : Vector2.left;
    }

    public bool IsFacingRight() => facingRight;

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)gizmoOffset, gizmoSize);
    }
}
