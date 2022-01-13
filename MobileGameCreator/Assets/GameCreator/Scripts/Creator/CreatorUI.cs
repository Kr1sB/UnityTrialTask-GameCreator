using GameCreator.UI;
using GameCreator.Elements;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace GameCreator.Creator.UI
{
    public class CreatorUI : MonoBehaviour
    {
        private enum InteractionState
        {
            Idle,
            MoveCamera,
            RotateAndZoomCamera,
            AddNewElement,
            EditElement
        }

        [SerializeField] private Camera _camera;
        public Camera uiCamera => _camera;

        [SerializeField] private UIView mainOptions;
        [SerializeField] private ElementPalettePanel elementPalettePanel;
        [SerializeField] private SkySettingsWindow skySettingsWindow;
        [SerializeField] private MultiTouchArea touchArea;

        [SerializeField] private RectTransform playButton;
        [SerializeField] private RectTransform playHint;

        [SerializeField] private float cameraDragScale = 1f;
        [SerializeField] private float pixelsPerUnit = 100f;


        [SerializeField] private GameElementGizmo elementGizmo;
        private CreatorCamera cam;

        private UIView currentView;
        private Stack<UIView> viewStack = new Stack<UIView>();
        private GameElement selectedElement;
        private Vector3 selectedElementTargetPosition;

        private CreatorController controller;
        private InteractionState interaction = InteractionState.Idle;

        private void Start()
        {
            controller = CreatorController.instance;
            cam = controller.creatorCam;

            controller.onAddElement += _ => ElementsHaveChanged();
            controller.onDeleteElement += _ => ElementsHaveChanged();

            //
            // Set up UI
            //

            touchArea.onSingleTouchDragStart += OnSingleTouchDragStart;
            touchArea.onSingleTouchEnd += OnSingleTouchEnd;

            touchArea.onDoubleTouchStart += OnDoubleTouchStart;

            currentView = mainOptions;

            elementPalettePanel.Initialize(controller.elementPalette);
            elementPalettePanel.onStartDragElement += OnStartDragNewElement;

            HidePlayButton();

            //
            // Set up element gizmo
            //

            elementGizmo.onRotateStart += RotateElementStart;
            elementGizmo.onRotate += RotateElement;
            elementGizmo.onRotateEnd += RotateElementEnd;

            elementGizmo.onElevationStart += ElevateElementStart;
            elementGizmo.onElevation += ElevateElement;
            elementGizmo.onElevationEnd += ElevateElementEnd;

            elementGizmo.onTranslateStart += TranslateElementStart;
            elementGizmo.onTranslate += TranslateElement;
            elementGizmo.onTranslateEnd += TranslateElementEnd;

            elementGizmo.onScaleStart += ScaleElementStart;
            elementGizmo.onScale += ScaleElement;
            elementGizmo.onScaleEnd += ScaleElementEnd;
        }

        private void ElementsHaveChanged()
        {
            if (controller.project.ContainsAnyOf(GameElement.Category.Player))
                ShowPlayButton();
            else
                HidePlayButton();
        }

        private void ShowPlayButton()
        {
            playButton.gameObject.SetActive(true);
            playHint.gameObject.SetActive(false);
        }

        private void HidePlayButton()
        {
            playButton.gameObject.SetActive(false);
            playHint.gameObject.SetActive(true);
        }

        public void DeleteSelectedElement()
        {
            if (selectedElement == null)
                return;

            controller.DeleteElement(selectedElement);
            DeselectElement();
        }

        private void RotateElementStart(Vector2 delta)
        {
            if (interaction != InteractionState.Idle)
                return;

            interaction = InteractionState.EditElement;
            selectedElement.DisableCollider();
        }

        private void RotateElement(Vector2 delta)
        {
            Vector2 worldDelta = new Vector2(
                delta.y / pixelsPerUnit,
                delta.x / pixelsPerUnit
            );

            selectedElement.transform.Rotate(worldDelta, Space.Self);
            controller.AdjustElementPosition(selectedElement);
        }

        private void RotateElementEnd(Vector2 delta)
        {
            interaction = InteractionState.Idle;

            controller.AdjustElementPosition(selectedElement);
            selectedElement.EnableCollider();
        }

        private void TranslateElementStart(Vector2 delta)
        {
            if (interaction != InteractionState.Idle)
                return;

            interaction = InteractionState.EditElement;
            selectedElement.DisableCollider();
        }


        private void TranslateElement(Vector2 delta)
        {
            Vector2 worldDelta = delta / pixelsPerUnit;

            Vector3 forward = controller.creatorCam.flatForward;
            Vector3 right = Vector3.Cross(forward, Vector3.up);

            Vector3 offset = (right * -worldDelta.x) + (forward * worldDelta.y);

            Vector3 p = selectedElement.transform.position + offset;
            Ray ray = new Ray(p + new Vector3(0, 20, 0), Vector3.down);

            if (controller.RaycastElement(selectedElement, ray, 100, out Vector3 desiredPosition, out Vector3 position))
            {
                if (!draggingElement)
                {
                    draggingElement = true;
                    selectedElement.transform.position = position;
                }

                selectedElement.desiredPosition = desiredPosition;
                selectedElementTargetPosition = position;
            }
        }

        private void TranslateElementEnd(Vector2 delta)
        {
            EndDragElement();
        }

        private void ElevateElementStart(float delta)
        {
            if (interaction != InteractionState.Idle)
                return;

            interaction = InteractionState.EditElement;
            selectedElement.DisableCollider();
        }


        private void ElevateElement(float delta)
        {
            float worldDelta = delta / pixelsPerUnit;

            selectedElement.Translate(new Vector3(0, worldDelta, 0));
            controller.AdjustElementPosition(selectedElement);
        }


        private void ElevateElementEnd(float delta)
        {
            interaction = InteractionState.Idle;

            controller.AdjustElementPosition(selectedElement);
            selectedElement.desiredPosition = selectedElement.transform.position;

            selectedElement.EnableCollider();
        }


        private void ScaleElementStart(float delta)
        {
            if (interaction != InteractionState.Idle)
                return;

            interaction = InteractionState.EditElement;
            selectedElement.DisableCollider();
        }


        private void ScaleElement(float delta)
        {
            float worldDelta = delta / pixelsPerUnit;
            selectedElement.objectScale += worldDelta;

            controller.AdjustElementPosition(selectedElement);
        }


        private void ScaleElementEnd(float delta)
        {
            interaction = InteractionState.Idle;

            controller.AdjustElementPosition(selectedElement);
            selectedElement.EnableCollider();
        }




        private void OnDoubleTouchStart(MultiTouchArea.DoubleTouchInfo doubleTouch)
        {
            if (interaction != InteractionState.Idle)
                return;

            interaction = InteractionState.RotateAndZoomCamera;
            touchArea.onDoubleTouch += OnDoubleTouch;
            touchArea.onDoubleTouchEnd += OnDoubleTouchEnd;
        }

        private void OnDoubleTouch(MultiTouchArea.DoubleTouchInfo doubleTouch)
        {
            cam.Zoom(doubleTouch.distanceDelta / pixelsPerUnit);
            cam.RotateY(doubleTouch.angleDelta);
        }

        private void OnDoubleTouchEnd(MultiTouchArea.DoubleTouchInfo doubleTouch)
        {
            interaction = InteractionState.Idle;
            touchArea.onDoubleTouch -= OnDoubleTouch;
            touchArea.onDoubleTouchEnd -= OnDoubleTouchEnd;
        }

        private void Update()
        {
            if (draggingElement)
            {
                float speed = 20f;
                selectedElement.transform.position = Vector3.Slerp(
                    selectedElement.transform.position,
                    selectedElementTargetPosition,
                    speed * Time.unscaledDeltaTime
                );
            }
        }



        private void SelectElement(GameElement element)
        {
            if (element != null && element == selectedElement)
                return;

            DeselectElement();

            selectedElement = element;

            if (selectedElement == null)
                return;

            elementGizmo.SetElement(selectedElement);
            elementGizmo.Show();
        }

        private void DeselectElement()
        {
            if (selectedElement == null)
                return;

            elementGizmo.SetElement(null);
            elementGizmo.Hide();
            selectedElement.EnableCollider();
            selectedElement = null;
        }


        private bool draggingElement;

        private void OnStartDragNewElement(GameElement prefab, PointerEventData eventData)
        {
            if (interaction != InteractionState.Idle)
                return;

            if (prefab.metadata.category == GameElement.Category.Player &&
                controller.project.ContainsAnyOf(GameElement.Category.Player))
            {
                return;
            }

            interaction = InteractionState.AddNewElement;

            elementPalettePanel.onDragElement += OnDragNewElement;
            elementPalettePanel.onEndDragElement += OnEndDragNewElement;

            GameElement element = controller.NewElementInstance(prefab);
            SelectElement(element);
            elementGizmo.Hide();
        }

        private void OnDragNewElement(PointerEventData eventData)
        {
            if (controller.RaycastElement(selectedElement, eventData.position, out Vector3 desiredPosition, out Vector3 position))
            {
                if (!draggingElement)
                {
                    draggingElement = true;
                    selectedElement.transform.position = position;
                }

                selectedElement.desiredPosition = desiredPosition;
                selectedElementTargetPosition = position;
            }
        }


        private void OnEndDragNewElement(PointerEventData eventData)
        {
            elementPalettePanel.onDragElement -= OnDragNewElement;
            elementPalettePanel.onEndDragElement -= OnEndDragNewElement;

            EndDragElement();
        }

        private void EndDragElement()
        {
            interaction = InteractionState.Idle;
            draggingElement = false;

            selectedElement.transform.position = selectedElementTargetPosition;
            selectedElement.desiredPosition = selectedElementTargetPosition;

            selectedElement.EnableCollider();

            elementGizmo.Show();
        }



        public void ShowSkySettings()
        {
            PushView(skySettingsWindow);
        }


        private void OnSingleTouchDragStart(MultiTouchArea.TouchInfo touch)
        {
            if (interaction != InteractionState.Idle)
                return;

            touchArea.onSingleTouchDragEnd += OnSingleTouchDragEnd;
            touchArea.onSingleTouchDrag += OnSingleTouchDrag;

            interaction = InteractionState.MoveCamera;
        }

        private void OnSingleTouchDrag(MultiTouchArea.TouchInfo touch)
        {
            Vector2 delta = new Vector2(touch.delta.x, -touch.delta.y) * cameraDragScale;

            Vector3 worldDelta = new Vector3(
                delta.x / pixelsPerUnit,
                0,
                delta.y / pixelsPerUnit
            );

            cam.Move(worldDelta);
        }

        private void OnSingleTouchDragEnd(MultiTouchArea.TouchInfo touch)
        {
            touchArea.onSingleTouchDragEnd -= OnSingleTouchDragEnd;
            touchArea.onSingleTouchDrag -= OnSingleTouchDrag;

            interaction = InteractionState.Idle;
        }


        private void OnSingleTouchEnd(MultiTouchArea.TouchInfo touch)
        {
            if (interaction == InteractionState.Idle)
            {
                Camera cam = controller.creatorCam.cam;
                Ray ray = cam.ScreenPointToRay(touch.position);

                bool deselect = false;

                if (Physics.Raycast(ray, out RaycastHit hit, 100, controller.boxCastMask))
                {
                    GameElement element = hit.collider.GetComponent<GameElement>();

                    if (element != null)
                        SelectElement(element);
                    else
                        deselect = true;
                }
                else
                {
                    deselect = true;
                }

                if (deselect)
                    DeselectElement();
            }

            interaction = InteractionState.Idle;
        }


        public void PushView(UIView view)
        {
            currentView.Hide();
            viewStack.Push(currentView);

            currentView = view;
            currentView.Show();
        }

        public void PopView()
        {
            currentView.Hide();
            currentView = viewStack.Pop();
            currentView.Show();
        }

        public void Show(bool show = true)
        {
            gameObject.SetActive(show);
        }

        public void Hide() =>
            Show(false);
    }
}
