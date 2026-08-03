using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    // Game Objects
    public List<GameObject> weaponSet;
    [SerializeField] private GameObject bullet;

    // Projectile Spawning Related
    [SerializeField] private Transform soloBarrelPoint;
    [SerializeField] private float fireRate;
    private float nextFireTime;

    // Weapon Management
    public int currentWeaponUse;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        weaponSet.Clear();
        if (fireRate == 0) fireRate = 1;
        currentWeaponUse = 1;
    }

    void Update()
    {
    }

    public void TriggerWeaponFire()
    {
        if (Time.time >= nextFireTime)
        {
            if (bullet != null && soloBarrelPoint != null)
            {
                Instantiate(bullet, soloBarrelPoint.transform.position, soloBarrelPoint.transform.rotation);
                nextFireTime = fireRate + Time.time;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUpBulletSpeed"))
        {
            Destroy(other.gameObject);
            if (fireRate != 0) fireRate -= 0.2f;
        }
    }
}
