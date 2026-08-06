using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    // Game Objects
    public List<GameObject> weaponSet;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject laser;

    // Invokes / Actions
    // [SerializeField] private UnityEvent<float> WeaponCooldownTime;
    public static Action<float> WeaponOnCooldown;


    // Projectile Spawning Related
    [SerializeField] private Transform soloBarrelPoint, leftBarrelPoint, rightBarrelPoint, laserPoint;
    [SerializeField] private float fireRate;
    private float nextFireTime, nextFireTimePassive, laserCountdown, laserCooldownSet;

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
        if (fireRate == 0) fireRate = 1f;
        if (laserCooldownSet == 0) laserCooldownSet = 10f;
        laserCountdown = laserCooldownSet;
        currentWeaponUse = 1;
    }

    void Update()
    {
        if (laserCountdown > 0)
        {
            laserCountdown -= Time.deltaTime;
            WeaponOnCooldown?.Invoke(laserCountdown);
            // Debug.Log($"LASERTIME: {laserCountdown}");
        }
        else if (laserCountdown != 0)
        {
            laserCountdown = 0;
            WeaponOnCooldown?.Invoke(0);
        }

        if (Input.GetKeyDown(KeyCode.L) && IfWeaponExists("WeaponLaser") && laserCountdown <= 0)
        {
            Instantiate(laser, laserPoint.position, laserPoint.rotation);
            laserCountdown = laserCooldownSet;
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J))
        {
            TriggerWeaponFire();
        }


        if (Time.time >= nextFireTimePassive)
        {
            if (bullet != null && soloBarrelPoint != null)
            {
                if (IfWeaponExists("WeaponDoubleBullet"))
                {
                    Instantiate(bullet, leftBarrelPoint.transform.position, leftBarrelPoint.transform.rotation);
                    Instantiate(bullet, rightBarrelPoint.transform.position, rightBarrelPoint.transform.rotation);
                }

                nextFireTimePassive = (fireRate + 1) + Time.time;
            }
        }
    }

    bool IfWeaponExists(string weaponTag)
    {
        foreach (GameObject weapon in weaponSet)
        {
            if (weapon.CompareTag(weaponTag)) return true;
        }

        return false;
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
