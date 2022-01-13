using System;
using GameCreator.Elements;
using GameCreator.UI;
using UnityEngine;


namespace GameCreator.Creator.UI
{
    public class GameElementGizmo : MonoBehaviour
    {
        public delegate void Vector3DragEvent(Vector3 delta);
        public delegate void FloatDragEvent(float delta);

        public event DragButton.DragButtonEvent onTranslateStart;
        public event DragButton.DragButtonEvent onTranslate;
        public event DragButton.DragButtonEvent onTranslateEnd;

        public event DragButton.DragButtonEvent onRotateStart;
        public event DragButton.DragButtonEvent onRotate;
        public event DragButton.DragButtonEvent onRotateEnd;


        public event FloatDragEvent onElevationStart;
        public event FloatDragEvent onElevation;
        public event FloatDragEvent onElevationEnd;

        public event FloatDragEvent onScaleStart;
        public event FloatDragEvent onScale;
        public event FloatDragEvent onScaleEnd;


        [SerializeField] private float translationSensitivity = 1f;
        [SerializeField] private float elevationSensitivity = 1f;
        [SerializeField] private float rotationSensitivity = 1f;
        [SerializeField] private float scaleSensitivity = 1f;

        [SerializeField] private DragButton translateButton;
        [SerializeField] private DragButton elevationButton;
        [SerializeField] private DragButton rotateYButton;
        [SerializeField] private DragButton rotateXButton;
        [SerializeField] private DragButton scaleButton;

        private bool isVisible;
        private GameElement target;

        private void Awake()
        {
            gameObject.SetActive(false);

            translateButton.onDragStart += TranslateDragStart;
            translateButton.onDrag += TranslateDrag;
            translateButton.onDragEnd += TranslateDragEnd;

            elevationButton.onDragStart += ElevationDragStart;
            elevationButton.onDrag += ElevationDrag;
            elevationButton.onDragEnd += ElevationDragEnd;

            rotateXButton.onDragStart += RotateDragStart;
            rotateXButton.onDrag += RotateDrag;
            rotateXButton.onDragEnd += RotateDragEnd;

            rotateYButton.onDragStart += RotateDragStart;
            rotateYButton.onDrag += RotateDrag;
            rotateYButton.onDragEnd += RotateDragEnd;

            scaleButton.onDragStart += ScaleDragStart;
            scaleButton.onDrag += ScaleDrag;
            scaleButton.onDragEnd += ScaleDragEnd;
        }


        private void ScaleDragStart(Vector2 delta) =>
            onScaleStart?.Invoke(delta.y * scaleSensitivity);

        private void ScaleDrag(Vector2 delta) =>
            onScale?.Invoke(delta.y * scaleSensitivity);

        private void ScaleDragEnd(Vector2 delta) =>
            onScaleEnd?.Invoke(delta.y * scaleSensitivity);


        private void RotateDragStart(Vector2 delta) =>
            onRotateStart?.Invoke(delta * rotationSensitivity);

        private void RotateDrag(Vector2 delta) =>
            onRotate?.Invoke(delta * rotationSensitivity);

        private void RotateDragEnd(Vector2 delta) =>
            onRotateEnd?.Invoke(delta * rotationSensitivity);


        private void ElevationDragStart(Vector2 delta) =>
            onElevationStart?.Invoke(delta.y * elevationSensitivity);

        private void ElevationDrag(Vector2 delta) =>
            onElevation?.Invoke(delta.y * elevationSensitivity);

        private void ElevationDragEnd(Vector2 delta) =>
            onElevationEnd?.Invoke(delta.y * elevationSensitivity);

        private void TranslateDragStart(Vector2 delta) =>
            onTranslateStart?.Invoke(delta * translationSensitivity);

        private void TranslateDrag(Vector2 delta) =>
            onTranslate?.Invoke(delta * translationSensitivity);

        private void TranslateDragEnd(Vector2 delta) =>
            onTranslateEnd?.Invoke(delta * translationSensitivity);


        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (target == null)
                return;

            var controller = CreatorController.instance;
            Camera cam = controller.creatorCam.cam;
            Bounds bounds = target.worldBounds;

            Vector2 screenPoint = cam.WorldToScreenPoint(bounds.center);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent,
                screenPoint,
                controller.ui.uiCamera,
                out Vector2 localPosition
            );

            RectTransform rect = (RectTransform)transform;
            rect.anchoredPosition = localPosition;

            Rect screenRect = Geometry.WorldBoundsToScreenRect(bounds, cam);
            rect.sizeDelta = screenRect.size;
        }


        public void Show(bool show = true)
        {
            isVisible = show;
            gameObject.SetActive(show);

            if (show)
                UpdatePosition();
        }

        public void Hide() =>
            Show(false);

        public void SetElement(GameElement element)
        {
            target = element;
            UpdatePosition();
        }
    }
}