using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] enemies;
    private float time;

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 1f;
        time = 0;
    }

    void Update()
    {
        if (time < spawnInterval)
        {
            time += Time.deltaTime;
        }
        else
        {
            transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
            time = 0;
            Instantiate(enemies[0], transform.position, transform.rotation);
        }
    }
}
