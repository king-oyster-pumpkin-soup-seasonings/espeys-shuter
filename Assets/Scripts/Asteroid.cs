using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Rigidbody2D asteroidRB;
    [SerializeField] private float movementSpeed;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite explosionSprite;
    private bool isExploding;
    [SerializeField] private int scoreValue = 5;


    void Start()
    {
        isExploding = false;
        if (movementSpeed == 0) movementSpeed = Random.Range(0.3f, 1f);
        asteroidRB.linearVelocity = Vector2.down * movementSpeed;
        asteroidRB.AddForceX(Random.Range(-25f, 25f) * movementSpeed);
    }

    void Update()
    {
        if (asteroidRB.position.y >= 7f || asteroidRB.position.y <= -7f ||
            asteroidRB.position.x >= 9.5f || asteroidRB.position.x <= -9.5f)
            Destroy(gameObject);
    }

    public void AsteroidExplode()
    {
        if (isExploding) return;
        isExploding = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        asteroidRB.linearVelocity = Vector2.zero;

        spriteRenderer.sprite = explosionSprite;

        Destroy(gameObject, 0.3f);
    }
}
