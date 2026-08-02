using System;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector2[] directions;
    [SerializeField] private float movementSpeed;
    [SerializeField] private Rigidbody2D playerRigidBody;
    private Vector2 movementKey, previousPosition;

    void Start()
    {
        if (playerRigidBody == null) playerRigidBody = GetComponent<Rigidbody2D>();
        if (movementSpeed == 0) movementSpeed = 3;
    }

    void Update()
    {
        movementKey = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Move();
    }

    void Move()
    {
        previousPosition = transform.position;
        if (playerRigidBody.position.x >= 9f)
        {
            playerRigidBody.position = new Vector2(-9f, playerRigidBody.position.y);
        }
        else if (playerRigidBody.position.x <= -9f)
        {
            playerRigidBody.position = new Vector2(9f, playerRigidBody.position.y);
        }

        playerRigidBody.AddForce(movementKey * movementSpeed);
    }
}
