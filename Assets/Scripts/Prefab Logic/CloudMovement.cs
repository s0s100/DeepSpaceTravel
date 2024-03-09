using UnityEngine;
using UniRx;
using System;

public class CloudMovement : MonoBehaviour
{
    public float MovementSpeed { get; set; } = 0.0f;
    public float DeletionTime { get; set; } = 10.0f;

    private IDisposable _movementSubscription;

    void Start()
    {
        Destroy(gameObject, DeletionTime);

        _movementSubscription = Observable
                    .EveryUpdate()
                    .Subscribe(_ =>
                    {
                        PeriodicMovement();
                    });
    }

    private void PeriodicMovement()
    {
        Vector2 moveVector = MovementSpeed * Time.deltaTime * Vector2.down;
        transform.Translate(moveVector);
    }

    private void OnDestroy()
    {
        _movementSubscription.Dispose();   
    }
}
