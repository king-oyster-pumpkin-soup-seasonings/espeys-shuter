using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D enemyRB;
    [SerializeField] private GameObject player;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite explosionSprite;
    private Transform playerTransform;
    private bool isExploding;
    [SerializeField] private int scoreValue = 10;

    [SerializeField] private float widthMovement;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Transform barrel1, barrel2, barrel3, barrel4, soloBarrel;
    [SerializeField] private GameObject enemyBullet;

    public static Action EnemyDied;

    private Vector2[] directionsX = new Vector2[2];
    private Vector2 directionY;
    private Vector2 assignedDirection;
    private Vector2 downwardForce;

    void Start()
    {
        directionsX[0] = new Vector2(1, 0);
        directionsX[1] = new Vector2(-1, 0);
        directionY = new Vector2(0, -1);
        assignedDirection = directionsX[Random.Range(0, directionsX.Length)];
        // Debug.Log($"DIRECTION: {directionsX[1]}");
        // Debug.Log($"ASSIGNED DIRECTION: {assignedDirection}");
        if (gameObject.CompareTag("EnemyFighter")) widthMovement = Random.Range(5f, 10f);

        isExploding = false;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyRB == null) enemyRB = GetComponent<Rigidbody2D>();
        if (movementSpeed == 0) movementSpeed = 1f;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && playerTransform == null) playerTransform = player.transform;
        transform.up = playerTransform.position - transform.position;
        if (gameObject.CompareTag("EnemySprayer")) StartCoroutine(EnemyAsSprayer());
        else if (gameObject.CompareTag("EnemyFighter")) StartCoroutine(EnemyAsFighter());
    }

    private void FixedUpdate()
    {
        if (isExploding) return;

        if (gameObject.CompareTag("Enemy"))
        {
            transform.up = playerTransform.position - transform.position;
            enemyRB.linearVelocity = transform.up * movementSpeed;
        }

        else if (gameObject.CompareTag("EnemySprayer"))
        {
            // enemyRB.linearVelocity = transform.up * (Mathf.Sin(Time.time * movementSpeed) * widthMovement);
            downwardForce = Vector2.down * movementSpeed;
            Vector2 rotationForce = transform.up * movementSpeed * widthMovement;
            enemyRB.angularVelocity = rotationSpeed;
            enemyRB.linearVelocity = downwardForce + rotationForce;
        }

        else if (gameObject.CompareTag("EnemyFighter"))
        {
            transform.up = playerTransform.position - transform.position;

            downwardForce = directionY * movementSpeed;
            Vector2 signatureMovement = assignedDirection * MathF.Sin(Time.time * movementSpeed);
            enemyRB.linearVelocity = downwardForce + signatureMovement;
        }
    }


    public void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        enemyRB.linearVelocity = Vector2.zero;

        spriteRenderer.sprite = explosionSprite;

        EnemyDied?.Invoke();
        Destroy(gameObject, 0.3f);
    }

    void Update()
    {
        // SPRAYER
        if (enemyRB.position.y >= 8f || enemyRB.position.y <= -7.5f)
            transform.position = new Vector2(transform.position.x, 8f);

        // THE REST
        if (enemyRB.position.y >= 5.7f || enemyRB.position.y <= -5.6f && !gameObject.CompareTag("EnemySprayer"))
            transform.position = new Vector2(transform.position.x, 5.5f);

        if (enemyRB.position.x >= 9.2f || enemyRB.position.x <= -9.2f && !gameObject.CompareTag("EnemySprayer"))
            transform.position = new Vector2(Random.Range(-9f, 9f), 7f);
    }

    // private IEnumerator EnemyAsRammer()
    // {
    //     while (!isExploding)
    //     {
    //         yield return new WaitForSeconds(0);
    //         if (isExploding) break;
    //         transform.up = playerTransform.position - transform.position;
    //         enemyRB.linearVelocity = transform.up * movementSpeed;
    //     }
    // }

    private IEnumerator EnemyAsFighter()
    {
        while (!isExploding)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f));
            Instantiate(enemyBullet, soloBarrel.position, soloBarrel.rotation);
            widthMovement = Random.Range(1f, 5f);
            assignedDirection = directionsX[Random.Range(0, directionsX.Length)];
            movementSpeed = Random.Range(1f, 2f);
            if (directionY.y < 1f)
            {
                directionY.y = Mathf.Round(Random.Range(-1f, 1f));
                // Debug.Log($"new directionY: {directionY.y}");
            }
            else directionY.y = -1f;
        }
    }

    private IEnumerator EnemyAsSprayer()
    {
        while (!isExploding)
        {
            // Debug.Log("SPRAYER SHOOTS");
            yield return new WaitForSeconds(Random.Range(3f, 5f));
            Instantiate(enemyBullet, barrel1.position, barrel1.rotation);
            Instantiate(enemyBullet, barrel2.position, barrel2.rotation);
            Instantiate(enemyBullet, barrel3.position, barrel3.rotation);
            Instantiate(enemyBullet, barrel4.position, barrel4.rotation);
        }
    }
}
