using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] asteroids;
    [SerializeField] private Sprite[] asteroidSprites;
    private float time;
    private int spawnCount;

    private void OnEnable()
    {
        GameManager.OnWaveStart += StartAsteroidSpawningUponWaveStart;
    }

    private void OnDisable()
    {
        GameManager.OnWaveStart -= StartAsteroidSpawningUponWaveStart;
    }

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 1f;
        time = 0;
    }

    void StartAsteroidSpawningUponWaveStart()
    {
        StartCoroutine(CoroutineSpawnLoop());
    }

    private IEnumerator CoroutineSpawnLoop()
    {
        Debug.Log("RUNNING COURTINE SPAWN LOOP FOR ASTEROID. note, there is while conditon below");
        Debug.Log($"is bool waveIsOngoing true? answer is {GameManager.Instance.waveIsOngoing}");
        while (GameManager.Instance.waveIsOngoing)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            if (!GameManager.Instance.waveIsOngoing) break;
            RandomizeSpawnPoint();
            if (asteroids.Length > 1) Spawn(asteroids[Random.Range(0, asteroids.Length)]);
            else Spawn(asteroids[0]);
        }
    }

    void RandomizeSpawnPoint()
    {
        transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
        if (GameManager.Instance.wave > 0) spawnInterval = Random.Range(0, 3.5f);
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
