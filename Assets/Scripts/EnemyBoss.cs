using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBoss : MonoBehaviour
{
    // BOSS APPEARANCE
    [SerializeField] private Rigidbody2D enemyBossRB;
    [SerializeField] private Transform enemyBossTransform;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite explosionSprite;
    [SerializeField] private SpriteRenderer shieldSprite;
    [SerializeField] private Rigidbody2D shieldRB;
    [SerializeField] private CircleCollider2D shieldCollider;
    [SerializeField] private Transform shieldTransform;

    // BOSS TRAITS
    [SerializeField] private float movementSpeed;
    [SerializeField] private int scoreValue;

    // BOSS CONDITIONS AND STATES
    private bool isExploding;
    public static Action EnemyBossDied; // state
    public static Action<bool> EnemyBossSetInvulnerability;
    private short fireLaser, shootBullets, spawnDrones; // weapons
    private bool isIntroSpawnDone, isIntermissionDone; // pause
    private short isShieldOn; // shield
    private short moveToOtherSide, moveToLeft, moveToRight; // movement
    private bool recastActions; // action permission


    // BOSS WEAPON
    [SerializeField] private Transform barrelCenter, barrelSide1, barrelSide2, circleBarrel1, circleBarrel2;
    [SerializeField] private GameObject enemyBullet, enemyLaser, enemyDrones;
    [SerializeField] private float bulletFireRate;
    private float timeForNextFire;
    private bool firedFirstSide;

    // PLAYER AS TARGET
    [SerializeField] private GameObject player;
    private Transform playerTransform;

    void Start()
    {
        fireLaser = 0;
        shootBullets = 0;
        spawnDrones = 0;
        moveToLeft = 0;
        moveToRight = 0;
        recastActions = false;
        isShieldOn = 0;
        shieldSprite.enabled = false;
        shieldCollider.enabled = false;

        // object
        if (movementSpeed == 0) movementSpeed = 1f;
        if (enemyBossRB == null) enemyBossRB = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (explosionSprite == null) Debug.LogError("Enemy Boss: Missing Explosion Sprite Component");
        if (scoreValue == 0) scoreValue = 100;
        if (isExploding) isExploding = false;

        // weapon
        if (bulletFireRate == 0) bulletFireRate = 0.4f;
        firedFirstSide = false;
        timeForNextFire = Time.time;

        // player
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");
        if (playerTransform == null && player != null) playerTransform = player.transform;

        // setting up
        transform.up = Vector2.down;
        isIntroSpawnDone = false;
        transform.position = new Vector2(0, 6.2f);
        StartCoroutine(TriggerIntermissionFor(3f));
        // if (shootBullets == 1) StartCoroutine(ShootBullets(0.4f));
    }

    private short randomDecision()
    {
        return (short)MathF.Floor(Random.Range(0, 2));
    }

    private void Update()
    {
        if (!isIntroSpawnDone && !isIntermissionDone) return;

        if (transform.position.x > 6.5f)
        {
            moveToRight = 0;
            moveToLeft = 1;
        }

        if (transform.position.x < -6.5f)
        {
            moveToLeft = 0;
            moveToRight = 1;
        }

        if (recastActions)
        {
            // weapon modifiers
            bulletFireRate = Random.Range(0.1f, 0.4f);

            // weapons use
            shootBullets = randomDecision();

            // shield?
            isShieldOn = randomDecision();

            // movement direction
            moveToOtherSide = randomDecision();
            if (moveToOtherSide == 1) moveToLeft = randomDecision();
            if (moveToLeft == 0 && moveToOtherSide == 1) moveToRight = 1; // if not left, then right

            StartCoroutine(RecastNewActionsAfter(5f));
        }

        if (shootBullets == 1)
        {
            if (Time.time >= timeForNextFire)
            {
                if (firedFirstSide == false)
                {
                    Instantiate(enemyBullet, barrelSide1.position, barrelSide1.rotation);
                    firedFirstSide = true;
                }
                else
                {
                    Instantiate(enemyBullet, barrelSide2.position, barrelSide2.rotation);
                    firedFirstSide = false;
                }

                timeForNextFire = Time.time + bulletFireRate;
            }
        }

        if (isShieldOn == 1)
        {
            shieldSprite.enabled = true;
            shieldCollider.enabled = true;
            EnemyBossSetInvulnerability?.Invoke(true);
            StartCoroutine(RoutineInvulnerabilityExpireAfter(Random.Range(5f, 8f)));
        }

        if (shieldSprite.enabled)
        {
            shieldTransform.position = enemyBossTransform.position;
        }
    }

    private void FixedUpdate()
    {
        // transform.up = playerTransform.position - transform.position;
        if (!isIntroSpawnDone && isIntermissionDone)
        {
            enemyBossRB.linearVelocity = transform.up * movementSpeed;
            if (enemyBossRB.position.y < 3.5f)
            {
                isIntroSpawnDone = true;
                StartCoroutine(TriggerIntermissionFor(3f));
            }
            // Debug.Log($"isIntroSpawnDone: {isIntroSpawnDone}");
        }

        if (!isIntroSpawnDone) return;

        if (!isIntermissionDone)
        {
            enemyBossRB.linearVelocity = Vector2.zero; // DITO KA LANG hah
            return;
        }

        // --------------------- AFTER INTRO AND INTERMISSION --------------------

        // GameManager.Instance.UpdateBossHealthText(67); // test

        Vector2 downUp = Vector2.down * Mathf.Sin(Time.time * (movementSpeed + 1.5f)) * 2.25f;
        Vector2 signatureMovement = transform.right * Mathf.Cos(Time.time * (movementSpeed + 1.5f)) * 10f;
        enemyBossRB.linearVelocity = downUp + signatureMovement;

        if (moveToRight == 1 && moveToLeft == 0)
        {
            // Debug.Log($"going right {enemyBossRB.position.x}");
            enemyBossRB.linearVelocity = Vector2.right * movementSpeed;
        }

        if (moveToLeft == 1 && moveToRight == 0)
        {
            // Debug.Log($"going left {enemyBossRB.position.x}");
            enemyBossRB.linearVelocity = Vector2.left * movementSpeed;
            if (transform.position.x < -6f) moveToLeft = 0;
        }

        if (shieldSprite.enabled)
        {
            shieldRB.angularVelocity = 23f;
        }
        else shieldRB.rotation = 0;
    }

    private IEnumerator RoutineInvulnerabilityExpireAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        shieldSprite.enabled = false;
        shieldCollider.enabled = false;
        isShieldOn = 0;
        EnemyBossSetInvulnerability?.Invoke(false);
    }

    private IEnumerator TriggerIntermissionFor(float seconds = 1f)
    {
        isIntermissionDone = false;
        Debug.Log("TriggerIntermissionFor " + seconds);
        yield return new WaitForSeconds(seconds);
        isIntermissionDone = true;
        if (isIntroSpawnDone)
        {
            recastActions = true;
            EnemyBossSetInvulnerability?.Invoke(false);
        }
    }

    private IEnumerator ShootBullets(float seconds = 1f)
    {
        yield return new WaitForSeconds(1f);
        while (shootBullets == 1)
        {
            yield return new WaitForSeconds(seconds);
            Instantiate(enemyBullet, barrelSide1.position, barrelSide1.rotation);
            yield return new WaitForSeconds(seconds);
            Instantiate(enemyBullet, barrelSide2.position, barrelSide2.rotation);
        }
    }

    private IEnumerator RecastNewActionsAfter(float seconds = 2f)
    {
        recastActions = false;
        yield return new WaitForSeconds(seconds);
        recastActions = true;
    }

    // private IEnumerator MoveLeftToRightFor(int repeat)
    // {
    // }

    public void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        Debug.Log("BOSS IS DYING!");

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        StartCoroutine(TriggerIntermissionFor(1.5f));
        enemyBossRB.linearVelocity = Vector2.zero;

        spriteRenderer.sprite = explosionSprite;

        if (gameObject.CompareTag("Boss")) EnemyBossDied?.Invoke();
        Destroy(gameObject, 0.3f);
    }
}
