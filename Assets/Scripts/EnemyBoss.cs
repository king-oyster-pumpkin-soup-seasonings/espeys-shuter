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
    private short fireLaser, shootBullets, spawnDrones, swarmBullets; // weapons
    private bool isIntroSpawnDone, isIntermissionDone; // pause
    private short isShieldOn; // shield
    private short moveToOtherSide, moveToLeft, moveToRight; // movement
    private bool recastActions; // action permission


    // BOSS WEAPON
    [SerializeField] private Transform barrelCenter, barrelSide1, barrelSide2, circleBarrel1, circleBarrel2;
    [SerializeField] private GameObject enemyBullet, enemyLaser, enemyDrones;
    [SerializeField] private float bulletFireRate, spawnRate, swarmFireRate;
    private float timeForNextFire, timeForNextSpawnDrones, timeForNextSwarmFires;
    private bool firedFirstSide, isLaserAvailable;

    // PLAYER AS TARGET
    [SerializeField] private GameObject player;
    private Transform playerTransform;


    private void OnEnable()
    {
        Laser.LaserIsDone += SetLaserAsAvailable;
    }

    private void OnDisable()
    {
        Laser.LaserIsDone -= SetLaserAsAvailable;
    }

    private void SetLaserAsAvailable()
    {
        isLaserAvailable = true;
    }

    void Start()
    {
        isLaserAvailable = true;
        fireLaser = 0;
        shootBullets = 0;
        spawnDrones = 0;
        swarmBullets = 0;
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
        if (spawnRate == 0) spawnRate = 2f;
        if (swarmFireRate == 0) swarmFireRate = 0.4f;
        firedFirstSide = false;
        timeForNextFire = Time.time;
        timeForNextSpawnDrones = Time.time;
        timeForNextSwarmFires = Time.time;

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

    private void SelectWeapons()
    {
        shootBullets = 0;
        fireLaser = 0;
        spawnDrones = 0;
        swarmBullets = 0;

        int weaponsToPick = Random.Range(1, 3);
        int currentPicks = 0;

        while (currentPicks < weaponsToPick)
        {
            int weaponChoice = Random.Range(0, 4);

            if (weaponChoice == 0 && shootBullets == 0)
            {
                shootBullets = 1;
                currentPicks++;
            }
            else if (weaponChoice == 1 && fireLaser == 0)
            {
                fireLaser = 1;
                currentPicks++;
            }
            else if (weaponChoice == 2 && spawnDrones == 0)
            {
                spawnDrones = 1;
                currentPicks++;
            }
            else if (weaponChoice == 3 && swarmBullets == 0)
            {
                swarmBullets = 1;
                currentPicks++;
            }
        }
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
            bulletFireRate = Random.Range(0.05f, 0.4f);
            spawnRate = Random.Range(2f, 4.5f);
            swarmFireRate = Random.Range(0.2f, 0.4f);

            // weapons use
            SelectWeapons();

            // shield?
            isShieldOn = randomDecision();
            // isShieldOn = ((short)Random.Range(0, 10));

            // movement direction
            moveToOtherSide = randomDecision();
            if (moveToOtherSide == 1) moveToLeft = randomDecision();
            if (moveToLeft == 0 && moveToOtherSide == 1) moveToRight = 1; // if not left, then right

            if (fireLaser == 1 && isLaserAvailable)
            {
                Instantiate(enemyLaser, transform.position, transform.rotation);
                isLaserAvailable = false;
            }

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

        if (spawnDrones == 1)
        {
            if (Time.time >= timeForNextSpawnDrones)
            {
                if (firedFirstSide == false)
                {
                    circleBarrel1.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                    Instantiate(enemyDrones, circleBarrel1.position, circleBarrel1.rotation);
                    firedFirstSide = true;
                }
                else
                {
                    circleBarrel2.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                    Instantiate(enemyDrones, circleBarrel2.position, circleBarrel2.rotation);
                    firedFirstSide = false;
                }

                timeForNextSpawnDrones = Time.time + spawnRate;
            }
        }

        if (swarmBullets == 1)
        {
            if (Time.time >= timeForNextSwarmFires)
            {
                if (firedFirstSide == false)
                {
                    circleBarrel1.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                    Instantiate(enemyBullet, circleBarrel1.position, circleBarrel1.rotation);
                    circleBarrel1.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                    Instantiate(enemyBullet, circleBarrel1.position, circleBarrel1.rotation);
                    firedFirstSide = true;
                }
                else
                {
                    circleBarrel2.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                    Instantiate(enemyBullet, circleBarrel2.position, circleBarrel2.rotation);
                    circleBarrel1.eulerAngles = new Vector3(0, 0, Random.Range(0, 360f));
                    Instantiate(enemyBullet, circleBarrel1.position, circleBarrel1.rotation);
                    firedFirstSide = false;
                }

                timeForNextSwarmFires = Time.time + swarmFireRate;
            }
        }

        if (isShieldOn == 1)
        {
            if (!shieldSprite.enabled)
            {
                StartCoroutine(RoutineInvulnerabilityExpireAfter(Random.Range(5f, 8f)));
                shieldSprite.enabled = true;
                shieldCollider.enabled = true;
                EnemyBossSetInvulnerability?.Invoke(true);
            }
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