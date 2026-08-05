using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] asteroids;
    [SerializeField] private Sprite[] asteroidSprites;
    private float time;
    private int spawnCount;

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 1f;
        time = 0;
    }

    void Update()
    {
        if (GameManager.Instance.isMessageDone == false) return;

        if (time < spawnInterval) time += Time.deltaTime;
        else
        {
            time = 0;
            RandomizeSpawnPoint();
            if (asteroids.Length > 1) Spawn(asteroids[Random.Range(0, asteroids.Length)]);
            else Spawn(asteroids[0]);
        }
    }

    void RandomizeSpawnPoint()
    {
        transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
        if (GameManager.Instance.wave > 0) spawnInterval = Random.Range(0, 5f);
    }

    void Spawn(GameObject preAsteroid)
    {
        GameObject asteroid = Instantiate(preAsteroid, transform.position, transform.rotation);

        if (asteroidSprites.Length > 1)
        {
            asteroid.GetComponent<SpriteRenderer>().sprite =
                asteroidSprites[Random.Range(0, asteroidSprites.Length)];
        }
    }
}
