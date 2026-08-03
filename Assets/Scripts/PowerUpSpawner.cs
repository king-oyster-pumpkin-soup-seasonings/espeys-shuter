using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private float spawnInterval;
    [SerializeField] private GameObject[] powerups;
    private float time;

    void Start()
    {
        if (spawnInterval == 0) spawnInterval = 3f;
        time = 0;
    }

    void Update()
    {
        if (GameManager.Instance.isMessageDone == false ||
            powerups == null) return;

        if (time < spawnInterval)
        {
            time += Time.deltaTime;
        }
        else
        {
            transform.position = new Vector2(Random.Range(-9f, 9f), transform.position.y);
            time = 0;
            spawnInterval = Random.Range(3f, 10f);
            Instantiate(powerups[Random.Range(0, powerups.Length)], transform.position, transform.rotation);
        }
    }
}
