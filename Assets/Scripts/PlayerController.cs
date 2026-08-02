using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D playerRigidBody;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float fireRate;
    [SerializeField] private Transform soloBarrelPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite explosionSprite;
    private Vector2 movementKey;
    private float nextFireTime;
    private bool isExploding;

    void Start()
    {
        isExploding = false;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (fireRate == 0) fireRate = 1;
        if (playerRigidBody == null) playerRigidBody = GetComponent<Rigidbody2D>();
        if (movementSpeed == 0) movementSpeed = 3;
    }

    void Update()
    {
        if (isExploding) return;

        movementKey = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move();

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J))
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
}
