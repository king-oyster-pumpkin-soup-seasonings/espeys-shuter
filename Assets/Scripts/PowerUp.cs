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

    void Update()
    {
    }

    private IEnumerator StartPowerUpLifeCouroutine()
    {
        yield return new WaitForSeconds(powerUpLifespan);
        Destroy(gameObject);
    }
}
