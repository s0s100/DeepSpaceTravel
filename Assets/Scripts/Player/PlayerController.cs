using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;

namespace Player
{
    public enum MovementType
    {
        LimitedSpeed,
        UnlimitedSpeed,
        Joystick,
        ImprovedMovement
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

        // Used to simplify control type with lambda function
        private Dictionary<MovementType, Action> actionDictionary = new();
        // Movement subscription
        private IDisposable m_movementSubscription;

        private PlayerData m_playerData;
        private Rigidbody2D m_rigidbody;
        private Vector2 m_moveTouchPos = Vector2.zero;
        private Vector2 m_startTouchPos = Vector2.zero;
        private Vector2 m_startPlayerPosition = Vector2.zero;


        private void PopulateActionDictionary()
        {
            actionDictionary[MovementType.LimitedSpeed] = () => LimitedSpeedMove();
            actionDictionary[MovementType.UnlimitedSpeed] = () => UnlimitedSpeedMove();
            actionDictionary[MovementType.Joystick] = () => JoystickMove();
            actionDictionary[MovementType.ImprovedMovement] = () => ImprovedMovement();
        }

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
            m_playerData = GetComponent<PlayerData>();
            PopulateActionDictionary();
            ImprovedMovementSetup();
        }

        private void Start()
        {
            SubscribeToTouchInputManager();
            SubscribeToMovement();
        }

        private void OnDestroy()
        {
            m_movementSubscription.Dispose();
        }

        private void SubscribeToMovement()
        {
            if (m_movementSubscription != null)
                m_movementSubscription.Dispose();

            var moveAction = actionDictionary[movementType];
            m_movementSubscription = Observable
                    .EveryUpdate()
                    .Subscribe(_ =>
                    {
                        moveAction();
                    });
        }

        private void SubscribeToTouchInputManager()
        {
            var touchInputManager = FindObjectOfType<TouchInputManager>();

            touchInputManager.OnTouchStart.Subscribe(touchPosition =>
            {
                m_moveTouchPos = ScreenInfo.GetWorldTouchPos(touchPosition);
                m_startTouchPos = m_moveTouchPos;
                m_startPlayerPosition = transform.position;
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

        private void StopMovement()
        {
            m_moveTouchPos = transform.position;
            m_startTouchPos = transform.position;
            m_startPlayerPosition = transform.position;
        }

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
            float difference = m_startTouchPos.x - m_moveTouchPos.x;
            newLocation.x = m_startPlayerPosition.x - difference;

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

        // Physics movement
        [SerializeField]
        private float f = 0.5f, c = 0.15f, r = 2f;
        private float k1, k2, k3;
        private float xSpeed;
        private float yPos, yVel, yAcc;

        private void ImprovedMovementSetup()
        {
            // Caclulate default constunts
            k1 = c / (float) (Math.PI * f);
            k2 = 1f / (float) Math.Pow(2f * Math.PI * f, 2);    
            k3 = (r * c) / (float) (2 * Math.PI * f);

            yPos = transform.position.x;
            yVel = 0.0f;
            yAcc = 0.0f;
        }

        // X Refers to basic limited movement while Y to improved movement
        private void ImprovedMovement()
        {
            float xDirection = CalculatDirection(m_moveTouchPos);
            if (xDirection != 0.0f)
                xSpeed = xDirection * m_playerData.Speed;
            else
                xSpeed = 0.0f;

            yPos += yVel * Time.deltaTime;
            yAcc = (m_moveTouchPos.x + k3 * xSpeed - yPos - k1 * yVel) / k2;
            yVel += yAcc * Time.deltaTime;

            Vector3 moveVector = (yPos - transform.position.x) * Vector3.right;
            Vector2 newLocation = transform.position + moveVector;

            if (CanMove(newLocation))
            {
                m_rigidbody.MovePosition(newLocation);
            }
        }
    }
}
