using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D missileRB;
    [SerializeField] private float missileSpeed;
    [SerializeField] private float missileTurnSpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask enemyLayer;

    private Transform targetEnemyTransformToFollow;

    void Start()
    {
        if (missileRB == null) missileRB = GetComponent<Rigidbody2D>();
        if (missileSpeed == 0) missileSpeed = 4f;
        if (missileTurnSpeed == 0) missileTurnSpeed = 240f;
        if (detectionRadius == 0) detectionRadius = 20f;

        FindNearestEnemy();
    }

    private void FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        float currentMinimumDistance = Mathf.Infinity;
        targetEnemyTransformToFollow = null;

        foreach (Collider2D enemyCollider in enemies)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemyCollider.transform.position);
            if (distanceToEnemy < currentMinimumDistance)
            {
                currentMinimumDistance = distanceToEnemy;
                targetEnemyTransformToFollow = enemyCollider.transform;
            }
        }
    }


    private void FixedUpdate()
    {
        if (targetEnemyTransformToFollow == null)
        {
            missileRB.linearVelocity = transform.up * missileSpeed;
            missileRB.angularVelocity = 0f;
            return;
        }

        Vector2 direction = targetEnemyTransformToFollow.position - missileRB.transform.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        missileRB.angularVelocity = -rotateAmount * missileTurnSpeed;
        missileRB.linearVelocity = transform.up * missileSpeed;
    }

    void Update()
    {
        if (missileRB.position.y >= 6f || missileRB.position.y <= -6f ||
            missileRB.position.x >= 9.5f || missileRB.position.x <= -9.5f)
            Destroy(gameObject);
    }

    void LockOnTarget()
    {
    }
}
