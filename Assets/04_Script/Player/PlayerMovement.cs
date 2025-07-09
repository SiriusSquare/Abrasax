using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Movement
{
    [SerializeField] private PlayerInputMovement playerInputMovement;
    private float doubleTapTime = 0.15f;
    private float lastLeftTapTime = -1f;
    private float lastRightTapTime = -1f;
    private void Update()
    {
        _xMove = playerInputMovement.Movecontext;
        if (entity.isDead || entity.isHit || entity.isflyHit || entity.isAttacking)
        {
            if (entity.isRunning)
            {
                entity.isRunning = false;

            }
        }
        if (!entity.isAttacking && Keyboard.current.upArrowKey.isPressed && !entity.isDead && !entity.isHit && !entity.isflyHit)
        {
            entity.Animator.SetBool("isGuarding", true);
            entity.isGard = true;
            moveSpeedMultiplier = 0.3f;

            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                entity.OnGardEvent?.Invoke();
        }
        else if (entity.isGard && !Keyboard.current.upArrowKey.isPressed && !entity.isDead && !entity.isHit && !entity.isflyHit)
        {
            entity.isGard = false;
            moveSpeedMultiplier = 1.0f;
            entity.Animator.SetBool("isGuarding", false);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && (!entity.isflyHit || !entity.isHit || !entity.isGard || !entity.isDead))
        {
            HandleJump();
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame && entity.isflyHit && (!entity.isGard || !entity.isDead))
        {
            entity.CrisisEscapeJump();
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame  && !entity.isDead && !entity.isHit && !entity.isflyHit && !entity.isAttacking)
        {
            if (Time.time - lastLeftTapTime < doubleTapTime)
            {
                entity.isRunning = true;

            }
            lastLeftTapTime = Time.time;
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame && !entity.isDead && !entity.isHit && !entity.isflyHit && !entity.isAttacking)
        {
            if (Time.time - lastRightTapTime < doubleTapTime)
            {
                entity.isRunning = true;

            }
            lastRightTapTime = Time.time;
        }

        if (!Keyboard.current.leftArrowKey.isPressed && !Keyboard.current.rightArrowKey.isPressed || !entity.isGround || entity.isGard)
        {
            if (entity.isRunning)
            {
                entity.isRunning = false;
                entity.Animator.SetBool("isRunning", false);
            }
        }

        base.Update();
    }
}
