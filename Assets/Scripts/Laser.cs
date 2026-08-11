using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Laser : MonoBehaviour
{
    [SerializeField] private SpriteRenderer laserBurst;

    // [SerializeField] private GameObject laserUser;
    [SerializeField] private Transform laserPoint;
    [SerializeField] private float currentAngle, rotationSpeed;
    private bool switchEndpoint, isRotatable;

    public static Action LaserIsDone;

    void Start()
    {
        if (laserBurst == null)
            laserBurst = GameObject.FindGameObjectWithTag("BossLaserBurst")?.GetComponent<SpriteRenderer>();
        if (laserBurst != null) laserBurst.enabled = false;

        // Player Laser setup
        if (gameObject.CompareTag("Laser"))
        {
            if (laserPoint == null)
            {
                GameObject targetPoint = GameObject.FindGameObjectWithTag("LaserPoint");
                if (targetPoint != null) laserPoint = targetPoint.transform;
            }

            if (laserPoint != null)
            {
                transform.SetParent(laserPoint);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }

            StartCoroutine(LaserOnFire());
        }
        // Enemy Laser setup
        else if (gameObject.CompareTag("EnemyLaser"))
        {
            if (laserPoint == null)
            {
                GameObject targetPoint = GameObject.FindGameObjectWithTag("BossLaserPoint");
                if (targetPoint != null) laserPoint = targetPoint.transform;
            }

            if (laserPoint != null)
            {
                transform.SetParent(laserPoint);
                transform.localPosition = new Vector3(0f, 5f, 0f);
                laserPoint.eulerAngles = new Vector3(0, 0, 180f);
            }

            if (laserBurst != null) laserBurst.enabled = true;
            StartCoroutine(EnemyLaserOnFire());
        }

        isRotatable = false;
        switchEndpoint = (Random.value < 0.5f);
        rotationSpeed = Random.Range(0.25f, 0.5f);
    }

    void Update()
    {
        if (laserPoint != null)
        {
            if (gameObject.CompareTag("EnemyLaser"))
            {
                transform.position = laserPoint.TransformPoint(new Vector3(0f, 5f, 0f));
                transform.rotation = laserPoint.rotation;
            }
            else transform.position = laserPoint.position;
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.CompareTag("EnemyLaser") && isRotatable)
        {
            if (!switchEndpoint)
            {
                currentAngle += 0.5f;
                laserPoint.Rotate(0f, 0f, rotationSpeed);

                if (currentAngle >= 67)
                {
                    switchEndpoint = true;
                }
            }
            else
            {
                currentAngle -= 0.5f;
                laserPoint.Rotate(0f, 0f, -rotationSpeed);

                // Check if rotation returned to 0 degrees
                if (currentAngle <= -67)
                {
                    switchEndpoint = false;
                }
            }
        }
    }

    private IEnumerator LaserOnFire()
    {
        transform.localScale = new Vector2(0.05f, transform.localScale.y);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.05f);
            transform.localScale = new Vector2(0.2f, transform.localScale.y);
            yield return new WaitForSeconds(0.05f);
            transform.localScale = new Vector2(0.1f, transform.localScale.y);
        }

        transform.localScale = new Vector2(0.05f, transform.localScale.y);
        yield return new WaitForSeconds(0.25f);

        Destroy(gameObject);
    }

    private IEnumerator EnemyLaserOnFire()
    {
        transform.localScale = new Vector2(0f, transform.localScale.y);
        yield return new WaitForSeconds(2.5f);

        laserBurst.transform.localScale = new Vector2(0.1f, 0.1f);
        transform.localScale = new Vector2(0.05f, transform.localScale.y);
        yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < Random.Range(50, 100); i++)
        {
            yield return new WaitForSeconds(0.05f);
            laserBurst.transform.localScale = new Vector2(0.2f, 0.2f);
            transform.localScale = new Vector2(0.2f, transform.localScale.y);
            yield return new WaitForSeconds(0.05f);
            transform.localScale = new Vector2(0.1f, transform.localScale.y);
            isRotatable = true;
        }

        transform.localScale = new Vector2(0.05f, transform.localScale.y);
        laserBurst.transform.localScale = new Vector2(0.1f, 0.1f);
        yield return new WaitForSeconds(0.25f);
        laserBurst.enabled = false;
        LaserIsDone?.Invoke();

        Destroy(gameObject);
    }
}
