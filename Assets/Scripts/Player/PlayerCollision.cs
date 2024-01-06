using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;
using TMPro;

namespace Player
{
    public class PlayerCollision : MonoBehaviour
    {
        private IDisposable m_collisionSubscription;
        private Rigidbody2D m_rigidbody;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            m_collisionSubscription = this
                .OnCollisionEnter2DAsObservable()
                .Where(x => x.gameObject.tag == "Obstacle")
                .Subscribe(x =>
            {
                ObstacleTouch(x);
            });
        }

        private void ObstacleTouch(Collision2D other)
        {
            // Get rigidbody and calculate result force
            Rigidbody2D rigidbody = other.gameObject.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                float otherMass = rigidbody.mass;
                float thisMass = m_rigidbody.mass;

                print("Other mass is: " + otherMass);
                print("Rocker mass is: " + thisMass);

                float massDiff = otherMass / thisMass;
            }
        }

        private void OnDestroy()
        {
            m_collisionSubscription?.Dispose();
        }
    }
}