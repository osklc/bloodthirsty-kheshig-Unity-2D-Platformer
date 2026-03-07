using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;
    private Rigidbody2D rb;
    private float moveInput;

    public bool isAttacking = false;
    public bool isHurt = false;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 20;

    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    private bool isFacingRight = true;
    private Animator anim;

    public AudioClip jumpSound;
    public AudioSource audioSource;
    public AudioClip[] footstepSounds;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();    
    }

    void Update()
    {   
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        
        if (!isAttacking && !isHurt)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            anim.SetFloat("Speed", Mathf.Abs(moveInput));

            if (moveInput > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (moveInput <0 && isFacingRight)
            {
                Flip();
            }

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                audioSource.PlayOneShot(jumpSound);
            }
        }
        else if (isAttacking)
        {
            moveInput = 0;
            anim.SetFloat("Speed", 0);
        }

        if (Time.time >= nextAttackTime && !isAttacking && isGrounded)
        {
            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                isAttacking = true;
                rb.linearVelocity = Vector2.zero;
                anim.SetTrigger("Attack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    void FixedUpdate()
    {
        if (!isAttacking && !isHurt)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    public void EndHurt()
    {
        isHurt = false;
    }

    public void PlayFootstep()
{
    if (!isGrounded) return; 

    int randomIndex = Random.Range(0, footstepSounds.Length);
    audioSource.PlayOneShot(footstepSounds[randomIndex]);
}

    public void DeathTouchGround()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        col.offset = new Vector2(-0.05350187f, 0.07009165f);
        col.size = new Vector2(0.3138242f, 0.120186f);
    }

    public void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, transform);
                CameraEffects.instance.TriggerHitStop(0.1f);
                CameraEffects.instance.TriggerShake(0.15f, 0.2f);
            }
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight; 

        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}