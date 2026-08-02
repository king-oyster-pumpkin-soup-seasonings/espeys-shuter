using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletLifespan;
    [SerializeField] private Rigidbody2D bulletRB;
    [SerializeField] private float bulletSpeed;

    void Start()
    {
        bulletRB = GetComponent<Rigidbody2D>();
        if (bulletSpeed == 0) bulletSpeed = 5f;
        if (bulletLifespan == 0) bulletLifespan = 10f;

        bulletRB.linearVelocity = transform.up * bulletSpeed;
        Destroy(gameObject, bulletLifespan);
    }

    void Update()
    {
        if (bulletRB.position.y >= 6f || bulletRB.position.y <= -6f ||
            bulletRB.position.x >= 9.5f || bulletRB.position.x <= -9.5f)
            Destroy(gameObject);
    }
}
