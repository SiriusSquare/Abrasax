using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyMovement : Movement
{

    private void Update()
    {
        

        //if (!entity.isAttacking && Keyboard.current.upArrowKey.isPressed && !entity.isDead)
        //{
        //    entity.Animator.SetBool("isGuarding", true);
        //    entity.isGard = true;
        //    moveSpeedMultiplier = 0.5f;

        //    if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        //        entity.OnGardEvent?.Invoke();
        //}
        //else if (entity.isGard && !Keyboard.current.upArrowKey.isPressed && !entity.isDead)
        //{
        //    entity.isGard = false;
        //    moveSpeedMultiplier = 1.0f;
        //    entity.Animator.SetBool("isGuarding", false);
        //}

        base.Update();
    }
}
