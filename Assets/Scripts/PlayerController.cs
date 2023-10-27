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
        TestMovement
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
        private Vector2 m_moveTouchPos = Vector2.zero;
        private Vector2 m_startTouchPos = Vector2.zero;
        private Vector2 m_startPlayerPosition = Vector2.zero;


        private void PopulateActionDictionary()
        {
            actionDictionary[MovementType.LimitedSpeed] = () => LimitedSpeedMove();
            actionDictionary[MovementType.UnlimitedSpeed] = () => UnlimitedSpeedMove();
            actionDictionary[MovementType.Joystick] = () => JoystickMove();
            actionDictionary[MovementType.TestMovement] = () => TestMovement();
        }

        private void Awake()
        {
            m_playerData = GetComponent<PlayerData>();
            PopulateActionDictionary();
            TestStart();
        }

        private void Start()
        {
            SubscribeToTouchInputManager();
            SubscribeToMovement();
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

        private void StopMovement()
        {
            m_moveTouchPos = transform.position;
            m_startTouchPos = transform.position;
            m_startPlayerPosition = transform.position;
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


        // Not optimized version
        public float f = 0.5f, c = 0.15f, r = 2f;
        private double k1, k2, k3;
        private float xCur, xNew, Vx;
        private float y, Vy;
        private float t;

        private void TestStart()
        {
            // Defining basic values
            y = transform.position.x;
            t = Time.deltaTime;
        }

        private void TestMovement()
        {
            // Caclulate default constunts
            k1 = c / (Math.PI * f);
            k2 = 1f / Math.Pow(2f * Math.PI * f, 2);
            k3 = (r * c) / (2 * Math.PI * f);

            xCur = transform.position.x;
            xNew = m_moveTouchPos.x;

            // Calculations
            Vx = (xNew - xCur) / t;
            if (Vx == 0.0f)
            {
                Vy = 0;
                return;
            }

            y = y + t * Vy;
            Vy = (float) (y + (t * (xNew + k3 * Vx - y - k1 * Vy) / k2));

            // Check variables
            Debug.Log("------------------------");
            Debug.Log("xCur = " + xCur + " xNew = " + xNew + " Vx = " + Vx + "\n");
            Debug.Log("y = " + y + " Vy = " + Vy + "\n");
            Debug.Log("k1 = " + k1 + " k2 = " + k2 + " k3 = " + k3 + " t = " + t);

            // Change current location
            var location = transform.position;
            location.x = y;
            transform.position = location;
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
