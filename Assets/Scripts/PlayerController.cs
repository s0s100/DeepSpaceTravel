using UnityEngine;
using UniRx;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        // Defines how close to the touch player object moves
        [SerializeField] private float minTouchDistance = 0.1f;

        private PlayerData m_playerData;
        private Vector2 m_moveTouchPos = Vector2.zero;

        private void Awake()
        {
            m_playerData = GetComponent<PlayerData>();
        }

        private void Start()
        {
            SubscribeToTouchInputManager();
            SubscribeToMovement();
        }

        // Called each frame to move player
        private void SubscribeToMovement()
        {
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
                m_moveTouchPos = ScreenInfo.GetWorldTouchPos(touchPosition);
            });

            touchInputManager.OnTouchMove.Subscribe(touchPosition =>
            {
                m_moveTouchPos = ScreenInfo.GetWorldTouchPos(touchPosition);
            });

            touchInputManager.OnTouchEnd.Subscribe(touchPosition =>
            {
                StopMovement();
            });
        }

        private void StopMovement()
        {
            m_moveTouchPos = transform.position;
        }

        private float CalculatDirection()
        {
            if (m_moveTouchPos.x > transform.position.x + minTouchDistance)
                return 1.0f;
            else if (m_moveTouchPos.x < transform.position.x - minTouchDistance)
                return -1.0f;
            
            return 0.0f;
        }

        private void MovePlayer()
        {
            float moveDirection = CalculatDirection();
            if (moveDirection == 0.0f)
                return;

            float movement = moveDirection * Time.deltaTime * m_playerData.Speed;
            Vector2 moveVector = new(movement, 0.0f);
            if (CanMove(moveVector)) 
                transform.Translate(moveVector);
        }

        private bool CanMove(Vector2 moveVector)
        {
            // Divided by 2 to have a bounary between screen and the right side of the screen equal to 2
            float playerXBoundary = m_playerData.CalculateXSize() / 2;
            float maxXScreenPos = ScreenInfo.GetMaxXPos();
            float minXScreenPos = ScreenInfo.GetMinXPos();

            bool canMoveLeft = (minXScreenPos + playerXBoundary < transform.position.x) || (moveVector.x > 0.0f);
            bool canMoveRight = (maxXScreenPos - playerXBoundary > transform.position.x) || (moveVector.x < 0.0f);

            return canMoveLeft && canMoveRight;
        }
    }
}


