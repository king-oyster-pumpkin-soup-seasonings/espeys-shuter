using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // --- variables ---
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    [SerializeField] private UnityEvent<int> OnHealthChanged;
    [SerializeField] private UnityEvent OnDied;

    // shields
    [SerializeField] GameObject shield;
    private SpriteRenderer shieldSR;
    public bool onShield;

    // lasers
    private bool isInflictingLaserDamage;

    // --- methods ---
    void Start()
    {
        // data
        if (maxHealth == 0) maxHealth = 1;
        currentHealth = maxHealth;
        isInflictingLaserDamage = false;

        if (shield == null) GameObject.FindGameObjectWithTag("Shield");
        if (shield != null && shieldSR == null) shieldSR = shield.GetComponent<SpriteRenderer>();
        if (shieldSR != null) shieldSR.enabled = false;
        onShield = false;

        // ui
        OnHealthChanged.Invoke(currentHealth);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if ((isInflictingLaserDamage == false && other.gameObject.CompareTag("Laser")) &&
            (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Asteroid")))
        {
            StartCoroutine(InflictLaserDamagePerSecond());
        }
    }

    private IEnumerator InflictLaserDamagePerSecond(float seconds = 0.125f)
    {
        isInflictingLaserDamage = true;
        TakeDamage();
        yield return new WaitForSeconds(seconds);
        isInflictingLaserDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("Asteroid") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("EnemyBullet") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("Enemy") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("Asteroid"))
        {
            TakeDamage();

            if (other.CompareTag("PlayerBullet") || other.CompareTag("EnemyBullet"))
            {
                Destroy(other.gameObject);
            }
        }

        if (gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("Player") ||
            gameObject.CompareTag("Asteroid") && other.gameObject.CompareTag("Player"))
        {
            currentHealth = 0;
            TakeDamage();
        }

        if (gameObject.CompareTag("Player") && other.gameObject.CompareTag("PowerUpHealth"))
        {
            if (currentHealth < maxHealth)
            {
                currentHealth++;
                OnHealthChanged.Invoke(currentHealth);
                Destroy(other.gameObject);
            }
        }

        if (gameObject.CompareTag("Player") && other.gameObject.CompareTag("PowerUpMaxHealth"))
        {
            maxHealth++;
            currentHealth++;
            OnHealthChanged.Invoke(currentHealth);
            Destroy(other.gameObject);
        }

        if (gameObject.CompareTag("Player") && other.CompareTag("PowerUpShield"))
        {
            Destroy(other.gameObject);
            if (shieldSR != null) StartCoroutine(GainShieldCoroutine());
        }
    }


    public void TakeDamage()
    {
        // condition
        if (onShield) return;

        // data
        currentHealth--;
        OnHealthChanged.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            OnDied.Invoke();
        }
    }

    private IEnumerator GainShieldCoroutine()
    {
        onShield = true;
        shieldSR.enabled = true;
        yield return new WaitForSeconds(5f);
        shieldSR.enabled = false;
        onShield = false;
    }
}
