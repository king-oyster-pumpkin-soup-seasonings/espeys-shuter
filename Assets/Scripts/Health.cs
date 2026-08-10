using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // --- variables ---
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool isInvulnerable;

    [SerializeField] private UnityEvent<int> OnHealthChanged;
    [SerializeField] private UnityEvent<string> OnDied;

    // alternative caller
    public static Action<int> OnHealthChangeAlt;

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
        GameManager.OnWaveStart += CheckThenSetPlayerHealthIfBelow3;
        EnemyBoss.EnemyBossDied += SelfDestroyWithExplosionIfAny;
        EnemyBoss.EnemyBossSetInvulnerability += SetInvulnerability;
    }

    private void OnDisable()
    {
        GameManager.OnWaveComplete -= SelfDestroyWithExplosionIfAny;
        GameManager.OnWaveStart -= CheckThenSetPlayerHealthIfBelow3;
        EnemyBoss.EnemyBossDied -= SelfDestroyWithExplosionIfAny;
        EnemyBoss.EnemyBossSetInvulnerability -= SetInvulnerability;
    }

    void SelfDestroyWithExplosionIfAny()
    {
        if (gameObject.CompareTag("Player")) return;

        while (currentHealth > 0)
        {
            TakeDamage("Asteroid");
        }
    }

    void SetInvulnerability(bool invulnerabilityValue)
    {
        if (CompareTag("Boss")) isInvulnerable = invulnerabilityValue;
    }

    void CheckThenSetPlayerHealthIfBelow3()
    {
        if (gameObject.CompareTag("Player") && currentHealth < 3)
        {
            currentHealth = 3;
            OnHealthChanged?.Invoke(currentHealth);
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
        isInvulnerable = false;
        if (gameObject.CompareTag("Boss")) OnHealthChangeAlt?.Invoke(currentHealth);
        if (gameObject.CompareTag("Boss")) isInvulnerable = true;

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
        if (!isInflictingLaserDamage && other.CompareTag("Laser") &&
            (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Asteroid") ||
             gameObject.CompareTag("EnemySprayer") || gameObject.CompareTag("EnemyFighter") ||
             gameObject.CompareTag("Boss")))
        {
            StartCoroutine(InflictLaserDamagePerSecond());
        }
    }

    private IEnumerator InflictLaserDamagePerSecond(float seconds = 0.125f)
    {
        isInflictingLaserDamage = true;
        TakeDamage("Laser");
        yield return new WaitForSeconds(seconds);
        isInflictingLaserDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("Asteroid") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("EnemySprayer") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("EnemyFighter") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("Boss") && other.gameObject.CompareTag("PlayerBullet") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("EnemyBullet") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("Enemy") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("EnemySprayer") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("EnemyFighter") ||
            gameObject.CompareTag("Player") && other.gameObject.CompareTag("Asteroid"))
        {
            if (GameManager.Instance.waveIsOngoing) TakeDamage(other.tag);
        }

        // Object collides with missile
        if ((gameObject.CompareTag("Enemy") ||
             gameObject.CompareTag("EnemySprayer") ||
             gameObject.CompareTag("EnemyFighter") ||
             gameObject.CompareTag("Boss") ||
             gameObject.CompareTag("Asteroid"))
            && other.gameObject.CompareTag("Missile"))
        {
            TakeDamage(other.tag);
            TakeDamage(other.tag);
            TakeDamage(other.tag);
            Destroy(other.gameObject);
        }

        // Object collides with BombExplosion
        if ((gameObject.CompareTag("Enemy") ||
             gameObject.CompareTag("EnemySprayer") ||
             gameObject.CompareTag("EnemyFighter") ||
             gameObject.CompareTag("Boss") ||
             gameObject.CompareTag("Asteroid"))
            && other.gameObject.CompareTag("BombExplosion"))
        {
            // Debug.Log($"EXPLOSION HIT: {gameObject.name}");
            TakeDamage(other.tag);
            TakeDamage(other.tag);
            TakeDamage(other.tag);
            TakeDamage(other.tag);
            TakeDamage(other.tag);
        }

        // Object collides with PLAYER:
        if (gameObject.CompareTag("Enemy") && other.gameObject.CompareTag("Player") ||
            gameObject.CompareTag("EnemySprayer") && other.gameObject.CompareTag("Player") ||
            gameObject.CompareTag("EnemyFighter") && other.gameObject.CompareTag("Player") ||
            gameObject.CompareTag("Asteroid") && other.gameObject.CompareTag("Player"))
        {
            currentHealth = 0;
            TakeDamage(other.tag);
        }

        // Asteroid Collides with the BOSS:
        if (gameObject.CompareTag("Asteroid") && other.CompareTag("Boss"))
        {
            currentHealth = 0;
            TakeDamage(other.tag);
        }

        if (gameObject.CompareTag("Player") && other.CompareTag("Boss"))
        {
            currentHealth = 0;
            TakeDamage(other.tag);
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


    public void TakeDamage(string hittedBy = "")
    {
        // condition
        if (onShield || isInvulnerable) return;

        // data
        currentHealth--;
        OnHealthChanged.Invoke(currentHealth);
        if (gameObject.CompareTag("Boss")) OnHealthChangeAlt?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            OnDied.Invoke(hittedBy);
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
