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

        // Used to simplify control type with lambda function
        private readonly Dictionary<MovementType, Action> _actionDictionary = new();

        // Movement subscription
        private IDisposable _movementSubscription;

        private PlayerData _playerData;
        private Rigidbody2D _rigidbody;
        private Vector2 _moveTouchPos = Vector2.zero;
        private Vector2 _startTouchPos = Vector2.zero;
        private Vector2 _startPlayerPosition = Vector2.zero;

        private Vector2 movementSpeed = Vector2.zero;
        private Vector2 calculatedPos, calculatedVel, calculatedAcc;
        private float k1, k2, k3;

        private void PopulateActionDictionary()
        {
            _actionDictionary[MovementType.HorizontalMovement] = () => HorizontalMovement();
            _actionDictionary[MovementType.GlobalMovement] = () => GlobalMovement();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _playerData = GetComponent<PlayerData>();
            PopulateActionDictionary();
            MovementSetup();
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

            var movementType = _config.control.movementType;
            var moveAction = _actionDictionary[movementType];
            _movementSubscription = Observable
                    .EveryUpdate()
                    .Subscribe(_ =>
                    {
                        moveAction();
                    });

            if (movementType == MovementType.HorizontalMovement)
            {
                _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
                _rigidbody.freezeRotation = true;
            }
        }

        private void SubscribeToTouchInputManager()
        {
            // Get rid of FindObjectOfType method, use injection instead
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

        // TODO: Improve / Move to PlayerData / Remove (Choose one)
        private bool CanMove(Vector2 newPosition)
        {
            // Divided by 2 to have a bounary between screen and the right side of the screen equal to 2
            var playerBorders = _playerData.PlayerModelSize() / 2;
            var maxXScreenPos = ScreenInfo.GetMaxXPos();
            var minXScreenPos = ScreenInfo.GetMinXPos();
            var maxYScreenPos = ScreenInfo.GetMaxYPos();
            var minYScreenPos = ScreenInfo.GetMinYPos();

            bool canMoveLeft = (minXScreenPos + playerBorders.x < newPosition.x);
            bool canMoveRight = (maxXScreenPos - playerBorders.x > newPosition.x);
            bool canMoveDown = (minYScreenPos + playerBorders.y < newPosition.y);
            bool canMoveUp = (maxYScreenPos - playerBorders.y > newPosition.y);

            return canMoveLeft && canMoveRight & canMoveDown & canMoveUp;
        }

        private void StopMovement()
        {
            _moveTouchPos = transform.position;
            _startTouchPos = transform.position;
            _startPlayerPosition = transform.position;
        }

        private Vector2 CalculateDirection(Vector2 comparePosition)
        {
            var minTouchDistance = _config.control.minTouchDistance;
            var direction = Vector2.zero;

            if (_moveTouchPos.x > comparePosition.x + minTouchDistance)
                direction.x = 1.0f;
            else if (_moveTouchPos.x < comparePosition.x - minTouchDistance)
                direction.x = -1.0f;

            if (_moveTouchPos.y > comparePosition.y + minTouchDistance)
                direction.y = 1.0f;
            else if (_moveTouchPos.y < comparePosition.y + minTouchDistance)
                direction.y = -1.0f;

            return direction;
        }

        private void MovementSetup()
        {
            var f = _config.control.fCoefficient;
            var c = _config.control.cCoefficient;
            var r = _config.control.rCoefficient;

            // Caclulate default constunts
            k1 = c / (float)(Math.PI * f);
            k2 = 1f / (float)Math.Pow(2f * Math.PI * f, 2);
            k3 = (r * c) / (float)(2 * Math.PI * f);

            calculatedPos = transform.position;
            calculatedVel = Vector2.zero;
            calculatedAcc = Vector2.zero;
        }

        private void HorizontalMovement()
        {
            var movementDirection = CalculateDirection(_moveTouchPos);
            movementSpeed.x = movementDirection.x * _playerData.Speed;

            calculatedPos += calculatedVel * Time.deltaTime;
            calculatedAcc.x = (_moveTouchPos.x + k3 * movementSpeed.x - calculatedPos.x - k1 * calculatedVel.x) / k2;
            calculatedVel += calculatedAcc * Time.deltaTime;

            var moveVector = (calculatedPos.x - transform.position.x) * Vector2.right;
            moveVector *= _playerData.Speed;
            var newLocation = (Vector2)transform.position + moveVector;

            if (CanMove(newLocation))
            {
                _rigidbody.MovePosition(newLocation);
            }
        }

        // To implement
        private void GlobalMovement()
        {
            var movementDirection = CalculateDirection(_moveTouchPos);
            movementSpeed = movementDirection * _playerData.Speed;

            calculatedPos += calculatedVel * Time.deltaTime;
            calculatedAcc = (_moveTouchPos + k3 * movementSpeed - calculatedPos - k1 * calculatedVel) / k2;
            calculatedVel += calculatedAcc * Time.deltaTime;

            var moveVector = calculatedPos - (Vector2)transform.position;
            moveVector *= _playerData.Speed;
            var newLocation = (Vector2)transform.position + moveVector;

            if (CanMove(newLocation))
            {
                _rigidbody.MovePosition(newLocation);
            }
        }
    }
}
