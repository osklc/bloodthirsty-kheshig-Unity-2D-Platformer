using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public Color damageColor = Color.red;
    private Color originalColor;
    public float knockbackForce = 5f;
    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage, Transform attackerTransform)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " hasar aldi! Kalan Can: " + currentHealth); //for debug

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

    void Die()
    {
        Debug.Log(gameObject.name + " PARCALANDI!");
        Destroy(gameObject);
    }

}
