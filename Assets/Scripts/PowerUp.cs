using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Rigidbody2D powerUpRB;
    [SerializeField] private float movementSpeed, powerUpLifespan;

    void Start()
    {
        if (movementSpeed == 0) movementSpeed = 3f;
        if (powerUpLifespan == 0) powerUpLifespan = 5f;

        powerUpRB.linearVelocity = Vector2.down * movementSpeed;
        powerUpRB.AddForceX(Random.Range(-25f, 25f) * movementSpeed);
        StartCoroutine(StartPowerUpLifeCouroutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log("POWERUP TOUCHES PLAYER");
            if (gameObject.CompareTag("PowerUpMaxHealth") ||
                gameObject.CompareTag("PowerUpBulletSpeed") ||
                gameObject.CompareTag("PowerUpMovementSpeed"))
                PowerUpSpawner.Instance.permaPowerUpCaughtDuringTheWave++;
        }
    }

    private IEnumerator StartPowerUpLifeCouroutine()
    {
        yield return new WaitForSeconds(powerUpLifespan);
        Destroy(gameObject);
    }
}
