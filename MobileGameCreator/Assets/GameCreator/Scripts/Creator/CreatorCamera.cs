using UnityEngine;

namespace GameCreator.Creator
{
    [RequireComponent(typeof(Camera))]
    public class CreatorCamera : MonoBehaviour
    {
        [SerializeField] private Transform focusPoint;
        [SerializeField] private float xRotation = 24f;
        [SerializeField] private float yRotation = 0f;
        [SerializeField] private float moveSpeed = 10f;

        [SerializeField] private float defaultZoomDistance = 5f;
        [SerializeField] private float minZoomDistance = 0.2f;
        [SerializeField] private float maxZoomDistance = 10f;
        [SerializeField] private float zoomSpeedScale = 2f;
        [SerializeField] private float rotationSpeedScale = 2f;

        [SerializeField] private float zoomDistance;

        public Camera cam => _cam;
        private Camera _cam;

        public Vector3 flatForward
        {
            get
            {
                Vector3 position = transform.position;
                Vector3 focusPosition = focusPoint.position;

                Vector3 p = new Vector3(
                    position.x,
                    focusPosition.y,
                    position.z
                );

                return (focusPosition - p).normalized;
            }
        }

        private void Awake()
        {
            zoomDistance = defaultZoomDistance;
            _cam = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            Vector3 targetPosition = focusPoint.position - (transform.forward * zoomDistance);

            transform.position = Vector3.Slerp(
                transform.position,
                targetPosition,
                moveSpeed * Time.unscaledDeltaTime
            );
        }

        public void RotateY(float angleDelta)
        {
            float y = yRotation + (angleDelta * rotationSpeedScale);

            if (y > 180f)
                y -= 360f;

            else if (y < -180f)
                y += 360f;

            yRotation = y;
        }


        public void Zoom(float delta)
        {
            float focusDistance = Vector3.Distance(transform.position, focusPoint.position);

            zoomDistance = Mathf.Clamp(
                focusDistance + (delta * zoomSpeedScale),
                minZoomDistance,
                maxZoomDistance
            );
        }


        public void Move(Vector3 delta)
        {
            Vector3 forward = flatForward;
            Vector3 right = Vector3.Cross(forward, Vector3.up);

            Vector3 offset = (right * delta.x) + (forward * delta.z);
            focusPoint.transform.position += offset;
        }

        public void SetActive(bool active) =>
            gameObject.SetActive(active);
    }
}
