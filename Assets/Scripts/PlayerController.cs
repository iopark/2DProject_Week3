using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Object Movement")]
    [SerializeField] private float movePower; // Where player's movement is somewhat is limited by the movePower (Assuming this is the fixed acceleration). 
    [SerializeField] private float jumpPower;
    [SerializeField] private float maxSpeed; // Limiting player's top speed 

    [Header("Object Composition")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 inputDir;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        jumpPower = 10;
        movePower = 10;
        maxSpeed = 5; 
    }

    private void Update()
    {
        Move(); 
    }
    private void Move()
    {
        // further details can be applied for 'better' overall moving experience 
        // thus, how a character can move based on the input can be specified in the following function. 
        // When controller input is placed, Limit by the maxSpeed, 
        if (inputDir.x < 0 && // if input is placed to the left, 
            rb.velocity.x > -maxSpeed) // left top speed 
        {
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
        }
        else if (inputDir.x > 0 && // if input is placed to right 
            rb.velocity.x < maxSpeed) // rigidbody's velocity on x-axis is less then right top speed 
        {
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force); // only insert new force if not top speed 
        }

        // 역방향 힘 가해주기로 멈춰주기 구현  
    }
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        anim.SetBool("onLand", false);
    }
    private void OnMove(InputValue value)   
    {
        inputDir = value.Get<Vector2>(); // for each directional related input keys, update the next vector2 values on the rigid body 
        anim.SetFloat("MoveSpeed", Mathf.Abs(inputDir.x));
        if (inputDir.x > 0)
        {
            spriteRenderer.flipX = false; 
        }
        else if (inputDir.x < 0)
            spriteRenderer.flipX = true; 
    }
    private void OnJump(InputValue value)
    {
        anim.SetBool("onLand", false);
        Jump(); 
    }
    /// <summary>
    /// 땅에 접촉유무를 위해 충돌체 를 이용한다. 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetBool("onLand", true); 
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        anim.SetBool("onLand", false); 
    }
}
