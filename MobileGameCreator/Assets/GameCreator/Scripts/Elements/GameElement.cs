using UnityEngine;

namespace GameCreator.Elements
{
    public class GameElement : MonoBehaviour
    {
        public enum Category
        {
            PhysicsObject,
            StaticObject,
            Player,
        }

        [System.Serializable]
        public class Metadata
        {
            public string name;
            public Sprite icon;
            public Category category;
        }

        public Metadata metadata;

        public Bounds bounds;
        public Bounds worldBounds =>
            Geometry.CalculateBounds(bounds, transform.localToWorldMatrix);

        [SerializeField] protected float _objectScale = 1f;
        public float objectScale
        {
            get => _objectScale;

            set
            {
                _objectScale = Mathf.Clamp(value, minScale, maxScale);
                transform.localScale = _objectScale * Vector3.one;
            }
        }

        public float minScale = 0.05f;
        public float maxScale = 5f;

        public Collider mainCollider;
        public Rigidbody rigidBody;

        [HideInInspector]
        public Vector3 desiredPosition;
        [HideInInspector] public uint instanceId;

        public void SpawnInCreator()
        {
            OnBeforeSpawn();
            DisablePhysics();
            DisableCollider();
            OnSpawnInCreator();
        }

        virtual protected void OnSpawnInCreator() { }

        public void SpawnInGame()
        {
            OnBeforeSpawn();
            EnablePhysics();
            EnableCollider();
            OnSpawnInGame();
        }

        virtual protected void OnSpawnInGame() { }

        virtual protected void OnBeforeSpawn() { }

        virtual public void EnableCollider()
        {
            if (mainCollider == null)
                return;

            mainCollider.enabled = true;
        }

        virtual public void DisableCollider()
        {
            if (mainCollider == null)
                return;

            mainCollider.enabled = false;
        }

        virtual public void EnablePhysics()
        {
            if (rigidBody == null)
                return;

            rigidBody.isKinematic = false;
        }

        virtual public void DisablePhysics()
        {
            if (rigidBody == null)
                return;

            rigidBody.isKinematic = true;
        }

        public void Translate(Vector3 delta)
        {
            desiredPosition += delta;
            transform.position += delta;
        }

        private void OnDrawGizmos()
        {
            var m = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                transform.localScale
            );

            Bounds b = worldBounds;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(b.center, b.size);


            Gizmos.matrix = Matrix4x4.TRS(
                transform.position,
                transform.rotation,
                transform.localScale
            );

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(bounds.center, bounds.size);

        }
    }
}