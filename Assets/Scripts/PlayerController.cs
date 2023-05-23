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

    [Header("JumpDetect")]
    [SerializeField] private bool isGrounded;
    [SerializeField] LayerMask groundLayer; // you can select multiple, single, or non amount of layers in the inspector view. 

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
    /// <summary>
    /// 가능한 물리적인 연산들(Raycast, Physics)들은 고정update 로 구현하는것이 ㅈ2ㅓㅇ배다. 
    /// </summary>
    private void FixedUpdate()
    {
        GroundCheck(); 
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
        anim.SetBool("hasLanded", false);
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
        //anim.SetBool("hasLanded", false);
        Jump(); 
    }
    //Ground Interaction: 2가지 방법이 존재합니다. 
    //1. RayCast 를 응용하여 땅 찾는방법 Landing Interaction 
    //2. Collider객체를 하위자식으로 두어 collider 에 따른 eventFunction 으로 반응하는 방법 
    private void GroundCheck()
    {

        RaycastHit2D groundDetect = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, groundLayer); 
        Debug.DrawRay(transform.position, Vector2.down, Color.red, 0);

        if (groundDetect.collider != null)
        {
            isGrounded = true;
            anim.SetBool("hasLanded", true);
            Debug.DrawRay(transform.position, new Vector3(groundDetect.point.x, groundDetect.point.y, 0) - transform.position, Color.red, 0);
        }
        else
        {
            isGrounded= false;
            anim.SetBool("hasLanded", false);
            Debug.DrawRay(transform.position, Vector2.down, Color.red, 0);
        }
    }

    private bool WallCheck()
    {
        //Vector2 dirFront = new Vector2 (transform.position.x * -0.4f, transform.position.y); //given 마우스는 왼쪽을 바라보며 시작하니) 
        Debug.DrawRay(transform.position, Vector2.left, Color.green, 0); 
        return Physics2D.Raycast(transform.position, Vector2.left, 0.8f, groundLayer);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
            anim.SetBool("hasLanded", true); 
    }
}
