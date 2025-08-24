using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 300f;
    private float damage = 10f;
    private Animator animator;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    public Transform player;
    public float moveSpeed = 3f;
    public float chaseRange = 50f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (player == null) return;

        // Oyuncu ile düşman arasındaki mesafeyi hesapla
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            // Oyuncuya doğru yönel ve hareket et
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            animator.SetFloat("Speed", Mathf.Abs(direction.x));
            // Karakterin yüzünü oyuncuya dön
            if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        float distance = Vector2.Distance(
        new Vector2(transform.position.x, transform.position.y),
        new Vector2(player.position.x, player.position.y));

        if (distance < 2f)
        {
            animator.SetTrigger("Attack");
            audioSource.Play();
        }
    }
    public void GetDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            animator.SetTrigger("Dead");
            Invoke("Reload", 2f);
        }
    }
    void Reload()
    {
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControl player = other.gameObject.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.GetDamage(damage);
            }
        }
    }
}
