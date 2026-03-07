using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Image healthBarFill;

    private Animator anim;
    private PlayerController playerController;

    public AudioClip damageSound;
    public AudioClip dieSound;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        UpdateHealthBar();
    }

    public void TakeDamage(int damage, Transform enemyTransform)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth > 0)
        {
            anim.SetTrigger("Hurt");
            audioSource.PlayOneShot(damageSound);
            playerController.isAttacking = false;
            playerController.isHurt = true;

            playerController.CancelInvoke("EndHurt");
            playerController.Invoke("EndHurt", 0.4f);
            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            int knockbackDirection = transform.position.x < enemyTransform.position.x ? -1 : 1;
            
            rb.linearVelocity = Vector2.zero; 
            rb.AddForce(new Vector2(knockbackDirection * 10f, 5f), ForceMode2D.Impulse);
        }
        else
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        audioSource.PlayOneShot(dieSound);
        anim.SetBool("IsDead", true);
        gameObject.layer = LayerMask.NameToLayer("Corpse");
        playerController.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.sharedMaterial = null;
   }
}