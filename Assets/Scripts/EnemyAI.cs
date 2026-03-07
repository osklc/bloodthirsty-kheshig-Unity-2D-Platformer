using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // --- AI Settings ---
    public Transform player;     
    public float moveSpeed = 3f;
    public float aggroRange = 5f;
    private Rigidbody2D rb;
    private bool isFacingRight = false;

    public int damageToPlayer = 15;
    public float attackRate = 1.5f;
    private float nextAttackTime = 1f;

    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null) return; 

        PlayerHealth pHealth = player.GetComponent<PlayerHealth>();
        if (pHealth != null && pHealth.currentHealth <= 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0);
            return;
        }

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < aggroRange && !anim.GetBool("IsDead"))
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    void ChasePlayer()
    {
        if (transform.position.x < player.position.x)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            if (!isFacingRight) Flip();
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            if (isFacingRight) Flip();
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            PlayerHealth pHealth = collision.gameObject.GetComponent<PlayerHealth>();
            
            if (pHealth != null)
            {
                anim.SetBool("isAttacking", true);
                pHealth.TakeDamage(damageToPlayer, transform);
                
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void EndSlimeAttack()
    {
        anim.SetBool("isAttacking", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}