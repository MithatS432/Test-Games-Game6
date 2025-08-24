using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    public GameObject shurikenPrefab;
    public GameObject effect;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private AudioSource audioSource;
    public float speed = 6f;
    public float jumpForce = 5f;
    public float hinput;
    public bool isGrounded;
    public bool isJump2;
    private bool facingRight = true;
    [SerializeField] private float health = 100f;

    private bool jumpPressed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        hinput = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", Mathf.Abs(hinput));

        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        Turn();

        if (Input.GetButtonDown("Fire2") && shurikenPrefab != null)
        {
            audioSource.Play();
            ShootShuriken();
        }
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        // Yatay hız ayarı (physics-safe)
        rb.linearVelocity = new Vector2(hinput * speed, rb.linearVelocity.y);

        // Jump uygulama (fizik güncellemesinde)
        if (jumpPressed)
        {
            if (isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                anim.SetTrigger("Jump");
            }
            else if (isJump2)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isJump2 = false;
            }
            jumpPressed = false;
        }
    }

    void ShootShuriken()
    {
        Vector3 spawnPos = transform.position + (facingRight ? Vector3.right * 0.5f : Vector3.left * 0.5f);

        GameObject shuriken = Instantiate(shurikenPrefab, spawnPos, Quaternion.identity);

        if (effect != null)
        {
            GameObject fx = Instantiate(effect, shuriken.transform.position, Quaternion.identity);
            fx.transform.parent = shuriken.transform;
        }

        Shuriken shurikenScript = shuriken.GetComponent<Shuriken>();
        if (shurikenScript != null)
        {
            shurikenScript.SetDirection(facingRight ? Vector2.right : Vector2.left);
        }
    }

    void Turn()
    {
        if (hinput < 0)
        {
            facingRight = false;
            sr.flipX = false;
        }
        else if (hinput > 0)
        {
            facingRight = true;
            sr.flipX = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGround", true);
            isJump2 = true; // tekrar çift zıplama iznini ver
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("isGround", false);
        }
    }

    public void GetDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
