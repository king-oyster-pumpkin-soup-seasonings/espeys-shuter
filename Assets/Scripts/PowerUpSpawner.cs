using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] powerups;
    private float time;

    private void OnEnable() => GameManager.OnWaveStart += StartingUpPowerupSpawner;
    private void OnDisable() => GameManager.OnWaveStart -= StartingUpPowerupSpawner;

    public static PowerUpSpawner Instance { get; private set; }
    public int permaPowerUpCaughtDuringTheWave;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 3f;
        time = 0;
    }

    void StartingUpPowerupSpawner()
    {
        if (powerups == null) return;
        permaPowerUpCaughtDuringTheWave = 0;
        // Debug.Log("StartingUpPowerupSpawner");
        StartCoroutine(DelayBeforeActualSpawnerLoop());
    }

    private IEnumerator DelayBeforeActualSpawnerLoop()
    {
        yield return new WaitForSeconds(2f);
        // Debug.Log("STARTING THE POWERUP SPAWN LOOP");
        StartCoroutine(SpawnerLoop());
    }

    private IEnumerator SpawnerLoop()
    {
        while (GameManager.Instance.waveIsOngoing)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (!GameManager.Instance.waveIsOngoing) break;

            transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
            time = 0;

            if (permaPowerUpCaughtDuringTheWave < 3)
            {
                Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, transform.rotation);
                spawnInterval = Random.Range(10f, 15f);
                // spawnInterval = Random.Range(0, 1f); // for debug since its fast 
            }
            else
            {
                // Debug.Log($"player caught {permaPowerUpCaughtDuringTheWave} perma powerups, So powerup types LIMITS!");
                List<int> limitedPowerupsAvailableAsIndexes = new List<int>();
                for (int i = 0; i < powerups.Length; i++)
                {
                    if (powerups[i].CompareTag("PowerUpHealth") ||
                        powerups[i].CompareTag("PowerUpShield"))
                        limitedPowerupsAvailableAsIndexes.Add(i);
                }

                if (limitedPowerupsAvailableAsIndexes.Count != 0)
                {
                    Instantiate
                    (
                        powerups
                        [
                            limitedPowerupsAvailableAsIndexes[Random.Range(0, limitedPowerupsAvailableAsIndexes.Count)]
                        ],
                        transform.position,
                        transform.rotation
                    );
                    spawnInterval = Random.Range(15f, 20f);
                }
            }
        }
    }
}
