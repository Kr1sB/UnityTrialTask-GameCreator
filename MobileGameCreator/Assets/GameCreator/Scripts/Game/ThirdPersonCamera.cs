using UnityEngine;


namespace GameCreator.Game
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] private float distance = 5;

        [SerializeField] private float minXRotation = 7;
        [SerializeField] private float maxXRotation = 64;
        [SerializeField] private float xRotation = 24;
        [SerializeField] private float yRotation = 0;

        [SerializeField] private float speed = 5;
        [SerializeField] private float rotationSpeed = 5;

        private Quaternion targetRotation;
        private Vector3 targetPosition;

        public Vector3 flatForward
        {
            get
            {
                Vector3 position = transform.position;
                Vector3 targetPosition = target.position;

                Vector3 p = new Vector3(
                    position.x,
                    targetPosition.y,
                    position.z
                );

                return (targetPosition - p).normalized;
            }
        }

        private void Awake()
        {
            if (target != null)
                SnapToTarget();
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
            SnapToTarget();
        }

        private void SnapToTarget()
        {
            transform.rotation = Quaternion.Euler(xRotation, target.rotation.eulerAngles.y, 0);
            transform.position = target.position - (transform.forward * distance);
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            float x = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
            float y = target.rotation.eulerAngles.y + yRotation;
            targetRotation = Quaternion.Euler(x, y, 0);
            targetPosition = target.position - (transform.forward * distance);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.unscaledDeltaTime
            );

            transform.position = Vector3.Slerp(
                transform.position,
                targetPosition,
                speed * Time.unscaledDeltaTime
            );
        }
    }
}