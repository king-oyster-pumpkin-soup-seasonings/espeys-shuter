using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Transform laserPoint;

    void Start()
    {
        if (laserPoint == null) laserPoint = GameObject.FindGameObjectWithTag("LaserPoint").transform;
        StartCoroutine(LaserOnFire());
    }

    void Update()
    {
        if (laserPoint != null) transform.position = laserPoint.transform.position;
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
}
