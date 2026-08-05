using UnityEngine;

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

    void Start()
    {
        isExploding = false;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (enemyRB == null) enemyRB = GetComponent<Rigidbody2D>();
        if (movementSpeed == 0) movementSpeed = 1.5f;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    void Update()
    {
        if (playerTransform == null || isExploding) return;

        transform.up = playerTransform.position - transform.position;
        enemyRB.linearVelocity = transform.up * movementSpeed;
    }

    public void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        enemyRB.linearVelocity = Vector2.zero;

        spriteRenderer.sprite = explosionSprite;

        Destroy(gameObject, 0.3f);
    }
}
