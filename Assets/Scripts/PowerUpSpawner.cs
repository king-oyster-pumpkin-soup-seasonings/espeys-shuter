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
        Debug.Log("StartingUpPowerupSpawner");
        StartCoroutine(DelayBeforeActualSpawnerLoop());
    }

    private IEnumerator DelayBeforeActualSpawnerLoop()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("STARTING THE POWERUP SPAWN LOOP");
        StartCoroutine(SpawnerLoop());
    }

    private IEnumerator SpawnerLoop()
    {
        while (GameManager.Instance.waveIsOngoing)
        {
            yield return new WaitForSeconds(spawnInterval);
            transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
            time = 0;
            spawnInterval = Random.Range(5f, 10f);
            // spawnInterval = Random.Range(0, 1f); // for debug since its fast 
            Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, transform.rotation);
        }
    }
}
