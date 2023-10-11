using UnityEngine;
using UniRx;
using System;

namespace Player
{
    public enum MovementType
    {
        LimitedSpeed,
        UnlimitedSpeed,
        Joystick
    }

    public class PlayerController : MonoBehaviour
    {
        [Header("Change these values before running the game")]
        [Tooltip("Min required distance between player position / start touch position and current touch position to move player")]
        [SerializeField] private float minTouchDistance = 0.1f;

        [Tooltip("Limited Speed - Default movement to the touch position \n" +
            "Unlimited Speed - Movement with no speed limit to the touch position \n" +
            "Joystick - Movement which stores first touch location as joystick center")]
        [SerializeField] private MovementType movementType = MovementType.LimitedSpeed;

        private PlayerData m_playerData;
        private Vector2 m_moveTouchPos = Vector2.zero;
        private Vector2 m_startTouchPos = Vector2.zero;

        private IDisposable m_movementSubscription;

        private void Awake()
        {
            m_playerData = GetComponent<PlayerData>();
        }

        private void Start()
        {
            SubscribeToTouchInputManager();
            SubscribeToMovement();
        }

        private void SubscribeToMovement()
        {
            if (m_movementSubscription != null)
            {
                m_movementSubscription.Dispose();
            }

            switch (movementType)
            {
                case MovementType.LimitedSpeed:
                    m_movementSubscription = Observable
                    .EveryUpdate()
                    .Subscribe(_ =>
                    {
                        LimitedSpeedMove();
                    });
                    break;
                case MovementType.UnlimitedSpeed:
                    m_movementSubscription = Observable
                    .EveryUpdate()
                    .Subscribe(_ =>
                    {
                        UnlimitedSpeedMove();
                    });
                    break;
                case MovementType.Joystick:
                    m_movementSubscription = Observable
                    .EveryUpdate()
                    .Subscribe(_ =>
                    {
                        JoystickMove();
                    });
                    break;
                default:
                    Debug.LogWarning("Unknown Movement Type: " + movementType);
                    break;
            }
        }

        private void SubscribeToTouchInputManager()
        {
            var touchInputManager = FindObjectOfType<TouchInputManager>();

            touchInputManager.OnTouchStart.Subscribe(touchPosition =>
            {
                m_moveTouchPos = ScreenInfo.GetWorldTouchPos(touchPosition);
                m_startTouchPos = m_moveTouchPos;
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
            m_startTouchPos = transform.position;
        }

        // xComparePosition is the point in world coordinates according to which direction is calculated
        private float CalculatDirection(Vector2 comparePosition)
        {
            if (m_moveTouchPos.x > comparePosition.x + minTouchDistance)
                return 1.0f;
            else if (m_moveTouchPos.x < comparePosition.x - minTouchDistance)
                return -1.0f;
            
            return 0.0f;
        }

        private void LimitedSpeedMove()
        {
            float moveDirection = CalculatDirection(transform.position);
            if (moveDirection == 0.0f)
                return;

            float movement = moveDirection * Time.deltaTime * m_playerData.Speed;
            Vector3 moveVector = movement * Vector2.right;
            Vector2 newLocation = transform.position + moveVector;

            if (CanMove(newLocation)) 
                transform.Translate(moveVector);
        }

        private void UnlimitedSpeedMove()
        {
            Vector2 newLocation = transform.position;
            newLocation.x = m_moveTouchPos.x;

            if (CanMove(newLocation))
                transform.position = newLocation;
        }

        private void JoystickMove()
        {
            float moveDirection = CalculatDirection(m_startTouchPos);
            if (moveDirection == 0.0f)
                return;

            float movement = moveDirection * Time.deltaTime * m_playerData.Speed;
            Vector3 moveVector = movement * Vector2.right;
            Vector2 newLocation = transform.position + moveVector;

            if (CanMove(newLocation))
                transform.Translate(moveVector);
        }

        private bool CanMove(Vector2 newPosition)
        {
            // Divided by 2 to have a bounary between screen and the right side of the screen equal to 2
            float playerXBoundary = m_playerData.CalculateXSize() / 2;
            float maxXScreenPos = ScreenInfo.GetMaxXPos();
            float minXScreenPos = ScreenInfo.GetMinXPos();

            bool canMoveLeft = (minXScreenPos + playerXBoundary < newPosition.x);
            bool canMoveRight = (maxXScreenPos - playerXBoundary > newPosition.x);

            return canMoveLeft && canMoveRight;
        }

        private void OnDestroy()
        {
            m_movementSubscription.Dispose();
        }
    }
}


