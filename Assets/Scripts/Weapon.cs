using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private bool weaponIsBeingSelected;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine selectCoroutine;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        weaponIsBeingSelected = false;
    }

    void Update()
    {
        if (GameManager.Instance.isSelectingWeapon == false) Destroy(gameObject);
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
    }
}
