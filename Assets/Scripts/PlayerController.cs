using UnityEngine;
using UniRx;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveTouchPos = Vector2.zero;
    private float moveValue = 0.0f;
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
            moveTouchPos = touchPosition;
            CalculateMoveVector(touchPosition);
        });

        touchInputManager.OnTouchMove.Subscribe(touchPosition =>
        {
            moveTouchPos = touchPosition;
            CalculateMoveVector(touchPosition);
        });

        touchInputManager.OnTouchEnd.Subscribe(touchPosition =>
        {
            StopMovement();
        });
    }

    private void StopMovement()
    {
        moveTouchPos = Vector2.zero;
        moveValue = 0.0f;
    }

    private void CalculateMoveVector(Vector2 touchPosition)
    {
        if (touchPosition.x > Screen.width / 2)
            moveValue = 1.0f;
        else
            moveValue = -1.0f;
    }

    private void MovePlayer()
    {
        Vector2 moveVector = moveValue* Time.deltaTime * playerData.Speed * Vector2.right;
        transform.Translate(moveVector);
    }
}
