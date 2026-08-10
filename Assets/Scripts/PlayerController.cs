using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite explosionSprite;
    private Vector2 movementKey;
    private bool isExploding;


    void Start()
    {
        isExploding = false;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (playerRigidBody == null) playerRigidBody = GetComponent<Rigidbody2D>();
        if (movementSpeed == 0) movementSpeed = 8;
        if (playerRigidBody != null) playerRigidBody.linearDamping = 1.25f;

        GoToSpawnPoint();
    }

    void GoToSpawnPoint()
    {
        transform.position = new Vector2(0, -3.5f);
    }

    void FixedUpdate()
    {
        if (isExploding) return;

        movementKey = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move();
    }

    void Move()
    {
        if (playerRigidBody.position.x >= 9f)
        {
            playerRigidBody.position = new Vector2(-9f, playerRigidBody.position.y);
        }
        else if (playerRigidBody.position.x <= -9f)
        {
            playerRigidBody.position = new Vector2(9f, playerRigidBody.position.y);
        }

        if (playerRigidBody.position.y >= 4.67f)
        {
            playerRigidBody.position = new Vector2(playerRigidBody.position.x, 4.67f);
            playerRigidBody.linearVelocity = new Vector2(playerRigidBody.linearVelocity.x, 0);
        }

        else if (playerRigidBody.position.y <= -4.67f)
        {
            playerRigidBody.position = new Vector2(playerRigidBody.position.x, -4.67f);
            playerRigidBody.linearVelocity = new Vector2(playerRigidBody.linearVelocity.x, 0);
        }

        playerRigidBody.AddForce(movementKey * movementSpeed);
    }


    public void Explode()
    {
        isExploding = true;

        playerRigidBody.linearVelocity = Vector2.zero;

        spriteRenderer.sprite = explosionSprite;

        Destroy(gameObject, 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerUpMovementSpeed"))
        {
            Destroy(other.gameObject);
            movementSpeed += 7.5f;
            playerRigidBody.linearDamping += 0.33f;

            if (playerRigidBody.linearDamping >= 4.5f) playerRigidBody.linearDamping = 4.5f;
            if (movementSpeed >= 50f) movementSpeed = 50f;
        }
    }
}
