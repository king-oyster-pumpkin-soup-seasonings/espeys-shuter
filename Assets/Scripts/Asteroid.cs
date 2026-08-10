using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private Rigidbody2D asteroidRB;
    [SerializeField] private float movementSpeed;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite explosionSprite;
    private bool isExploding;
    [SerializeField] private int scoreValue = 1;


    void Start()
    {
        isExploding = false;
        if (movementSpeed == 0) movementSpeed = Random.Range(0.3f, 1f);
        asteroidRB.linearVelocity = Vector2.down * movementSpeed;
        asteroidRB.rotation = Random.Range(0, 360f);
        asteroidRB.AddForceX(Random.Range(-25f, 25f) * movementSpeed);
    }

    void Update()
    {
        if (asteroidRB.position.y >= 7f || asteroidRB.position.y <= -7f ||
            asteroidRB.position.x >= 9.5f || asteroidRB.position.x <= -9.5f)
            Destroy(gameObject);
    }

    public void AsteroidExplode(string hittedBy = "")
    {
        if (isExploding) return;
        isExploding = true;

        Debug.Log($"HITTED BY {hittedBy}");

        if ((GameManager.Instance != null && GameManager.Instance.waveIsOngoing) &&
            (!hittedBy.Equals("Boss") && !hittedBy.Equals("")))
            GameManager.Instance.AddScore(scoreValue);

        if (GameManager.Instance.waveIsOngoing)
        {
            asteroidRB.linearVelocity = Vector2.zero;
            spriteRenderer.sprite = explosionSprite;
            Destroy(gameObject, 0.3f);
        }
        else StartCoroutine(RoutineAsteroidExplode());
    }

    private IEnumerator RoutineAsteroidExplode()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2.25f));
        asteroidRB.linearVelocity = Vector2.zero;
        spriteRenderer.sprite = explosionSprite;
        Destroy(gameObject, 0.3f);
    }
}
