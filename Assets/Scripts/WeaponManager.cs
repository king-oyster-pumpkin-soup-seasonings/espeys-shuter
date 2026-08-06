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
    [SerializeField] private GameObject bomb;

    // Invokes / Actions
    // [SerializeField] private UnityEvent<float> WeaponCooldownTime;
    public static Action<float> WeaponOnCooldown;


    // Projectile Spawning Related
    [SerializeField] private Transform soloBarrelPoint, leftBarrelPoint, rightBarrelPoint, laserPoint, bombPoint;
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
        if (fireRate == 0) fireRate = 0.8f;
        if (laserCooldownSet == 0) laserCooldownSet = 12f;
        laserCountdown = 0;
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

        // bomb key
        if (Input.GetKeyDown(KeyCode.K) && IfWeaponExists("WeaponBomb"))
        {
            Instantiate(bomb, bombPoint.position, bombPoint.rotation);
        }

        // laser key
        if (Input.GetKeyDown(KeyCode.L) && IfWeaponExists("WeaponLaser") && laserCountdown <= 0)
        {
            Instantiate(laser, laserPoint.position, laserPoint.rotation);
            laserCountdown = laserCooldownSet;
        }

        // bullet key
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J))
        {
            TriggerWeaponFire();
        }

        // auto double bullets
        if (Time.time >= nextFireTimePassive)
        {
            if (bullet != null && leftBarrelPoint != null && rightBarrelPoint != null)
            {
                if (IfWeaponExists("WeaponDoubleBullet"))
                {
                    Instantiate(bullet, leftBarrelPoint.transform.position, leftBarrelPoint.transform.rotation);
                    Instantiate(bullet, rightBarrelPoint.transform.position, rightBarrelPoint.transform.rotation);
                }

                nextFireTimePassive = (fireRate + (fireRate / 0.5f)) + Time.time;
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
            fireRate -= 0.125f;
            if (fireRate < 0.2f) fireRate = 0.2f;
        }
    }
}
