using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] enemyBosses;
    private float time;
    private int aliveEnemyCount;
    public int spawnCount, spawnLimit;


    private void OnEnable()
    {
        Enemy.EnemyDied += OnEnemyDied;
        GameManager.OnWaveStart += StartSpawningUponWaveStart;
    }

    private void OnDisable()
    {
        Enemy.EnemyDied -= OnEnemyDied;
        GameManager.OnWaveStart -= StartSpawningUponWaveStart;
    }

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 1f;
        if (spawnLimit == 0) spawnLimit = 5;
        time = 0;
    }

    void StartSpawningUponWaveStart()
    {
        Debug.Log("Calling method to StartSpawningUponWaveStart");

        if (GameManager.Instance.wave >= 7)
        {
            Debug.Log($"Wave: {GameManager.Instance.wave}, so endless mode(?)");
            if (time < spawnInterval) time += Time.deltaTime;
            else
            {
                time = 0;
                RandomizeSpawnPoint();
                Spawn(enemies[0]);
            }
        }
        else if (GameManager.Instance.wave <= 6)
        {
            Debug.Log($"Wave < 5 = STARTING SPAWN SET WAVE NO: {GameManager.Instance.wave} !");
            StartCoroutine(SpawnSetWaveNo(GameManager.Instance.wave));
        }
    }

    void RandomizeSpawnPoint()
    {
        transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
        if (GameManager.Instance.wave > 0) spawnInterval = Random.Range(0.5f, 3f);
    }

    void OnEnemyDied()
    {
        aliveEnemyCount--;
    }

    void Spawn(GameObject enemy)
    {
        Instantiate(enemy, transform.position, transform.rotation);
        aliveEnemyCount++;
    }

    private IEnumerator SpawnSetWaveNo(int waveNum)
    {
        yield return new WaitUntil(() => GameManager.Instance.isMessageDone);

        Debug.Log("Actual wave-based (not endless) Spawner which is the Couroutine is RUNNING");

        // WAVE 1
        if (waveNum == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(1f);
                for (int j = 0; j < i; j++)
                {
                    yield return new WaitForSeconds(1f);
                    RandomizeSpawnPoint();
                    Spawn(enemies[0]);
                }

                yield return new WaitUntil(() => aliveEnemyCount <= 0);
            }
        }

        // WAVE 2
        else if (waveNum == 2)
        {
            // spawnCount = 0;

            for (int i = 0; i < 3; i++)
            {
                RandomizeSpawnPoint();
                Spawn(enemies[0]);
                yield return new WaitForSeconds(1f);
            }

            Debug.Log("Spawn forloop complete! Waiting for kill for next batch");

            yield return new WaitUntil(() => aliveEnemyCount <= 0);
            yield return new WaitForSeconds(3f);

            for (int i = 0; i < 2; i++)
            {
                RandomizeSpawnPoint();
                Spawn(enemies[1]);
                yield return new WaitForSeconds(3f);
            }

            yield return new WaitUntil(() => aliveEnemyCount <= 0);
            yield return new WaitForSeconds(3f);

            for (int i = 0; i < 5; i++)
            {
                RandomizeSpawnPoint();
                Spawn(enemies[0]);
                yield return new WaitForSeconds(1f);
                RandomizeSpawnPoint();
                Spawn(enemies[1]);
                yield return new WaitForSeconds(2f);
            }

            Debug.Log("Spawn forloop complete!"); // yield return new WaitUntil(() => aliveEnemyCount <= 0);
        }

        // WAVE 3
        else if (waveNum == 3)
        {
            for (int i = 0; i < 5; i++)
            {
                RandomizeSpawnPoint();
                Spawn(enemies[0]);
                yield return new WaitForSeconds(1f);
                RandomizeSpawnPoint();
                Spawn(enemies[2]);
                yield return new WaitForSeconds(2f);
            }

            yield return new WaitUntil(() => aliveEnemyCount <= 0);
        }

        // WAVE 4
        else if (waveNum == 4)
        {
            for (int i = 0; i < 2; i++)
            {
                RandomizeSpawnPoint();
                Spawn(enemies[0]);
                yield return new WaitForSeconds(1f);
                RandomizeSpawnPoint();
                Spawn(enemies[2]);
                RandomizeSpawnPoint();
                Spawn(enemies[0]);
                yield return new WaitForSeconds(2f);
                RandomizeSpawnPoint();
                Spawn(enemies[1]);
                RandomizeSpawnPoint();
                Spawn(enemies[1]);
                yield return new WaitForSeconds(1f);

                yield return new WaitUntil(() => aliveEnemyCount <= 3);
            }
        }

        // WAVE 5
        else if (waveNum == 5)
        {
            for (int i = 0; i < 5; i++)
            {
                RandomizeSpawnPoint();
                Spawn(enemies[0]);
                RandomizeSpawnPoint();
                Spawn(enemies[1]);
                RandomizeSpawnPoint();
                Spawn(enemies[2]);
                yield return new WaitForSeconds(2f);
            }
        }

        else if (waveNum == 6)
        {
            Spawn(enemyBosses[0]);
            yield return new WaitForSeconds(2f);
        }


        yield return new WaitUntil(() => aliveEnemyCount <= 0);
        yield return new WaitForSeconds(3f);
        GameManager.Instance.TriggerWaveComplete();
        Debug.Log("WAVE COMPLETED!");
    }
}
