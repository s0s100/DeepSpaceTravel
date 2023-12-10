using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    private float maxRotationSpeed = 10.0f;
    private float xMoveLimit = 1.0f;
    private float yMoveLimit = 3.0f;
    private float deletionTime = 10.0f;

    Rigidbody2D rb;

    private IDisposable m_movementSubscription;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, deletionTime);
        DefaultForceSetup();

        m_movementSubscription = Observable
                    .EveryUpdate()
                    .Subscribe(_ =>
                    {
                        PeriodicMovement();
                    });
    }

    private void DefaultForceSetup()
    {
        Vector2 moveVector = new();

        float xMovement = UnityEngine.Random.Range(-xMoveLimit, xMoveLimit);
        float yMovement = UnityEngine.Random.Range(-yMoveLimit, 0);
        moveVector.x = xMovement;
        moveVector.y = yMovement;

        Debug.Log("Move vector: " + moveVector);

        rb.velocity = moveVector;
        // rb.AddTorque(360.0f); Rotate object somehow
    }

    private void PeriodicMovement()
    {
    }

    private void OnDestroy()
    {
        m_movementSubscription.Dispose();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            Debug.Log("Entered");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            Debug.Log("Triggered");
    }
}
