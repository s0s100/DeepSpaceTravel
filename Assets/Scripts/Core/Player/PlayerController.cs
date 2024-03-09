using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;
using Zenject;
using Config;
using Controls;

namespace Core.Player
{
    public enum MovementType
    {
        HorizontalMovement,
        GlobalMovement
    }

    public class PlayerController : MonoBehaviour
    {
        [Inject] private readonly ConfigData _config;

        [Header("Input type config")]
        [SerializeField] private MovementType movementType = MovementType.GlobalMovement;

        // Used to simplify control type with lambda function
        private readonly Dictionary<MovementType, Action> actionDictionary = new();

        // Movement subscription
        private IDisposable _movementSubscription;

        private PlayerData _playerData;
        private Rigidbody2D _rigidbody;
        private Vector2 _moveTouchPos = Vector2.zero;
        private Vector2 _startTouchPos = Vector2.zero;
        private Vector2 _startPlayerPosition = Vector2.zero;

        // Move to config files as well
        // Physics movement
        [SerializeField]
        private float f = 0.5f, c = 0.15f, r = 2f;
        private float k1, k2, k3;
        private float xSpeed;
        private float yPos, yVel, yAcc;


        private void PopulateActionDictionary()
        {
            actionDictionary[MovementType.HorizontalMovement] = () => AdvancedHorizontalMovement();
            actionDictionary[MovementType.GlobalMovement] = () => Advanced2DMovement();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerData = GetComponent<PlayerData>();
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
            _movementSubscription.Dispose();
        }

        private void SubscribeToMovement()
        {
            _movementSubscription?.Dispose();

            var moveAction = actionDictionary[movementType];
            _movementSubscription = Observable
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
                _moveTouchPos = ScreenInfo.GetWorldTouchPos(touchPosition);
                _startTouchPos = _moveTouchPos;
                _startPlayerPosition = transform.position;
            });

            touchInputManager.OnTouchMove.Subscribe(touchPosition =>
            {
                _moveTouchPos = ScreenInfo.GetWorldTouchPos(touchPosition);
            });

            touchInputManager.OnTouchEnd.Subscribe(touchPosition =>
            {
                StopMovement();
            });
        }

        private bool CanMove(Vector2 newPosition)
        {
            // Divided by 2 to have a bounary between screen and the right side of the screen equal to 2
            float playerXBoundary = _playerData.CalculateXSize() / 2;
            float maxXScreenPos = ScreenInfo.GetMaxXPos();
            float minXScreenPos = ScreenInfo.GetMinXPos();

            bool canMoveLeft = (minXScreenPos + playerXBoundary < newPosition.x);
            bool canMoveRight = (maxXScreenPos - playerXBoundary > newPosition.x);

            return canMoveLeft && canMoveRight;
        }

        private void StopMovement()
        {
            _moveTouchPos = transform.position;
            _startTouchPos = transform.position;
            _startPlayerPosition = transform.position;
        }

        private float CalculateHorizontalDirection(Vector2 comparePosition)
        {
            var minTouchDistance = _config.control.minTouchDistance;

            if (_moveTouchPos.x > comparePosition.x + minTouchDistance)
                return 1.0f;
            else if (_moveTouchPos.x < comparePosition.x - minTouchDistance)
                return -1.0f;

            return 0.0f;
        }

        private void ImprovedMovementSetup()
        {
            // Caclulate default constunts
            k1 = c / (float)(Math.PI * f);
            k2 = 1f / (float)Math.Pow(2f * Math.PI * f, 2);
            k3 = (r * c) / (float)(2 * Math.PI * f);

            yPos = transform.position.x;
            yVel = 0.0f;
            yAcc = 0.0f;
        }

        // X Refers to basic limited movement while Y to improved movement
        private void AdvancedHorizontalMovement()
        {
            float xDirection = CalculateHorizontalDirection(_moveTouchPos);
            if (xDirection != 0.0f)
                xSpeed = xDirection * _playerData.Speed;
            else
                xSpeed = 0.0f;

            yPos += yVel * Time.deltaTime;
            yAcc = (_moveTouchPos.x + k3 * xSpeed - yPos - k1 * yVel) / k2;
            yVel += yAcc * Time.deltaTime;

            Vector3 moveVector = (yPos - transform.position.x) * Vector3.right;
            Vector2 newLocation = transform.position + moveVector;

            if (CanMove(newLocation))
            {
                _rigidbody.MovePosition(newLocation);
            }
        }

        private void Advanced2DMovement()
        {

        }
    }
}
