using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    [SerializeField]
    private float maxRotationSpeed = 10.0f;

    [SerializeField]
    private float movementSpeed = 5.0f;

    [SerializeField]
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
        Vector2 moveVector = Vector2.down * movementSpeed;
        float yMovement = UnityEngine.Random.Range(-movementSpeed, movementSpeed);
        moveVector += Vector2.right * yMovement;

        rb.AddForce(moveVector);
        // rb.AddTorque(360.0f); Rotate object somehow
    }

    private void PeriodicMovement()
    {
    }

    private void OnDestroy()
    {
        m_movementSubscription.Dispose();
    }
}
