using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private bool weaponIsBeingSelected;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    private Coroutine selectCoroutine;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        weaponIsBeingSelected = false;
    }

    void Update()
    {
        if (GameManager.Instance.isSelectingWeapon == false)
        {
            foreach (GameObject weapon in WeaponManager.Instance.weaponSet)
            {
                if (!weapon.CompareTag(gameObject.tag))
                {
                    Destroy(gameObject);
                }
                else
                {
                    transform.localScale = new Vector2(2.5f, 2.5f);
                    transform.position = new Vector2(-8.35f, -3.75f);
                    boxCollider.enabled = false;
                    spriteRenderer.sortingOrder = 3;
                    return;
                }
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
    }
}
