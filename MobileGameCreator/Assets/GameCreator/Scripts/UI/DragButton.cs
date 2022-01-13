using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCreator.UI
{
    public class DragButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public delegate void DragButtonEvent(Vector2 delta);
        public event DragButtonEvent onDragStart;
        public event DragButtonEvent onDrag;
        public event DragButtonEvent onDragEnd;

        [SerializeField] private bool lockXAxis;
        [SerializeField] private bool lockYAxis;

        public void OnBeginDrag(PointerEventData eventData)
        {
            onDragStart?.Invoke(GetDelta(eventData.delta));
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(GetDelta(eventData.delta));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onDragEnd?.Invoke(GetDelta(eventData.delta));
        }

        private Vector2 GetDelta(Vector2 delta)
        {
            if (lockXAxis)
                delta.x = 0;

            if (lockYAxis)
                delta.y = 0;

            return delta;
        }
    }
}