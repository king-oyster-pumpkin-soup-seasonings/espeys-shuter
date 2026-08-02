using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // --- variables ---
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    [SerializeField] private UnityEvent<int> OnHealthChanged;
    [SerializeField] private UnityEvent OnDied;

    // --- methods ---
    void Start()
    {
        // data
        if (maxHealth == 0) maxHealth = 1;
        currentHealth = maxHealth;

        // ui
        OnHealthChanged.Invoke(currentHealth);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("Player") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("EnemyBullet") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();

            if (other.CompareTag("PlayerBullet") || other.CompareTag("EnemyBullet"))
            {
                Destroy(other.gameObject);
            }
        }
    }

    public void TakeDamage()
    {
        // data
        currentHealth--;
        OnHealthChanged.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            OnDied.Invoke();
        }
    }
}
