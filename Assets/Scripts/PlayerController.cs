using UnityEngine;
using UniRx;

public class PlayerController : MonoBehaviour
{
    // Connects how strong should we pull our rocket to reach max acceleration
    private static float SCREEN_SIZE_CORELATION = 0.25f;

    private Vector2 startTouchPos = Vector2.zero;
    private float moveValue = 0.0f;
    private PlayerData playerData;

    private void Awake()
    {
        playerData = GetComponent<PlayerData>();
    }

    private void Start()
    {
        Debug.Log("Player Controller Start");

        // Get a reference to the TouchInputManager
        var touchInputManager = FindObjectOfType<TouchInputManager>();

        // Subscribe to touch input events
        touchInputManager.OnTouchStart.Subscribe(touchPosition =>
        {
            Debug.Log("I've touched it!");
            startTouchPos = touchPosition;
        });

        touchInputManager.OnTouchMove.Subscribe(touchPosition =>
        {
            Debug.Log("I've moved it!");
            CalculateMoveVector(touchPosition);
        });

        touchInputManager.OnTouchEnd.Subscribe(touchPosition =>
        {
            Debug.Log("I've ended it!");
            StopMovement();
        });

        // Periodical movement (should probably refactor)
        var periodicMovementObservator = Observable
            .EveryUpdate()
            .Subscribe(_ =>
            {
                MovePlayer();
            });
    }

    private void StopMovement()
    {
        startTouchPos = Vector2.zero;
        moveValue = 0.0f;
    }

    private void CalculateMoveVector(Vector2 touchPosition)
    {
        // Get X move vector
        //moveValue = touchPosition.x - startTouchPos.x;
        float difference = touchPosition.x - startTouchPos.x;

        // Normilize according to screen size
        moveValue = difference /  (Screen.width * SCREEN_SIZE_CORELATION);
        moveValue = Mathf.Clamp(moveValue, -1f, 1f);

        Debug.Log(moveValue);
    }

    private void MovePlayer()
    {
        // Move player according to move value
        // Also check map boundaries (separate it as well to follow SOLID principles)
        Vector2 moveVector = moveValue* Time.deltaTime * playerData.Speed * Vector2.right;
        transform.Translate(moveVector);
    }
}
