using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private bool weaponIsBeingSelected;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private TextMeshPro keyAndCooldownText;
    private Coroutine selectCoroutine;
    private Vector2 proceduralWeaponSlotPosition;

    private void OnEnable()
    {
        WeaponManager.WeaponOnCooldown += UpdateKeyAndCooldownTimerLabel;
    }

    private void OnDisable()
    {
        WeaponManager.WeaponOnCooldown -= UpdateKeyAndCooldownTimerLabel;
    }

    public void UpdateKeyAndCooldownTimerLabel(float cooldownTime)
    {
        if (keyAndCooldownText == null) return;

        if (cooldownTime > 0)
        {
            keyAndCooldownText.text = ": " + Math.Ceiling(cooldownTime);
        }
        else
        {
            keyAndCooldownText.text = ": L"; // Shows "L" when cooldown hits 0
        }
    }

    void Start()
    {
        proceduralWeaponSlotPosition = new Vector2(-8.35f, -3.75f);
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        weaponIsBeingSelected = false;
    }

    void Update()
    {
        if (GameManager.Instance.isSelectingWeapon == false)
        {
            if (!WeaponManager.Instance.weaponSet.Contains(gameObject))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !weaponIsBeingSelected)
        {
            selectCoroutine = StartCoroutine(TriggerBeingSelect());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (selectCoroutine != null)
        {
            StopCoroutine(selectCoroutine);
            selectCoroutine = null;
        }

        weaponIsBeingSelected = false;
        Color currentColor = spriteRenderer.color;
        currentColor.a = 1f;
        spriteRenderer.color = currentColor;
    }

    IEnumerator TriggerBeingSelect()
    {
        weaponIsBeingSelected = true;
        Color currentColor = spriteRenderer.color;
        currentColor.a = 0.5f;
        spriteRenderer.color = currentColor;

        yield return new WaitForSeconds(1f);

        WeaponFinallySelected();
    }

    void WeaponFinallySelected()
    {
        WeaponManager.Instance.weaponSet.Add(gameObject);
        GameManager.Instance.isSelectingWeapon = false;
        GameManager.Instance.TriggerWeaponSelectionComplete();
        for (int i = 0; i < WeaponManager.Instance.weaponSet.Count; i++)
        {
            transform.position = proceduralWeaponSlotPosition;
            proceduralWeaponSlotPosition = new Vector2
            (
                proceduralWeaponSlotPosition.x,
                proceduralWeaponSlotPosition.y + 0.75f
            );
        }

        transform.localScale = new Vector2(2.5f, 2.5f);

        boxCollider.enabled = false;
        spriteRenderer.sortingOrder = 3;
    }
}
