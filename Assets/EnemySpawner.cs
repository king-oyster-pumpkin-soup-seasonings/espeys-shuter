using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] enemies;
    private float time;
    private int spawnCount, spawnLimit;

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 1f;
        if (spawnLimit == 0) spawnLimit = 5;
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
            Spawn(enemies[0]);
        }
    }

    void RandomizeSpawnPoint()
    {
        transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
        if (GameManager.Instance.wave > 0) spawnInterval = Random.Range(0.5f, 3f);
    }

    void Spawn(GameObject enemy)
    {
        Instantiate(enemy, transform.position, transform.rotation);
    }
}
