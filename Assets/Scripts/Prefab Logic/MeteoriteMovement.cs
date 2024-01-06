using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    private float m_maxRotationSpeed = 10.0f;
    private float m_xMoveLimit = 1.0f;
    private float m_yMoveLimit = 3.0f;
    private float m_deletionTime = 10.0f;

    Rigidbody2D rb;

    private IDisposable m_movementSubscription;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, m_deletionTime);
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

        float xMovement = UnityEngine.Random.Range(-m_xMoveLimit, m_xMoveLimit);
        float yMovement = UnityEngine.Random.Range(-m_yMoveLimit, 0);
        moveVector.x = xMovement;
        moveVector.y = yMovement;

        rb.velocity = moveVector;
        // rb.AddTorque(360.0f); Rotate object somehow
    }

    private void PeriodicMovement()
    {

    }

    private void OnDestroy()
    {
        m_movementSubscription?.Dispose();
    }
}
