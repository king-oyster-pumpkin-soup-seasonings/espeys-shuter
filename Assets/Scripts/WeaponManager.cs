using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    // Game Objects
    public List<GameObject> weaponSet;
    [SerializeField] private GameObject bullet;

    // Projectile Spawning Related
    [SerializeField] private Transform soloBarrelPoint, leftBarrelPoint, rightBarrelPoint;
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
        // weaponSet.Clear();
        if (fireRate == 0) fireRate = 1;
        currentWeaponUse = 1;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J))
        {
            TriggerWeaponFire();
        }
    }

    public void TriggerWeaponFire()
    {
        if (Time.time >= nextFireTime)
        {
            if (bullet != null && soloBarrelPoint != null)
            {
                Instantiate(bullet, soloBarrelPoint.transform.position, soloBarrelPoint.transform.rotation);

                foreach (GameObject weapon in weaponSet)
                {
                    if (weapon.CompareTag("WeaponDoubleBullet"))
                    {
                        Instantiate(bullet, leftBarrelPoint.transform.position, leftBarrelPoint.transform.rotation);
                        Instantiate(bullet, rightBarrelPoint.transform.position, rightBarrelPoint.transform.rotation);
                    }
                }

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
