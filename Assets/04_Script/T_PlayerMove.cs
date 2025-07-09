using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Tilemaps;

public class T_PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriterenderer;
    [SerializeField] private float movespeed = 8f;
    [SerializeField] private float jumppower = 13f;
    [SerializeField] private int MaxjumpCount = 0;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerlayer;
    [SerializeField] private Vector2 groundCheckerSize;
    [SerializeField] public bool CanJump { get; set; } = true;
    [SerializeField] public bool CanMove { get; set; } = true;
    [SerializeField] public bool isGround { get; protected set; }
    [SerializeField] public bool IsJump { get; protected set; }
    [SerializeField] public bool IsRun { get; protected set; }
    float targetSpeed = 0f;
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        isGround = CheckGround();
        if (isGround == true)
        {
            jumpCount = MaxjumpCount;
            IsJump = false;
        }
        else
        {

            IsJump = true;
        }


        float targetSpeed = 0f;
        float acceleration = 30f;
        float groundDeceleration = 15f;
        float airDeceleration = 7.5f;

        if (CanMove == true) // 이동이 true일시 이동 A와 D키로 이동
        {
            if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                targetSpeed = -movespeed;


            }
            else if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
                targetSpeed = movespeed;


            }
        }


        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) // 둘다 누르면 이동 X
        {
            targetSpeed = 0f;
        }


        float deceleration = IsJump ? airDeceleration : groundDeceleration;


        if (targetSpeed != 0f)
        {
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0f, deceleration * Time.deltaTime);
        }


        if (Mathf.Abs(rb.linearVelocityX) < 0.1f && targetSpeed == 0f)
        {
            rb.linearVelocityX = 0f;
        }



        if (Input.GetKey(KeyCode.Space)) // 점프 스크립트
        {
            if (isGround == true && CanJump)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(Vector2.up * jumppower * rb.mass, ForceMode2D.Impulse);


            }
            else
            {

                if (jumpCount > 0 && CanJump)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.AddForce(Vector2.up * jumppower * rb.mass, ForceMode2D.Impulse);
                    jumpCount--;
                }
                else
                {
                    return;
                }
            }
        }
    }
    public bool CheckGround() //그라운드체커로 레이어 검사
    {

        Collider2D collider = Physics2D.OverlapBox(groundChecker.position, groundCheckerSize, 0, groundLayer);

        return collider != null;
    }
    private void OnDrawGizmos() // 그냥 기즈모 표시
    {
        Gizmos.DrawWireCube(groundChecker.position, groundCheckerSize);
    }
    public void ChangeJumpActive(bool how) // 점프가능여부 변환
    {
        CanJump = how;
    }
    public void ChangeMoveActive(bool how) //이동가능여부 변환
    {
        CanMove = how;
    }
}