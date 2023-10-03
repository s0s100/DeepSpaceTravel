using UnityEngine;
using UniRx;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Defines how close to the touch player object moves
        private static float MIN_TOUCH_PLAYER_DISTANCE = 0.1f;

        private Vector2 moveTouchPos = Vector2.zero;
        private PlayerData playerData;

        private void Awake()
        {
            playerData = GetComponent<PlayerData>();
        }

        private void Start()
        {
            SubscribeToTouchInputManager();
            SubscribeToMovement();
        }

        // Called each frame to move player if required
        private void SubscribeToMovement()
        {
            // Periodical movement (should probably refactor)
            var periodicMovementObservator = Observable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    MovePlayer();
                });
        }

        private void SubscribeToTouchInputManager()
        {
            var touchInputManager = FindObjectOfType<TouchInputManager>();

            touchInputManager.OnTouchStart.Subscribe(touchPosition =>
            {
                moveTouchPos = ProperWorldTouchPos(touchPosition);
            });

            touchInputManager.OnTouchMove.Subscribe(touchPosition =>
            {
                moveTouchPos = ProperWorldTouchPos(touchPosition);
            });

            touchInputManager.OnTouchEnd.Subscribe(touchPosition =>
            {
                StopMovement();
            });
        }

        private void StopMovement()
        {
            moveTouchPos = transform.position;
        }

        private Vector2 ProperWorldTouchPos(Vector2 screenTouchPosition)
        {
            return Camera.main.ScreenToWorldPoint(screenTouchPosition);
        }

        private float CalculatDirection()
        {
            if (moveTouchPos.x > transform.position.x + MIN_TOUCH_PLAYER_DISTANCE)
                return 1.0f;
            else if (moveTouchPos.x < transform.position.x - MIN_TOUCH_PLAYER_DISTANCE)
                return -1.0f;
            
            return 0.0f;
        }

        private void MovePlayer()
        {
            float moveDirection = CalculatDirection();

            Vector2 moveVector = moveDirection * Time.deltaTime * playerData.Speed * Vector2.right;
            transform.Translate(moveVector);
        }
    }
}


