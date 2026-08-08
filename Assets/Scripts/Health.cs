using System;
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
    private float shieldDurationSet, shieldDurationCount;

    // lasers
    private bool isInflictingLaserDamage;


    // --- methods ---
    private void OnEnable()
    {
        GameManager.OnWaveComplete += SelfDestroyWithExplosionIfAny;
    }

    private void OnDisable()
    {
        GameManager.OnWaveComplete -= SelfDestroyWithExplosionIfAny;
    }

    void SelfDestroyWithExplosionIfAny()
    {
        if (gameObject.CompareTag("Player")) return;

        while (currentHealth > 0)
        {
            TakeDamage();
        }
    }

    void Update()
    {
        if (gameObject.CompareTag("Player") && onShield && shieldDurationCount > 0)
        {
            shieldDurationCount -= Time.deltaTime;
            GameManager.Instance.UpdateShieldHUD(shieldDurationCount);
        }
        else if (gameObject.CompareTag("Player") && onShield)
        {
            GameManager.Instance.UpdateShieldHUD(0);
            LoseShield();
        }
    }


    void Start()
    {
        // data
        if (maxHealth == 0) maxHealth = 1;
        currentHealth = maxHealth;
        isInflictingLaserDamage = false;

        if (shield == null && gameObject.CompareTag("Player")) GameObject.FindGameObjectWithTag("Shield");
        if (shield != null && shieldSR == null) shieldSR = shield.GetComponent<SpriteRenderer>();
        if (shieldSR != null) shieldSR.enabled = false;
        if (shieldDurationSet == 0) shieldDurationSet = 5f;
        shieldDurationCount = shieldDurationSet;
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

        // Object collides with missile
        if ((gameObject.CompareTag("Enemy") ||
             gameObject.CompareTag("Asteroid"))
            && other.gameObject.CompareTag("Missile"))
        {
            TakeDamage();
            TakeDamage();
            TakeDamage();
            Destroy(other.gameObject);
        }

        // Object collides with BombExplosion
        if ((gameObject.CompareTag("Enemy") ||
             gameObject.CompareTag("Asteroid"))
            && other.gameObject.CompareTag("BombExplosion"))
        {
            // Debug.Log($"EXPLOSION HIT: {gameObject.name}");
            TakeDamage();
            TakeDamage();
            TakeDamage();
            TakeDamage();
            TakeDamage();
        }

        // Object collides with PLAYER:
        if (gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("Player") ||
            gameObject.CompareTag("Asteroid") && other.gameObject.CompareTag("Player"))
        {
            currentHealth = 0;
            TakeDamage();
        }

        // Player pickups POWERUPS:
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
            // if (shieldSR != null) StartCoroutine(GainShieldCoroutine());
            if (shieldSR != null) GainShield();
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

    private void GainShield()
    {
        onShield = true;
        shieldSR.enabled = true;
        shieldDurationCount = shieldDurationSet;
    }

    private void LoseShield()
    {
        shieldSR.enabled = false;
        onShield = false;
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
