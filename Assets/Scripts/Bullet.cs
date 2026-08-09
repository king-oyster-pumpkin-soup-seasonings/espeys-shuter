using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D bulletRB;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private SpriteRenderer bulletSR;
    [SerializeField] private BoxCollider2D bulletCollider;
    [SerializeField] private Sprite hitSprite;

    void Start()
    {
        bulletSR = GetComponent<SpriteRenderer>();
        bulletRB = GetComponent<Rigidbody2D>();
        if (bulletSpeed == 0) bulletSpeed = 6f;

        bulletRB.linearVelocity = transform.up * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("PlayerBullet") && (
                other.CompareTag("Asteroid") || other.CompareTag("Enemy") ||
                other.CompareTag("EnemyFighter") || other.CompareTag("EnemySprayer")))
        {
            bulletSpeed = 0;
            bulletRB.linearVelocity = Vector2.zero;
            bulletSR.sprite = hitSprite;
            bulletCollider.enabled = false;
            transform.localScale = new Vector2(1.5f, 1.5f);
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Destroy(gameObject, 0.1f);
        }
    }

    void Update()
    {
        if (bulletRB.position.y >= 6f || bulletRB.position.y <= -6f ||
            bulletRB.position.x >= 9.5f || bulletRB.position.x <= -9.5f)
            Destroy(gameObject);
    }
}
