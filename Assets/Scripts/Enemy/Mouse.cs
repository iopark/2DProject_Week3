using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [Header("Pertaining to Coposition")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    private float moveSpeed;
    private int moveDir; 

    [Header("Sight")]
    [SerializeField] private Transform sightBox;
    [SerializeField] private LayerMask groundMask; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        moveDir = -1; 
        moveSpeed = 3; 
    }
    private void Update()
    {
        //Define activity by the state 
        //Do default activity 
        Move();
        if (!IsGroundExist())
        {
            Turn(); 
        }
        if (WallCheck())
        {
            Turn(); 
        }
    }
    private void Move()
    {
        // certain behavoir, defined by the state 
        rb.velocity = new Vector2(moveSpeed *moveDir, rb.velocity.y); 
    }
    private void Turn()
    {
        transform.Rotate(Vector3.up, 180); // Alternative to the spriterenderer flipX.
        moveDir *= -1; // flip marching direction 
        //if (!spriteRenderer.flipX)
        //{
        //    spriteRenderer.flipX = true;
        //    return;
        //}
        //else
        //{
        //    spriteRenderer.flipX = false;
        //    return;
        //}
    }
    private bool IsGroundExist()
    {
        Debug.DrawRay(sightBox.position, Vector2.down, Color.yellow);
        return Physics2D.Raycast(sightBox.position, Vector2.down, 1.5f, groundMask); // would return true everytime raycast projectile contacts with the ground layer. 
    }
    private bool WallCheck() // 우리 쥐는 벽도 가릴줄 알아요 
    {
        Vector2 mouseFace = new Vector2 (transform.position.x, transform.position.y -0.3f); //given 마우스는 왼쪽을 바라보며 시작하니) 
        Debug.DrawRay(mouseFace, rb.velocity.normalized, Color.green, 0);
        return Physics2D.Raycast(transform.position, rb.velocity.normalized, 0.8f, groundMask);
    }
}
