using UnityEngine;
using System;
using UniRx;
using UniRx.Triggers;

namespace Controls
{
    public class TouchInputManager : MonoBehaviour
    {
        private readonly Subject<Vector2> touchStartSubject = new();
        private readonly Subject<Vector2> touchMoveSubject = new();
        private readonly Subject<Vector2> touchEndSubject = new();

        public IObservable<Vector2> OnTouchStart => touchStartSubject;
        public IObservable<Vector2> OnTouchMove => touchMoveSubject;
        public IObservable<Vector2> OnTouchEnd => touchEndSubject;

        private void Start()
        {
            // Subscribe to touch events using UniRx
            this.UpdateAsObservable()
                .Where(_ => Input.touchCount > 0)
                .Select(_ => Input.GetTouch(0))
                .Subscribe(touch =>
                {
                    Vector2 touchPosition = touch.position;
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            touchStartSubject.OnNext(touchPosition);
                            break;
                        case TouchPhase.Moved:
                            touchMoveSubject.OnNext(touchPosition);
                            break;
                        case TouchPhase.Ended:
                            touchEndSubject.OnNext(touchPosition);
                            break;
                    }
                })
                .AddTo(this);
        }
    }
}