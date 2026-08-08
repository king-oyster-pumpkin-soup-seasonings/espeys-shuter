using System.Collections;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] powerups;
    private float time;

    private void OnEnable() => GameManager.OnWaveStart += StartingUpPowerupSpawner;
    private void OnDisable() => GameManager.OnWaveStart -= StartingUpPowerupSpawner;

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 3f;
        time = 0;
    }

    void StartingUpPowerupSpawner()
    {
        if (powerups == null) return;
        StartCoroutine(DelayBeforeActualSpawnerLoop());
    }

    private IEnumerator DelayBeforeActualSpawnerLoop()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnerLoop());
    }

    private IEnumerator SpawnerLoop()
    {
        yield return new WaitForSeconds(spawnInterval);
        transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
        time = 0;
        spawnInterval = Random.Range(3f, 10f);
        // spawnInterval = Random.Range(0, 1f); // debug
        GameObject powerup = Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position,
            transform.rotation);
    }
}
