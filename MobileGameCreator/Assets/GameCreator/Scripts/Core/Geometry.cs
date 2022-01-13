using UnityEngine;


namespace GameCreator
{
    public static class Geometry
    {
        public static Vector3[] GetPoints(Bounds bounds)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            Vector3[] points = new Vector3[]
            {
                new Vector3(min.x, min.y, min.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(max.x, max.y, min.z),
                new Vector3(max.x, min.y, min.z),

                new Vector3(min.x, min.y, max.z),
                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, max.y, max.z),
                new Vector3(max.x, min.y, max.z),
            };

            return points;
        }

        public static Bounds CalculateBounds(Bounds bounds, Matrix4x4 transform) =>
            GeometryUtility.CalculateBounds(GetPoints(bounds), transform);

        public static Rect WorldBoundsToScreenRect(Bounds bounds, Camera camera) =>
            WorldPointsToScreenRect(GetPoints(bounds), camera);

        public static Rect WorldPointsToScreenRect(Vector3[] points, Camera camera)
        {
            Rect rect = Rect.MinMaxRect(
                float.MaxValue, float.MaxValue,
                float.MinValue, float.MinValue
            );

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 v = camera.WorldToScreenPoint(points[i]);

                if (v.x < rect.xMin) rect.xMin = v.x;
                if (v.y < rect.yMin) rect.yMin = v.y;
                if (v.x > rect.xMax) rect.xMax = v.x;
                if (v.y > rect.yMax) rect.yMax = v.y;
            }
            return rect;
        }
    }
}