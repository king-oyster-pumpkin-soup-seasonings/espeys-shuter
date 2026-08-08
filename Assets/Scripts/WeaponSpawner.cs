using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] weapons;
    private GameObject[] availableWeaponsToChoose;
    private List<GameObject> unacquiredWeapons = new();
    public bool isDone;


    public static WeaponSpawner Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        availableWeaponsToChoose = new GameObject[2];
    }

    void Start()
    {
        unacquiredWeapons.Clear();
        isDone = false;
    }

    public void TriggerWeaponChooser()
    {
        // Debug.Log("TriggerWeaponChooser called");
        isDone = false;
        AvailWeapons();
        DisplayWeaponChooser();
    }

    void AvailWeapons()
    {
        if (weapons is null) return;

        // Debug.Log($"AVAIL WEAPONS TRIGGERED\n"); // debug

        unacquiredWeapons.Clear();
        bool isWeaponAlreadyOwned;
        for (int i = 0; i < weapons.Length; i++)
        {
            isWeaponAlreadyOwned = false;
            foreach (GameObject weapon in WeaponManager.Instance.weaponSet)
            {
                if (weapon.tag == weapons[i].tag)
                {
                    isWeaponAlreadyOwned = true;
                    break;
                }
            }

            if (!isWeaponAlreadyOwned) unacquiredWeapons.Add(weapons[i]);
        }

        if (unacquiredWeapons.Count <= 0) return;

        if (unacquiredWeapons.Count > 1)
        {
            int randomIndex1 = Random.Range(0, unacquiredWeapons.Count);
            int randomIndex2;

            do randomIndex2 = Random.Range(0, unacquiredWeapons.Count);
            while (randomIndex2 == randomIndex1);

            availableWeaponsToChoose[0] = unacquiredWeapons[randomIndex1];
            availableWeaponsToChoose[1] = unacquiredWeapons[randomIndex2];
        }
        else if (unacquiredWeapons.Count == 1) availableWeaponsToChoose[0] = unacquiredWeapons[0];
    }

    void DisplayWeaponChooser()
    {
        if (unacquiredWeapons.Count > 1)
        {
            Instantiate(availableWeaponsToChoose[0], new Vector2(1.5f, 2f), Quaternion.identity);
            Instantiate(availableWeaponsToChoose[1], new Vector2(-1.5f, 2f), Quaternion.identity);

            isDone = true;
        }

        if (unacquiredWeapons.Count == 1)
        {
            Instantiate(availableWeaponsToChoose[0], new Vector2(0, 2f), Quaternion.identity);

            isDone = true;
        }
        else if (unacquiredWeapons.Count < 1)
            Debug.Log($"Display Fail: UnacquiredWeapons is {unacquiredWeapons.Count}");
    }
}
