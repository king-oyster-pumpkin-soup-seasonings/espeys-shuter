using System.Collections;
using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float bombLifespan;
    [SerializeField] private Rigidbody2D bombRB;
    [SerializeField] private float bombSpeed, bombRotationSpeed;

    [SerializeField] private float bombCountdown;
    [SerializeField] private TextMeshPro textBombCountdown;
    // [SerializeField] private Transform bombCountdownLabelT;


    // visual bomb
    [SerializeField] private Rigidbody2D bombChildRB;

    // bomb explosion
    [SerializeField] private Transform bombExplosionT;
    [SerializeField] private CircleCollider2D bombCollider;
    [SerializeField] private Rigidbody2D visualBombRB;
    [SerializeField] private SpriteRenderer bombExplosionSR;

    void Start()
    {
        if (bombCountdown == 0) bombCountdown = 3f;
        bombExplosionSR.enabled = false;
        bombRB = GetComponent<Rigidbody2D>();
        if (bombSpeed == 0) bombSpeed = 0.2f;
        if (bombLifespan == 0) bombLifespan = 10f;
        if (bombRotationSpeed == 0) bombRotationSpeed = 50f;

        bombCollider.enabled = false;
        bombRB.linearVelocity = Random.insideUnitCircle.normalized * bombSpeed;
        bombRB.angularVelocity = bombRotationSpeed;
        // visualBombRB.angularVelocity = bombRotationSpeed;

        StartCoroutine(TriggerExplosion());
    }


    // private void LateUpdate()
    // {
    //     if (bombCountdownLabelT != null) bombCountdownLabelT.rotation = Quaternion.identity;
    // }

    private IEnumerator TriggerExplosion()
    {
        yield return new WaitForSeconds(bombCountdown);

        bombRB.linearVelocity = Vector2.zero;
        bombRB.angularVelocity = 0;
        bombCollider.enabled = true;
        Vector2 explosionSize = new Vector2(0.2f, 0.2f);
        bombExplosionSR.enabled = true;
        bombExplosionT = transform;
        bombExplosionT.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        for (int i = 0; i < 10; i++)
        {
            bombExplosionT.localScale = explosionSize * i;
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    void Update()
    {
        bombChildRB.linearVelocity = new Vector2(bombRB.linearVelocity.x, bombRB.linearVelocity.y);

        if (bombCountdown > 0)
        {
            bombCountdown -= Time.deltaTime;
            textBombCountdown.text = Mathf.CeilToInt(bombCountdown).ToString();
        }
        else textBombCountdown.text = "";
    }
}
