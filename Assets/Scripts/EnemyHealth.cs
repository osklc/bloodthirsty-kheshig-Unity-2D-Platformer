using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public bool isHurt = false;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    private Color originalColor;
    public float knockbackForce = 5f;
    private Animator anim;

    public AudioClip damageSound;
    public AudioClip dieSound;
    private AudioSource audioSource;
    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage, Transform attackerTransform)
    {
        currentHealth -= damage;
        anim.SetTrigger("Hurt");
        audioSource.PlayOneShot(damageSound);

        ApplyKnockback(attackerTransform);

        StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ApplyKnockback(Transform attackerTransform)
    {
        Vector2 knockbackDirection = (transform.position - attackerTransform.position).normalized;
        knockbackDirection.y = 0.5f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }
    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
    }

    public void EndHurt()
    {
        isHurt = false;
    }

    void Die()
    {
        anim.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        gameObject.layer = LayerMask.NameToLayer("Corpse");
        StartCoroutine(RemoveBody());
    }

    IEnumerator RemoveBody()
    {
        audioSource.PlayOneShot(dieSound);
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

}
