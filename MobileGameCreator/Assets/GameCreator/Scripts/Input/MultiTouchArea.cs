using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

//TODO(cb): Add support for simulating double touches in the editor

public class MultiTouchArea : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler,
    IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public delegate void SingleTouchEvent(TouchInfo touch);
    public delegate void DoubleTouchEvent(DoubleTouchInfo doubleTouch);

    public event SingleTouchEvent onSingleTouchStart;
    public event SingleTouchEvent onSingleTouchEnd;

    public event SingleTouchEvent onSingleTouchDragStart;
    public event SingleTouchEvent onSingleTouchDrag;
    public event SingleTouchEvent onSingleTouchDragEnd;

    public event DoubleTouchEvent onDoubleTouchStart;
    public event DoubleTouchEvent onDoubleTouch;
    public event DoubleTouchEvent onDoubleTouchEnd;

    private Coroutine trackDoubleTouchCoroutine;

    public struct TouchInfo
    {
        public int id;
        public Vector2 position;
        public Vector2 delta;
    }

    public struct DoubleTouchInfo
    {
        public float distance;
        public float distanceDelta;

        public Vector2 direction;
        public float angleDelta;

        public TouchInfo primaryTouch;
        public TouchInfo secondaryTouch;
    }

    private TouchInfo primaryTouch;
    private TouchInfo secondaryTouch;
    private DoubleTouchInfo doubleTouch;

    private int touchCount;

    private void PrimaryTouchStarted() =>
        onSingleTouchStart?.Invoke(primaryTouch);

    private void PrimaryTouchEnded()
    {
        touchCount = 0;
        onSingleTouchEnd?.Invoke(primaryTouch);
    }

    private void StartDoubleTouch()
    {
        trackDoubleTouchCoroutine = StartCoroutine(TrackDoubleTouch());
    }

    private void EndDoubleTouch()
    {
        StopCoroutine(trackDoubleTouchCoroutine);
        trackDoubleTouchCoroutine = null;
        touchCount = 0;

        onDoubleTouchEnd?.Invoke(doubleTouch);
    }

    private IEnumerator TrackDoubleTouch()
    {
        Vector2 initialDirection = (secondaryTouch.position - primaryTouch.position).normalized;
        Vector2 previousDirection = initialDirection;

        float initialDistance = Vector2.Distance(
            primaryTouch.position,
            secondaryTouch.position
        );

        float previousDistance = initialDistance;
        bool isFirstIteration = true;

        for (;;)
        {
            float distance = Vector2.Distance(
                primaryTouch.position,
                secondaryTouch.position
            );

            float distanceDelta = distance - previousDistance;
            Vector2 direction = (secondaryTouch.position - primaryTouch.position).normalized;
            float angleDelta = Vector2.SignedAngle(direction, previousDirection);

            doubleTouch = new DoubleTouchInfo()
            {
                distance = distance,
                distanceDelta = distanceDelta,
                angleDelta = angleDelta,
                primaryTouch = primaryTouch,
                secondaryTouch = secondaryTouch
            };

            if (isFirstIteration)
            {
                onDoubleTouchStart?.Invoke(doubleTouch);
                isFirstIteration = false;
            }
            else
            {
                onDoubleTouch?.Invoke(doubleTouch);
            }

            previousDistance = distance;
            previousDirection = direction;
            yield return null;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (touchCount == 1 && eventData.pointerId == primaryTouch.id)
        {
            primaryTouch.position = eventData.position;
            primaryTouch.delta = eventData.delta;

            onSingleTouchDragStart?.Invoke(primaryTouch);
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == primaryTouch.id)
        {
            primaryTouch.position = eventData.position;
            primaryTouch.delta = eventData.delta;
        }
        else if (eventData.pointerId == secondaryTouch.id)
        {
            secondaryTouch.position = eventData.position;
            secondaryTouch.delta = eventData.delta;
        }

        if (touchCount == 1)
            onSingleTouchDrag?.Invoke(primaryTouch);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (touchCount == 1 && eventData.pointerId == primaryTouch.id)
        {
            primaryTouch.position = eventData.position;
            primaryTouch.delta = eventData.delta;

            onSingleTouchDragEnd?.Invoke(primaryTouch);
        }
    }



    public void OnPointerUp(PointerEventData eventData)
    {
        if (touchCount == 0 || (eventData.pointerId != primaryTouch.id && eventData.pointerId != secondaryTouch.id))
            return;

        if (touchCount == 2)
            EndDoubleTouch();
        else
            PrimaryTouchEnded();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (touchCount == 2)
            return;

        ++touchCount;

        if (touchCount == 2)
        {
            secondaryTouch.id = eventData.pointerId;
            secondaryTouch.position = eventData.position;
            secondaryTouch.delta = Vector2.zero;

            StartDoubleTouch();
        }
        else
        {
            primaryTouch.id = eventData.pointerId;
            primaryTouch.position = eventData.position;
            primaryTouch.delta = Vector2.zero;

            PrimaryTouchStarted();
        }
    }

}
