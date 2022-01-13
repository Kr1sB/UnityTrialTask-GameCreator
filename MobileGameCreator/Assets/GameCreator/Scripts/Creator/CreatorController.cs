using GameCreator.Elements;
using GameCreator.Model;
using GameCreator.Weather;
using UnityEngine;

//TODO: Extend raycasting functions to support overhangs!

namespace GameCreator.Creator
{
    public class CreatorController : MonoBehaviour
    {
        public delegate void GameElementEvent(GameElement element);
        public event GameElementEvent onAddElement;
        public event GameElementEvent onDeleteElement;

        public static CreatorController instance { get; private set; }

        [SerializeField] private GameElementPalette _elementPalette;
        public GameElementPalette elementPalette => _elementPalette;

        [SerializeField] private Transform elementContainer;

        [SerializeField] private UI.CreatorUI _ui;
        public UI.CreatorUI ui => _ui;
        public CreatorCamera creatorCam;

        [SerializeField] private LayerMask _floorRaycastMask;
        public LayerMask floorRaycastMask => _floorRaycastMask;

        [SerializeField] private LayerMask _boxCastMask;
        public LayerMask boxCastMask => _boxCastMask;

        private GameProject _project;
        public GameProject project => _project;

        private void Awake()
        {
            if (instance != null)
                throw new System.Exception("Multiple instances of " + GetType());

            instance = this;

            elementContainer.gameObject.SetActive(false);
        }

        public void OnEnter()
        {
            gameObject.SetActive(true);
            elementContainer.gameObject.SetActive(true);

            App.instance.EnableScreenSleep(true);
            _ui.Show();
            creatorCam.SetActive(true);
            WeatherController.instance.BindEffectBox(creatorCam.transform);
        }


        public void OnExit()
        {
            _ui.Hide();
            creatorCam.SetActive(false);
            elementContainer.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public void LoadProject(GameProject gameProject)
        {
            //TODO(cb): Actually load the project from a file/stream
            _project = gameProject;
        }


        public GameElement NewElementInstance(GameElement prefab)
        {
            GameElement element = Instantiate(prefab);
            _project.Add(element);
            element.transform.SetParent(elementContainer);
            element.SpawnInCreator();

            onAddElement?.Invoke(element);
            return element;
        }

        public void DeleteElement(GameElement element)
        {
            Destroy(element.gameObject);
            _project.Remove(element.instanceId);
            onDeleteElement?.Invoke(element);
        }


        public void AdjustElementPosition(GameElement element)
        {
            Bounds worldBounds = element.worldBounds;

            bool didBoxHit = Physics.BoxCast(
                element.desiredPosition + new Vector3(0, worldBounds.size.y),
                worldBounds.extents,
                Vector3.down,
                out RaycastHit hit,
                Quaternion.identity,
                worldBounds.size.y,
                floorRaycastMask | boxCastMask
            );

            if (!didBoxHit)
                return;

            Vector3 position = element.desiredPosition;
            position.y = hit.point.y;

            Vector3 worldBoundsSize = worldBounds.extents;
            position.y += worldBoundsSize.y;

            element.transform.position = position;
        }


        public bool RaycastElement(GameElement element, Ray ray, float rayLength, out Vector3 desiredPosition, out Vector3 targetPosition)
        {
            if (!Physics.Raycast(ray, out RaycastHit floorHit, rayLength, floorRaycastMask))
            {
                targetPosition = element.transform.position;
                desiredPosition = element.desiredPosition;
                return false;
            }

            Vector3 hitPosition = floorHit.point;

            Vector3 offset = new Vector3(0, 20, 0);
            Vector3 boxOrigin = hitPosition + offset;

            Bounds bounds = element.bounds;
            bool didBoxHit = Physics.BoxCast(
                boxOrigin + bounds.center,
                bounds.extents * element.objectScale,
                Vector3.down,
                out RaycastHit boxHit,
                element.transform.rotation,
                rayLength,
                boxCastMask
            );

            Vector3 desired = floorHit.point;
            if (didBoxHit)
                desired.y = boxHit.point.y;

            desiredPosition = desired;

            Vector3 position = floorHit.point;
            if (didBoxHit)
                position.y = boxHit.point.y;

            Vector3 worldBoundsSize = element.worldBounds.extents;
            position.y += worldBoundsSize.y;
            targetPosition = position;
            return true;
        }


        public bool RaycastElement(GameElement element, Vector2 screenPoint, float rayLength, Camera cam, out Vector3 desiredPosition, out Vector3 targetPosition) =>
            RaycastElement(element, cam.ScreenPointToRay(screenPoint), rayLength, out desiredPosition, out targetPosition);

        public bool RaycastElement(GameElement element, Vector2 screenPoint, out Vector3 desiredPosition, out Vector3 targetPosition) =>
            RaycastElement(element, screenPoint, 100, creatorCam.cam, out desiredPosition, out targetPosition);
    }
}
