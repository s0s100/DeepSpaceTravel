using System;
using UniRx;
using UnityEngine;

namespace Core
{
    public class MeteoriteMovement : MonoBehaviour
    {
        // Move to config later on
        private readonly float _maxRotationSpeed = 10.0f;
        private readonly float _xMoveLimit = 1.0f;
        private readonly float _yMoveLimit = 3.0f;
        private readonly float _deletionTime = 10.0f;

        Rigidbody2D rb;

        private IDisposable m_movementSubscription;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            Destroy(gameObject, _deletionTime);
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

            float xMovement = UnityEngine.Random.Range(-_xMoveLimit, _xMoveLimit);
            float yMovement = UnityEngine.Random.Range(-_yMoveLimit, 0);
            moveVector.x = xMovement;
            moveVector.y = yMovement;

            rb.velocity = moveVector;
            // rb.AddTorque(360.0f); Rotate object somehow
        }

        private void PeriodicMovement() { }

        private void OnDestroy()
        {
            m_movementSubscription.Dispose();
        }
    }
}